using Ecommerce_api.Models;
using Ecommerce_api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public interface IProductService
{
    Task<IActionResult> CreateProductAsync(ProductViewModel viewModel, ClaimsPrincipal user, string storeId);
}

