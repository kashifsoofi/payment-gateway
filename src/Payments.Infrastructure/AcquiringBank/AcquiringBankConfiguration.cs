namespace Payments.Infrastructure.AcquiringBank
{
    public interface IAcquiringBankConfiguration
    {
        string BaseUrl { get; }
    }

    public class AcquiringBankConfiguration : IAcquiringBankConfiguration
    {
        public string BaseUrl { get; set; }
    }
}
