using Boksi.Application.DTOs;
using Boksi.Application.ServiceCategories.Commands;
using Boksi.Application.ServiceCategories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceCategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServiceCategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ServiceCategoryDto>>> GetCategories()
        {
            var categories = await _mediator.Send(new GetServiceCategoriesQuery());
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCategory([FromBody] CreateServiceCategoryCommand command)
        {
            var categoryId = await _mediator.Send(command);
            return Ok(categoryId);
        }
    }
}
