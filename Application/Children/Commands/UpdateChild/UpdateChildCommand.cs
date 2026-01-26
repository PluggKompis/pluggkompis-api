using Application.Children.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Children.Commands.UpdateChild
{
    public class UpdateChildCommand : IRequest<OperationResult<ChildDto>>
    {
        public Guid ChildId { get; }
        public Guid ParentId { get; }
        public UpdateChildRequest Request { get; }

        public UpdateChildCommand(Guid childId, Guid parentId, UpdateChildRequest request)
        {
            ChildId = childId;
            ParentId = parentId;
            Request = request;
        }
    }
}
