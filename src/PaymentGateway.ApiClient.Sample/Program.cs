using Microsoft.Extensions.Configuration;
using PaymentGateway.ApiClient;
using PaymentGateway.ApiClient.Requests;
using PaymentGateway.ApiClient.Responses;
using PaymentGateway.ApiClient.Sample;
using System.Text.Json;
using System.Text.Json.Serialization;

IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", true, true)
              .AddEnvironmentVariables()
              .Build();

var paymentGatewayConfiguration = configuration.GetSection("PaymentGateway").Get<PaymentGatewayConfiguration>();
var httpClient = new HttpClient()
{
    BaseAddress = new Uri(paymentGatewayConfiguration.BaseUrl),
};

var paymentGatewayApiClient = new PaymentGatewayApiClient(httpClient);
// Line 11-16 can be replaced as follows
// var paymentGatewayApiClient = new PaymentGatewayApiClient(paymentGatewayConfiguration.BaseUrl)

var merchantId = Guid.NewGuid(); // Get it from PaymentGateway and set using config
Console.WriteLine($"MerchantId: {merchantId}");

(var result, var paymentId1) = await CreatePaymentAsync(paymentGatewayApiClient, merchantId, "4242424242424242", "REF001");
Console.WriteLine($"Create Payment result: {result}, PaymentId: {paymentId1}");

(result, var paymentId2) = await CreatePaymentAsync(paymentGatewayApiClient, merchantId, "4242424242424242", "REF002");
Console.WriteLine($"Create Payment result: {result}, PaymentId: {paymentId2}");

var jsonSerializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    
};
jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

var payment = await GetPayment(paymentGatewayApiClient, merchantId, paymentId1);
Console.WriteLine($"Payment details: {JsonSerializer.Serialize(payment, jsonSerializerOptions)}");

var payments = await paymentGatewayApiClient.GetAllPaymentsAsync(merchantId);
Console.WriteLine($"Payments: {JsonSerializer.Serialize(payments, jsonSerializerOptions)}");

static async Task<PaymentResponse> GetPayment(
    PaymentGatewayApiClient paymentGatewayApiClient,
    Guid merchantId, Guid paymentId)
{
    var maxRetries = 3;
    for (var i = 0; i < maxRetries; i++)
    {
        try
        {
            return await paymentGatewayApiClient.GetPaymentByIdAsync(merchantId, paymentId);
        }
        catch (Exception)
        { }
    }

    return null;
}

static async Task<(bool, Guid)> CreatePaymentAsync(
    PaymentGatewayApiClient paymentGatewayApiClient,
    Guid merchantId,
    string cardNumber,
    string reference)
{
    var paymentId = Guid.NewGuid();
    var createPaymentRequest = new CreatePaymentRequest
    {
        Id = paymentId,
        CardHolderName = "Chuck Norris",
        CardNumber = cardNumber,
        ExpiryMonth = 12,
        ExpiryYear = 2023,
        Cvv = "123",
        Amount = 100,
        CurrencyCode = "GBP",
        Reference = reference,
    };

    var result = await paymentGatewayApiClient.CreatePaymentAsync(merchantId, createPaymentRequest);
    return (result, paymentId);
}