using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proctos.Models;

namespace proctos.Views.Shared.Components.AverageRating
{
    public class AverageRatingViewComponent : ViewComponent
    {
        private readonly BrandsHopContext _context;

        public AverageRatingViewComponent(BrandsHopContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            return View(averageRating);
        }
    }
}
