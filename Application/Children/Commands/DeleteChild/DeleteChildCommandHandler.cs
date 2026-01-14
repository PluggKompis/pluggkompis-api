using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities.Bookings;
using Domain.Models.Entities.Children;
using MediatR;

namespace Application.Children.Commands.DeleteChild
{
    public class DeleteChildCommandHandler : IRequestHandler<DeleteChildCommand, OperationResult>
    {
        private readonly IGenericRepository<Child> _childRepository;
        private readonly IGenericRepository<Booking> _bookingRepository;

        public DeleteChildCommandHandler(
            IGenericRepository<Child> childRepository,
            IGenericRepository<Booking> bookingRepository)
        {
            _childRepository = childRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<OperationResult> Handle(DeleteChildCommand command, CancellationToken cancellationToken)
        {
            var child = await _childRepository.GetByIdAsync(command.ChildId);

            if (child is null)
                return OperationResult.Failure("Child not found.");

            if (child.ParentId != command.ParentId)
                return OperationResult.Failure("Forbidden: you can only delete your own child.");

            // Pre-check: any booking referencing this child?
            var bookings = await _bookingRepository.FindAsync(b => b.ChildId == command.ChildId);
            if (bookings.Any())
                return OperationResult.Failure("Cannot delete child because bookings exist.");

            await _childRepository.DeleteAsync(child);
            return OperationResult.Success();
        }
    }
}
