using Application.Common.Interfaces;
using Application.Venues.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Venues.Queries.GetVenues
{
    // <summary>
    /// Handles venue listing with filtering and pagination
    /// </summary>
    public class GetVenuesQueryHandler : IRequestHandler<GetVenuesQuery, OperationResult<PagedResult<VenueDto>>>
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;
        public GetVenuesQueryHandler(IVenueRepository venueRepository, IMapper mapper)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
        }
        public async Task<OperationResult<PagedResult<VenueDto>>> Handle(GetVenuesQuery request, CancellationToken cancellationToken)
        {
            var venues = await _venueRepository.GetAllAsync(request.Filters);
            var totalCount = await _venueRepository.GetCountAsync(request.Filters);

            var venueDtos = _mapper.Map<List<VenueDto>>(venues);

            var pagedResult = new PagedResult<VenueDto>
            {
                Items = venueDtos,
                TotalCount = totalCount,
                PageNumber = request.Filters.PageNumber,
                PageSize = request.Filters.PageSize
            };

            return OperationResult<PagedResult<VenueDto>>.Success(pagedResult);
        }
    }
}
