using API.Extensions;
using Application.Children.Commands.CreateChild;
using Application.Children.Commands.DeleteChild;
using Application.Children.Commands.UpdateChild;
using Application.Children.Dtos;
using Application.Children.Queries.GetChildren;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChildrenController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChildrenController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Parent")]
        [HttpGet]
        public async Task<IActionResult> GetMyChildren(CancellationToken cancellationToken)
        {
            var parentId = User.GetUserId();

            var result = await _mediator.Send(new GetChildrenQuery(parentId), cancellationToken);

            return this.FromOperationResult(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetChildById(Guid id, CancellationToken cancellationToken)
        {
            var parentId = User.GetUserId();

            var result = await _mediator.Send(new Application.Children.Queries.GetChildById.GetChildByIdQuery(id, parentId), cancellationToken);

            return this.FromOperationResult(result);
        }

        [Authorize(Roles = "Parent")]
        [HttpPost]
        public async Task<IActionResult> CreateChild([FromBody] CreateChildRequest request, CancellationToken cancellationToken)
        {
            var parentId = User.GetUserId();

            var result = await _mediator.Send(new CreateChildCommand(parentId, request), cancellationToken);

            return this.FromOperationResult(result, created: true);
        }

        [Authorize(Roles = "Parent")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateChild(Guid id, [FromBody] UpdateChildRequest request, CancellationToken cancellationToken)
        {
            var parentId = User.GetUserId();

            var result = await _mediator.Send(new UpdateChildCommand(id, parentId, request), cancellationToken);

            return this.FromOperationResult(result);
        }

        [Authorize(Roles = "Parent")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteChild(Guid id, CancellationToken cancellationToken)
        {
            var parentId = User.GetUserId();

            var result = await _mediator.Send(new DeleteChildCommand(id, parentId), cancellationToken);

            return this.FromOperationResultNoContent(result);
        }
    }
}
