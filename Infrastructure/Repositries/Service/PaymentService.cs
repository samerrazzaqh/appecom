using Core.Entities;
using Core.Entities.Order;
using Core.interfaces;
using Core.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Repositries.Service
{
    internal class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork work;
        private readonly IConfiguration configuration;
        private readonly AppDbContext _context;

        public PaymentService(IUnitOfWork work, IConfiguration configuration, AppDbContext context)
        {
            this.work = work;
            this.configuration = configuration;
            _context = context;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId, int? delivertMethodId)
        {
            CustomerBasket basket = await work.CustomerBasket.GetBasketAsync(basketId);
            if (basket == null) return null!;

            StripeConfiguration.ApiKey = configuration["StripSetting:secretKey"];

            decimal shippingPrice = 0m;
            if (delivertMethodId.HasValue)
            {
                var delivery = await _context.DeliveryMethods.AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == delivertMethodId.Value);
                if (delivery != null)
                    shippingPrice = delivery.Price;
            }

            foreach (var item in basket.basketItems)
            {
                var product = await work.productRepositry.GetByIdAsync(item.Id);
                if (product != null)
                    item.Price = product.NewPrice;
            }

            PaymentIntentService paymentIntentService = new();
            PaymentIntent _intent;

            // التحقق مما إذا كانت القيمة فارغة أو تحتوي على النص الافتراضي "string"
            bool isInvalidIntent = string.IsNullOrEmpty(basket.PaymentIntentId) ||
                                   basket.PaymentIntentId.Equals("string", StringComparison.OrdinalIgnoreCase);

            if (isInvalidIntent)
            {
                var option = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.basketItems.Sum(m => m.Qunatity * (m.Price * 100)) +
                             (long)(shippingPrice * 100),
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                _intent = await paymentIntentService.CreateAsync(option);

                // تعيين القيم الجديدة من Stripe
                basket.PaymentIntentId = _intent.Id;
                basket.ClientSecret = _intent.ClientSecret;
            }
            else
            {
                var option = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.basketItems.Sum(m => m.Qunatity * (m.Price * 100)) +
                             (long)(shippingPrice * 100),
                };

                _intent = await paymentIntentService.UpdateAsync(basket.PaymentIntentId, option);

                // إعادة تعيين التأكيد للإرجاع
                basket.ClientSecret = _intent.ClientSecret;
            }

            // حفظ السلة المحدثة في Redis / DB
            await work.CustomerBasket.UpdateBasketAsync(basket);

            return basket;
        }


        // public async Task<Orders> UpdateOrderFaild(string PaymentInten)
        // {
        //     var order = await _context.Orders.FirstOrDefaultAsync(m => m.PaymentIntentId == PaymentInten);
        //     if (order is null)
        //     {
        //         return null;
        //     }
        //
        //     order.status = Status.PaymentFaild;
        //     _context.Orders.Update(order);
        //     await _context.SaveChangesAsync();
        //     return order;
        // }
        //
        //
        // public async Task<Orders?> UpdateOrderSuccess(string paymentIntentId)
        // {
        //     var order = await _context.Orders
        //         .FirstOrDefaultAsync(m => m.PaymentIntentId == paymentIntentId);
        //
        //     if (order is null)
        //     {
        //         Console.WriteLine(
        //             $"Order with PaymentIntentId------------------------ {paymentIntentId} not found in DB.");
        //         return null;
        //     }
        //
        //     order.status = Status.PaymentReceived;
        //     _context.Orders.Update(order);
        //     await _context.SaveChangesAsync();
        //     return order;
        // }
        
        
        
        // دالة النجاح الجديدة التي تستقبل orderId والـ PaymentIntentId الجديد
        public async Task<Orders?> UpdateOrderSuccess(int orderId, string paymentIntentId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == orderId);

            if (order is null)
            {
                Console.WriteLine($"Order with Id {orderId} not found in DB.");
                return null;
            }

            // تحديث رقم PaymentIntent بالجديد القادم من Stripe وتغيير الحالة
            order.PaymentIntentId = paymentIntentId;
            order.status = Status.PaymentReceived;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

// دالة الفشل الجديدة التي تستقبل orderId
        public async Task<Orders?> UpdateOrderFaild(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == orderId);

            if (order is null)
            {
                Console.WriteLine($"----------------Order with Id {orderId} not found in DB.");
                return null;
            }

            order.status = Status.PaymentFaild;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
        
        
        
        
    }
}