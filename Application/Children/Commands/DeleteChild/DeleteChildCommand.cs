using Domain.Models.Common;
using MediatR;

namespace Application.Children.Commands.DeleteChild
{
    public class DeleteChildCommand : IRequest<OperationResult>
    {
        public Guid ChildId { get; }
        public Guid ParentId { get; }

        public DeleteChildCommand(Guid childId, Guid parentId)
        {
            ChildId = childId;
            ParentId = parentId;
        }
    }
}
