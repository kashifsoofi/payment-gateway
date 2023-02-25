namespace Payments.Contracts.Responses
{
    public class Health
    {
        public Health(bool ok)
        {
            Ok = ok;
        }

        public bool Ok { get; }
    }
}
