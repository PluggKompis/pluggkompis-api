using Application.Children.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Children;
using MediatR;

namespace Application.Children.Commands.CreateChild
{
    public class CreateChildCommandHandler : IRequestHandler<CreateChildCommand, OperationResult<ChildDto>>
    {
        private readonly IGenericRepository<Child> _childRepository;
        private readonly IMapper _mapper;

        public CreateChildCommandHandler(
            IGenericRepository<Child> childRepository,
            IMapper mapper)
        {
            _childRepository = childRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<ChildDto>> Handle(CreateChildCommand request, CancellationToken cancellationToken)
        {
            var child = _mapper.Map<Child>(request.Request);

            child.Id = Guid.NewGuid();
            child.ParentId = request.ParentId;

            await _childRepository.AddAsync(child);

            var dto = _mapper.Map<ChildDto>(child);

            return OperationResult<ChildDto>.Success(dto);
        }
    }
}
