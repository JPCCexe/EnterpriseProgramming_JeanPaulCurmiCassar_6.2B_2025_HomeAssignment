using DataAccess.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class ItemsRestaurantController : Controller
    {
        //Contructor Injection is one of the variations of Dependency Injection
        private RestaurantsRepository _restaurantsRepository { get; set; }
        private MenuItemsRepository _menuItemsRepository { get; set; }

        public ItemsRestaurantController(RestaurantsRepository restaurantsRepository,
                                        MenuItemsRepository menuItemsRepository)
        {
            _restaurantsRepository = restaurantsRepository;
            _menuItemsRepository = menuItemsRepository;
        }

        //loads a page with empty input controls
        [HttpGet]
        public IActionResult Catalog(string type, string view)
        {
            //Get approved items based on type parameter
            IEnumerable<IItemValidating> items;

            if (type == "menuitem")
            {
                //Get approved menu items
                items = _menuItemsRepository.Get().Where(m => m.Status == "Approved").ToList();
            }
            else
            {
                //Get approved restaurants
                items = _restaurantsRepository.Get().Where(r => r.Status == "Approved").ToList();
            }

            //Pass data to the view using ViewBag
            ViewBag.ViewType = view;
            ViewBag.ItemType = type;

            return View(items); //returns IEnumerable<IItemValidating>
        }
    }
}