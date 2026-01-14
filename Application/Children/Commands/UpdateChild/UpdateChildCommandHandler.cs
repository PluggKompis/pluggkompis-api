using Application.Children.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Children;
using MediatR;

namespace Application.Children.Commands.UpdateChild
{
    public class UpdateChildCommandHandler : IRequestHandler<UpdateChildCommand, OperationResult<ChildDto>>
    {
        private readonly IGenericRepository<Child> _childRepository;
        private readonly IMapper _mapper;

        public UpdateChildCommandHandler(
            IGenericRepository<Child> childRepository,
            IMapper mapper)
        {
            _childRepository = childRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<ChildDto>> Handle(UpdateChildCommand request, CancellationToken cancellationToken)
        {
            var child = await _childRepository.GetByIdAsync(request.ChildId);

            if (child is null)
                return OperationResult<ChildDto>.Failure("Child not found.");

            // Ownership check
            if (child.ParentId != request.ParentId)
                return OperationResult<ChildDto>.Failure("Forbidden: you can only update your own child.");

            // Mapping update onto existing entity
            _mapper.Map(request.Request, child);

            await _childRepository.UpdateAsync(child);

            var dto = _mapper.Map<ChildDto>(child);

            return OperationResult<ChildDto>.Success(dto);
        }
    }
}
