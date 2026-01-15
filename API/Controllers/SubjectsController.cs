using API.Extensions;
using Application.Subjects.GetSubjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all available subjects (for dropdownlists etc.)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSubjects()
        {
            var query = new GetSubjectsQuery();
            var result = await _mediator.Send(query);

            return this.FromOperationResult(result);
        }
    }
}
