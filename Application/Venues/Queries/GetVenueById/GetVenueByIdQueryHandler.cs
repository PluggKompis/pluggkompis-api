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

namespace Application.Venues.Queries.GetVenueById
{
    /// <summary>
    /// Handles fetching detailed venue information including timeslots and volunteers
    /// </summary>
    public class GetVenueByIdQueryHandler : IRequestHandler<GetVenueByIdQuery, OperationResult<VenueDetailDto>>
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;
        public GetVenueByIdQueryHandler(IVenueRepository venueRepository, IMapper mapper)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
        }
        public async Task<OperationResult<VenueDetailDto>> Handle(GetVenueByIdQuery request, CancellationToken cancellationToken)
        {
            var venue = await _venueRepository.GetByIdWithDetailsAsync(request.VenueId);

            if (venue == null)
            {
                return OperationResult<VenueDetailDto>.Failure("Venue not found.");
            }

            var dto = _mapper.Map<VenueDetailDto>(venue);
            return OperationResult<VenueDetailDto>.Success(dto);
        }
    }
}
