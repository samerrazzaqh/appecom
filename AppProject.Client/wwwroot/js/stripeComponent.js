let stripe, cardNumber, cardExpiry, cardCvc;

window.stripeInterop = {
    initStripe: function (publishableKey) {
        // تنظيف العناصر القديمة إن وجدت لمنع تجميد الإدخال
        if (cardNumber) cardNumber.destroy();
        if (cardExpiry) cardExpiry.destroy();
        if (cardCvc) cardCvc.destroy();

        stripe = Stripe(publishableKey);
        const elements = stripe.elements();

        // تنسيق الحقول لتتناسب مع الـ Dark Theme
        const style = {
            base: {
                color: "#ffffff",
                fontFamily: "Roboto, sans-serif",
                fontSize: "16px",
                "::placeholder": { color: "#aab7c4" }
            },
            invalid: { color: "#ff5252" }
        };

        cardNumber = elements.create('cardNumber', { style: style });
        cardExpiry = elements.create('cardExpiry', { style: style });
        cardCvc = elements.create('cardCvc', { style: style });

        // ربط العناصر بالحقول
        cardNumber.mount('#card-number-element');
        cardExpiry.mount('#card-expiry-element');
        cardCvc.mount('#card-cvc-element');
    },

    confirmPayment: async function (clientSecret, billingName) {
        const result = await stripe.confirmCardPayment(clientSecret, {
            payment_method: {
                card: cardNumber,
                billing_details: { name: billingName }
            }
        });

        if (result.error) {
            return { success: false, error: result.error.message };
        } else if (result.paymentIntent.status === 'succeeded') {
            return { success: true };
        }
        return { success: false, error: 'Payment failed' };
    }
};