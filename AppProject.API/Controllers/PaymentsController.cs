using Core.Entities;
using Core.Entities.Order;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // تم إضافة هذا المجال
using Stripe;
using Stripe.Checkout;

namespace AppProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration; // 1. حقن Configuration

        // ⚠️ انتبه: هذا المفتاح يتغير في كل مرة تشغل فيها امر stripe listen جديد
        const string endpointSecret = "whsec_pzYSVLgNMqNsFI3SYnaYusdnWNkw4Zbi";

        public PaymentsController(
            IPaymentService paymentService, 
            IOrderService orderService, 
            IConfiguration configuration) // 2. استقبال Configuration في الـ Constructor
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<ActionResult<CustomerBasket>> Create(string basketId, int? deliveryId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentAsync(basketId, deliveryId);
            if (basket == null) return BadRequest("Problem with your basket");

            return Ok(basket);
        }

        [HttpPost("create-checkout-session")]
        public async Task<ActionResult> CreateCheckoutSession([FromQuery] string basketId, [FromQuery] int? deliveryId,
            [FromQuery] int orderId)
        {
            try
            {
                var basket = await _paymentService.CreateOrUpdatePaymentAsync(basketId, deliveryId);
                if (basket == null || basket.basketItems == null || !basket.basketItems.Any())
                    return BadRequest("Basket is empty or not found");

                decimal shippingPrice = 0;
                string deliveryName = "Standard Shipping";

                if (deliveryId.HasValue)
                {
                    var deliveryMethods = await _orderService.GetDeliveryMethodAsync();
                    var selectedMethod = deliveryMethods?.FirstOrDefault(d => d.Id == deliveryId.Value);
                    if (selectedMethod != null)
                    {
                        shippingPrice = selectedMethod.Price;
                        deliveryName = selectedMethod.Name;
                    }
                }

                var lineItems = basket.basketItems.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name ?? "Product",
                            Description = item.Description ?? string.Empty
                        },
                    },
                    Quantity = item.Qunatity,
                }).ToList();

                // 3. قراءة رابط الـ Client المباشر من appsettings.json للـ API
                //string clientUrl = _configuration["ClientSettings:BaseUrl"]?.TrimEnd('/') ?? "http://localhost:6001/";
                string clientUrl = _configuration["ClientSettings:BaseUrl"]?.TrimEnd('/') ?? "https://samer-kubernetes.infra-x.cloud/";
                
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,

                    // 1. إضافة orderId و basketId في Metadata الجلسة الأساسية
                    Metadata = new Dictionary<string, string>
                    {
                        { "orderId", orderId.ToString() },
                        { "basketId", basketId }
                    },

                    // 2. تمرير orderId و basketId إلى PaymentIntent القادم من Stripe
                    PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        Metadata = new Dictionary<string, string>
                        {
                            { "orderId", orderId.ToString() },
                            { "basketId", basketId }
                        }
                    },

                    ShippingOptions = new List<SessionShippingOptionOptions>
                    {
                        new SessionShippingOptionOptions
                        {
                            ShippingRateData = new SessionShippingOptionShippingRateDataOptions
                            {
                                Type = "fixed_amount",
                                FixedAmount = new SessionShippingOptionShippingRateDataFixedAmountOptions
                                {
                                    Amount = (long)(shippingPrice * 100),
                                    Currency = "usd",
                                },
                                DisplayName = deliveryName
                            }
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = $"{clientUrl}/CheckoutSuccess/{{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{clientUrl}/Basket"
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                return Ok(new { url = session.Url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        // السماح الصريح بالدخول بدون Auth
        [HttpPost("webhook")]
        public async Task<IActionResult> UpdateStatusWithStripe()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    endpointSecret,
                    throwOnApiVersionMismatch: false
                );

                int? orderId = null;
                string? newPaymentIntentId = null;

                // 1. معالجة حدث إتمام جلسة Stripe Checkout
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    if (session != null)
                    {
                        newPaymentIntentId = session.PaymentIntentId;

                        // استخراج orderId من Metadata الجلسة
                        if (session.Metadata != null && session.Metadata.TryGetValue("orderId", out var oIdStr))
                        {
                            if (int.TryParse(oIdStr, out int parsedId))
                                orderId = parsedId;
                        }
                    }
                }
                // 2. معالجة حدث نجاح الـ PaymentIntent المباشر
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    if (intent != null)
                    {
                        newPaymentIntentId = intent.Id;

                        // استخراج orderId من Metadata الـ PaymentIntent
                        if (intent.Metadata != null && intent.Metadata.TryGetValue("orderId", out var oIdStr))
                        {
                            if (int.TryParse(oIdStr, out int parsedId))
                                orderId = parsedId;
                        }
                    }
                }
                // 3. معالجة فشل الدفع
                else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    if (intent != null)
                    {
                        // استخراج orderId واستدعاء الدالة التي تقبل int فقط
                        if (intent.Metadata != null && intent.Metadata.TryGetValue("orderId", out var oIdStr) &&
                            int.TryParse(oIdStr, out int failedOrderId))
                        {
                            await _paymentService.UpdateOrderFaild(failedOrderId);
                        }

                        return Ok();
                    }
                }

                // 4. تنفيذ تحديث النجاح في قاعدة البيانات عند الحصول على orderId والـ PaymentIntentId الجديد
                if (orderId.HasValue && !string.IsNullOrEmpty(newPaymentIntentId))
                {
                    await _paymentService.UpdateOrderSuccess(orderId.Value, newPaymentIntentId);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine($"Webhook Error: {e.Message}");
                return BadRequest();
            }
        }
    }
}