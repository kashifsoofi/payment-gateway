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
    public class PaymentController : ControllerBase
    {
        private readonly IMessageSession messageSession;
        private readonly IGetAllPaymentsQuery getAllPaymentsQuery;
        private readonly IGetPaymentByIdQuery getPaymentByIdQuery;

        public PaymentController(IMessageSession messageSession, IGetAllPaymentsQuery getAllPaymentsQuery, IGetPaymentByIdQuery getPaymentByIdQuery)
        {
            this.messageSession = messageSession;
            this.getAllPaymentsQuery = getAllPaymentsQuery;
            this.getPaymentByIdQuery = getPaymentByIdQuery;
        }

        // GET api/payment
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await this.getAllPaymentsQuery.ExecuteAsync();
            return Ok(result);
        }

        // GET api/aggregatename/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == default)
            {
                throw new ArgumentException("Guid value cannot be default", nameof(id));
            }

            var result = await getPaymentByIdQuery.ExecuteAsync(id);
            return result == null ? (ActionResult)NotFound() : Ok(result);
        }

        // POST api/payment
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePaymentRequest request)
        {
            var createPaymentCommand =
                new CreatePayment(request.Id);

            var response = await this.messageSession.Request<RequestResponse>(createPaymentCommand);
            if (!response.Success)
            {
                throw response.Exception;
            }

            return Ok();
        }

        // PUT api/payment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] string value)
        {
            var updatePaymentCommand =
                new UpdatePayment(id);

            var response = await this.messageSession.Request<RequestResponse>(updatePaymentCommand);
            if (!response.Success)
            {
                throw response.Exception;
            }

            return NoContent();
        }

        // DELETE api/payment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletePaymentCommand = new DeletePayment(id);
            await this.messageSession.Send(deletePaymentCommand);

            return NoContent();
        }
    }
}
