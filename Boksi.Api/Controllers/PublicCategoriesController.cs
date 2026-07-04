using Boksi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/public/categories")]
    public class PublicCategoriesController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;

        public PublicCategoriesController(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _dbContext.BusinessCategories.ToListAsync();
            return Ok(categories);
        }
    }
}
