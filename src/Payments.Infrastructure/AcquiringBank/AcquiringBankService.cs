namespace Payments.Infrastructure.AcquiringBank
{
    using Flurl;
    using Flurl.Http;
    using Payments.Contracts.Enums;
    using Payments.Domain.AcquiringBank;

    public class AcquiringBankService : IAcquiringBankService
    {
        private class Endpoints
        {
            public const string Payment = "payments";
        }

        private readonly AcquiringBankConfiguration configuration;

        public AcquiringBankService(AcquiringBankConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<PaymentStatus> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var response = await $"{configuration.BaseUrl}"
                .AppendPathSegment(Endpoints.Payment)
                .PostJsonAsync(request)
                .ReceiveJson<CreatePaymentResponse>();

            return response.Result == CreatePaymentResult.Success ? PaymentStatus.Approved : PaymentStatus.Declined;
        }
    }
}
