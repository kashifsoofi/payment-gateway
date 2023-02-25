namespace Payments.Host.Handlers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NServiceBus;
    using Payments.Contracts.Messages.Commands;
    using Payments.Domain.Aggregates.Payment;
    using Payments.Infrastructure.Messages.Responses;

    public class CreatePaymentCommandHandler : IHandleMessages<CreatePayment>
    {
        private readonly IPaymentAggregateReadRepository aggregateReadRepository;
        private readonly IPaymentAggregateWriteRepository aggregateWriteRepository;

        public CreatePaymentCommandHandler(IPaymentAggregateReadRepository aggregateReadRepository, IPaymentAggregateWriteRepository aggregateWriteRepository)
        {
            this.aggregateReadRepository = aggregateReadRepository;
            this.aggregateWriteRepository = aggregateWriteRepository;
        }

        public async Task Handle(CreatePayment command, IMessageHandlerContext context)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.Id == Guid.Empty)
            {
                throw new ArgumentException("Guid value cannot be default", nameof(command.Id));
            }

            try
            {
                var aggregate = await this.aggregateReadRepository.GetByIdAsync(command.Id);
                aggregate.Create(command);

                await PersistAndPublishAsync(aggregate, context);
                await context.Reply(new RequestResponse(true)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await context.Reply(new RequestResponse(e)).ConfigureAwait(false);
            }
        }

        private async Task PersistAndPublishAsync(IPaymentAggregate aggregate, IMessageHandlerContext context)
        {
            if (aggregate.IsNew)
            {
                await aggregateWriteRepository.CreateAsync(aggregate);
            }
            else
            {
                await aggregateWriteRepository.UpdateAsync(aggregate);
            }

            await Task.WhenAll(aggregate.UncommittedEvents.Select(async (x) => await context.Publish(x)));
        }
    }
}