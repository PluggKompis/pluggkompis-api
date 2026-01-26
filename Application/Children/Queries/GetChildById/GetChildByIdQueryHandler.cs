using Application.Children.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Children;
using MediatR;

namespace Application.Children.Queries.GetChildById
{
    public class GetChildByIdQueryHandler : IRequestHandler<GetChildByIdQuery, OperationResult<ChildDto>>
    {
        private readonly IGenericRepository<Child> _childRepository;
        private readonly IMapper _mapper;

        public GetChildByIdQueryHandler(
            IGenericRepository<Child> childRepository,
            IMapper mapper)
        {
            _childRepository = childRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<ChildDto>> Handle(GetChildByIdQuery request, CancellationToken cancellationToken)
        {
            var child = await _childRepository.GetByIdAsync(request.ChildId);

            if (child is null)
                return OperationResult<ChildDto>.Failure("Child not found.");

            if (child.ParentId != request.ParentId)
                return OperationResult<ChildDto>.Failure("Forbidden: you can only view your own child.");

            var dto = _mapper.Map<ChildDto>(child);

            return OperationResult<ChildDto>.Success(dto);
        }
    }
}
