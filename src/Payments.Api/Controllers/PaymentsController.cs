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
    using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> Get([FromHeader(Name = "Merchant-Id")][Required] Guid merchantId)
        {
            if (merchantId == default)
            {
                return BadRequest("merchantId cannot be default value.");
            }

            var result = await this.getAllPaymentsQuery.ExecuteAsync(merchantId);
            return Ok(result);
        }

        // GET api/payments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromHeader(Name = "Merchant-Id")][Required] Guid merchantId, Guid id)
        {
            if (merchantId == default)
            {
                return BadRequest("merchantId cannot be default value.");
            }

            if (id == default)
            {
                return BadRequest("id cannot be defaul value.");
            }

            var result = await getPaymentByIdAndMerchantIdQuery.ExecuteAsync(id, merchantId);
            return result == null ? (ActionResult)NotFound() : Ok(result);
        }

        // POST api/payments
        [HttpPost]
        public async Task<IActionResult> Post([FromHeader(Name = "Merchant-Id")][Required] Guid merchantId, [FromBody] CreatePaymentRequest request)
        {
            if (merchantId == default)
            {
                return BadRequest("merchantId cannot be default value.");
            }

            var result = await getPaymentByIdQuery.ExecuteAsync(request.Id);
            if (result != null)
            {
                return Conflict();
            }

            var createPaymentCommand =
                new CreatePayment(
                    request.Id,
                    merchantId,
                    request.CardHolderName,
                    request.CardNumber,
                    request.ExpiryMonth,
                    request.ExpiryYear,
                    request.Cvv,
                    request.Amount,
                    request.CurrencyCode,
                    request.Reference);

            await messageSession.Send(createPaymentCommand);

            return Accepted();
        }
    }
}
