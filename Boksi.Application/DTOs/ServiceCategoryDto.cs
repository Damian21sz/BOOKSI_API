using System;

namespace Boksi.Application.DTOs
{
    public class ServiceCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
