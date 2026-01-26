using Application.Children.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Children.Queries.GetChildById
{
    public class GetChildByIdQuery : IRequest<OperationResult<ChildDto>>
    {
        public Guid ChildId { get; }
        public Guid ParentId { get; }

        public GetChildByIdQuery(Guid childId, Guid parentId)
        {
            ChildId = childId;
            ParentId = parentId;
        }
    }
}
