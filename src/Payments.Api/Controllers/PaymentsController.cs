namespace Payments.Api.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using NServiceBus;
    using System.Threading.Tasks;
    using Payments.Contracts.Messages.Commands;
    using Payments.Contracts.Requests;
    using Payments.Infrastructure.Messages.Responses;
    using Payments.Infrastructure.Queries;

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMessageSession messageSession;
        private readonly IGetPaymentsByMerchantIdQuery getAllPaymentsQuery;
        private readonly IGetPaymentByIdQuery getPaymentByIdQuery;
        private readonly IGetPaymentByIdAndMerchantIdQuery getPaymentByIdAndMerchantIdQuery;

        public PaymentsController(
            IMessageSession messageSession,
            IGetPaymentsByMerchantIdQuery getAllPaymentsQuery,
            IGetPaymentByIdQuery getPaymentByIdQuery,
            IGetPaymentByIdAndMerchantIdQuery getPaymentByIdAndMerchantIdQuery)
        {
            this.messageSession = messageSession;
            this.getAllPaymentsQuery = getAllPaymentsQuery;
            this.getPaymentByIdQuery = getPaymentByIdQuery;
            this.getPaymentByIdAndMerchantIdQuery = getPaymentByIdAndMerchantIdQuery;
        }

        // GET api/payments
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await this.getAllPaymentsQuery.ExecuteAsync(Guid.Empty);
            return Ok(result);
        }

        // GET api/payments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == default)
            {
                throw new ArgumentException("Guid value cannot be default", nameof(id));
            }

            var result = await getPaymentByIdAndMerchantIdQuery.ExecuteAsync(id, Guid.Empty);
            return result == null ? (ActionResult)NotFound() : Ok(result);
        }

        // POST api/payments
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePaymentRequest request)
        {
            var result = await getPaymentByIdQuery.ExecuteAsync(request.Id);
            if (result != null)
            {
                return Conflict();
            }

            var createPaymentCommand =
                new CreatePayment(
                    request.Id,
                    Guid.Empty,
                    request.CardHolderName,
                    request.CardNumber,
                    request.ExpiryMonth,
                    request.ExpiryYear,
                    request.Cvv,
                    request.Amount,
                    request.CurrencyCode,
                    request.Reference);

            var response = await messageSession.Request<RequestResponse>(createPaymentCommand);
            if (!response.Success)
            {
                throw response.Exception!;
            }

            return Accepted();
        }
    }
}
