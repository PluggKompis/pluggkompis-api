using Application.Children.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Children;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Children.Queries.GetChildren
{
    public class GetChildrenQueryHandler : IRequestHandler<GetChildrenQuery, OperationResult<List<ChildDto>>>
    {
        private readonly IGenericRepository<Child> _childRepository;
        private readonly IMapper _mapper;

        public GetChildrenQueryHandler(
            IGenericRepository<Child> childRepository,
            IMapper mapper)
        {
            _childRepository = childRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<ChildDto>>> Handle(GetChildrenQuery request, CancellationToken cancellationToken)
        {
            var children = await _childRepository.FindAsync(c => c.ParentId == request.ParentId);

            var result = _mapper.Map<List<ChildDto>>(children);

            return OperationResult<List<ChildDto>>.Success(result);
        }
    }
}
