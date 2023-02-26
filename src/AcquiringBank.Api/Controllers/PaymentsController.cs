namespace AcquiringBank.Api.Controllers
{
    using AcquiringBank.Api.Requests;
    using AcquiringBank.Api.Responses;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly List<string> validCardNumbers = new List<string>
        {
            "4242424242424242",
            "4273149019799094",
            "4539467987109256",
            "4024007181869214",
            "4916301720257093",
        };

        private readonly List<string> invalidExpiryCardNumbers = new List<string>
        {
            "4659959652550685",
            "4446900535698356",
        };

        private readonly List<string> insufficiantFundsCardNumbers = new List<string>
        {
            "4140253846048187",
            "4556447238607884",
        };

        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(ILogger<PaymentsController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "CreatePayment")]
        public IActionResult Post([FromBody] CreatePaymentRequest request)
        {
            var response = new CreatePaymentResponse();
            if (validCardNumbers.Contains(request.CardNumber))
            {
                response.Result = CreatePaymentResult.Success;
            }
            else if (invalidExpiryCardNumbers.Contains(request.CardNumber))
            {
                response.Result = CreatePaymentResult.InvalidExpiry;
            }
            else if (insufficiantFundsCardNumbers.Contains(request.CardNumber))
            {
                response.Result = CreatePaymentResult.InsufficientFunds;
            }
            else
            {
                response.Result = CreatePaymentResult.InvalidCardNumber;
            }
            return Ok(response);
        }
    }
}