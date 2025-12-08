using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Factory;

namespace EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Controllers
{
    public class BulkImportController : Controller
    {
        [HttpGet]
        public IActionResult BulkImport()
        {
            //Show the upload form
            return View();
        }

        [HttpPost]
        public IActionResult BulkImport(string jsonData,
            [FromKeyedServices("memory")] IItemsRepository cacheRepository) //keyed service injection
        {
        
            //Using factory to create items from JSON
            ImportItemFactory itemFactory = new ImportItemFactory();
            List<IItemValidating> parsedItems = itemFactory.Create(jsonData);

            //Storing the in memory cache (NOT database yet!)
            cacheRepository.Save(parsedItems);

            //Passing the items to view for preview
            ViewBag.Message = $"Successfully parsed {parsedItems.Count} items and stored in memory";
            return View("Preview", parsedItems);
        }
    }
}