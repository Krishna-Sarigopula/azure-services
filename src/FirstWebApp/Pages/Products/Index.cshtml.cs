using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstWebApp.Data;
using Microsoft.FeatureManagement;

namespace FirstWebApp
{
    public class IndexModel : PageModel
    {
        private readonly FirstWebApp.Data.AppDbContext _context;
        private readonly IFeatureManager _featureManager;

        public IndexModel(AppDbContext context, IFeatureManager featureManager)
        {
            _context = context;
            _featureManager = featureManager;
        }

        public IList<Products> Products { get; set; } = default!;
        public bool IsFeatureEnable { get; set; }

        public async Task OnGetAsync()
        {
            IsFeatureEnable = _featureManager.IsEnabledAsync("Feature1").Result;
            if (_context.Products is not null)
            {
                Products = await _context.Products.ToListAsync();
            }
        }
    }
}
