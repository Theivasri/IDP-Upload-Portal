using Microsoft.AspNetCore.Mvc;
using IDPUpload_Portal.Services;
using IDPUpload_Portal.Models;

namespace IDPUpload_Portal.Controllers
{
public class HomeController : Controller
{
private readonly CosmosMongoService _cosmosService;

public HomeController(CosmosMongoService cosmosService)  
    {  
        _cosmosService = cosmosService;  
    }  

    public async Task<IActionResult> Index()  
    {  
        // Fetch categories from DB  
        List<Category> categories = await _cosmosService.GetCategoriesAsync();  

        // Ensure Model is not null  
        if (categories == null)  
            categories = new List<Category>();  

        return View(categories);  
    }  
}

}
