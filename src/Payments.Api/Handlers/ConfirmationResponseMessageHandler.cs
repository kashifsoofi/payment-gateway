namespace Payments.Api.Handlers
{
    using System.Threading.Tasks;
    using NServiceBus;
    using Payments.Infrastructure.Messages.Responses;

    public class ConfirmationResponseMessageHandler : IHandleMessages<RequestResponse>
    {
        public Task Handle(RequestResponse message, IMessageHandlerContext context) => Task.CompletedTask;
    }
}