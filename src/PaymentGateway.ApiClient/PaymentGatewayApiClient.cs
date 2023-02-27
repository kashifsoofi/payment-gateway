namespace PaymentGateway.ApiClient
{
    using PaymentGateway.ApiClient.Requests;
    using PaymentGateway.ApiClient.Responses;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class PaymentGatewayApiClient : IPaymentGatewayApiClient
    {
        private const string PaymentsEndpoint = "api/payments";

        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public PaymentGatewayApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public PaymentGatewayApiClient(string baseUrl)
        {
            this.httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl),
            };
        }

        public async Task<bool> CreatePaymentAsync(Guid merchantId, CreatePaymentRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, PaymentsEndpoint)
            {
                Content = JsonContent.Create(request),
            };
            httpRequest.Headers.Add("Merchant-Id", merchantId.ToString());

            var response = await httpClient.SendAsync(httpRequest);
            return response.StatusCode == HttpStatusCode.Accepted;
        }

        public async Task<PaymentResponse> GetPaymentByIdAsync(Guid merchantId, Guid paymentId)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{PaymentsEndpoint}/{paymentId}");
            httpRequest.Headers.Add("Merchant-Id", merchantId.ToString());

            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentResponse>(responseContent, jsonSerializerOptions)!;
        }

        public async Task<IEnumerable<PaymentResponse>> GetAllPaymentsAsync(Guid merchantId)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, PaymentsEndpoint);
            httpRequest.Headers.Add("Merchant-Id", merchantId.ToString());

            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PaymentResponse>>(responseContent, jsonSerializerOptions)!;
        }
    }
}