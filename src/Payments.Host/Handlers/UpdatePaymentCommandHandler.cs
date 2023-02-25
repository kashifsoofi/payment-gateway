﻿namespace Payments.Host.Handlers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NServiceBus;
    using Payments.Contracts.Messages.Commands;
    using Payments.Domain.Aggregates.Payment;

    public class UpdatePaymentCommandHandler : IHandleMessages<UpdatePayment>
    {
        private readonly IPaymentAggregateReadRepository aggregateReadRepository;
        private readonly IPaymentAggregateWriteRepository aggregateWriteRepository;

        public UpdatePaymentCommandHandler(IPaymentAggregateReadRepository aggregateReadRepository, IPaymentAggregateWriteRepository aggregateWriteRepository)
        {
            this.aggregateReadRepository = aggregateReadRepository;
            this.aggregateWriteRepository = aggregateWriteRepository;
        }

        public async Task Handle(UpdatePayment command, IMessageHandlerContext context)
        {
            if (command == null || command.Id == Guid.Empty)
            {
                throw new ArgumentException(nameof(command));
            }

            try
            {
                var aggregate = await this.aggregateReadRepository.GetByIdAsync(command.Id);
                aggregate.Update(command);

                await PersistAndPublishAsync(aggregate, context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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