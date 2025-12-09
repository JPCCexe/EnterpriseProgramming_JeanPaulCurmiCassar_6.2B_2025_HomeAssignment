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

        //loads the catalog page with approved items
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
            //Passes the data to the view using ViewBag
            ViewBag.ViewType = view;
            ViewBag.ItemType = type;

            //returns IEnumerable<IItemValidating>
            return View(items); 
        }

        // pending list, uses the same Catalog.cshtml but only pending items
        [HttpGet]
        public IActionResult Pending(string type, string view = "card")
        {
            IEnumerable<IItemValidating> items;

            if (type == "menuitem")
            {
                items = _menuItemsRepository.Get()
                    .Where(m => m.Status == "Pending")
                    .ToList();
            }
            else
            {
                items = _restaurantsRepository.Get()
                    .Where(r => r.Status == "Pending")
                    .ToList();
            }

            ViewBag.ViewType = view;
            ViewBag.ItemType = type;
            ViewBag.IsApprovalMode = true; 

            // aking use of the same catalog.cshtml
            return View("Catalog", items);    
        }


        //Used to approve selected items, multiple selection
        [HttpPost]
        public IActionResult Approve(string type, List<int> restaurantIds, List<Guid> menuItemIds)
        {
            if (type == "menuitem")
            {
                _menuItemsRepository.Approve(menuItemIds);
            }
            else
            {
                _restaurantsRepository.Approve(restaurantIds);
            }

            return RedirectToAction("Pending", new { type = type, view = "card" });
        }


        //Show approved restaurants from database with images
        [HttpGet]
        public IActionResult ApprovedRestaurants()
        {
            //Get approved restaurants from database
            var restaurants = _restaurantsRepository.Get()
                .Where(r => r.Status == "Approved")
                .ToList();

            ViewBag.ViewType = "card";
            ViewBag.ItemType = "restaurant";
            return View("Catalog", restaurants.Cast<IItemValidating>().ToList());
        }

        //Show menu items for a specific restaurant
        [HttpGet]
        public IActionResult RestaurantMenu(int id)
        {
            //Get approved menu items for this restaurant
            var menuItems = _menuItemsRepository.Get()
                .Where(m => m.RestaurantId == id && m.Status == "Approved")
                .ToList();

            //Get restaurant name for display
            var restaurant = _restaurantsRepository.Get(id);

            ViewBag.ViewType = "list";
            ViewBag.ItemType = "menuitem";
            ViewBag.RestaurantName = restaurant?.Name ?? "Restaurant";

            return View("Catalog", menuItems.Cast<IItemValidating>().ToList());
        }

    }
}