using Application.Children.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Children.Queries.GetChildren
{
    public class GetChildrenQuery : IRequest<OperationResult<List<ChildDto>>>
    {
        public Guid ParentId { get; }

        public GetChildrenQuery(Guid parentId)
        {
            ParentId = parentId;
        }
    }
}
