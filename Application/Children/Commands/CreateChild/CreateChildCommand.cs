using Application.Children.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Children.Commands.CreateChild
{
    public class CreateChildCommand : IRequest<OperationResult<ChildDto>>
    {
        public Guid ParentId { get; }
        public CreateChildRequest Request { get; }

        public CreateChildCommand(Guid parentId, CreateChildRequest request)
        {
            ParentId = parentId;
            Request = request;
        }
    }
}
