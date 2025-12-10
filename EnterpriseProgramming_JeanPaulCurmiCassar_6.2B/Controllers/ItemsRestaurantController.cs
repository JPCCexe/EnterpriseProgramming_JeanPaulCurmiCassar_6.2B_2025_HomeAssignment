using DataAccess.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Filters; 

namespace Presentation.Controllers
{
    public class ItemsRestaurantController : Controller
    {
        //Contructor Injection is one of the variations of Dependency Injection
        private RestaurantsRepository _restaurantsRepository;
        private MenuItemsRepository _menuItemsRepository;

        public ItemsRestaurantController(RestaurantsRepository restaurantsRepository,
                                        MenuItemsRepository menuItemsRepository)
        {
            _restaurantsRepository = restaurantsRepository;
            _menuItemsRepository = menuItemsRepository;
        }

        // Show catalog - restaurants or menu items
        [HttpGet]
        public IActionResult Catalog(string type, string view)
        {
            IEnumerable<IItemValidating> items;

            if (type == "menuitem")
            {
                // Get all approved menu items
                items = _menuItemsRepository.Get()
                    .Where(m => m.Status == "Approved")
                    .ToList();
            }
            else
            {
                // Get all approved restaurants
                items = _restaurantsRepository.Get()
                    .Where(r => r.Status == "Approved")
                    .ToList();
            }

            ViewBag.ViewType = view;
            ViewBag.ItemType = type;

            return View(items);
        }


        // Show approved restaurants
        [HttpGet]
        public IActionResult ApprovedRestaurants()
        {
            var restaurants = _restaurantsRepository.Get()
                .Where(r => r.Status == "Approved")
                .ToList();

            ViewBag.ViewType = "card";
            ViewBag.ItemType = "restaurant";

            return View("Catalog", restaurants.Cast<IItemValidating>().ToList());
        }

        // Show menu items for a restaurant
        [HttpGet]
        public IActionResult RestaurantMenu(int id)
        {
            var menuItems = _menuItemsRepository.Get()
                .Where(m => m.RestaurantId == id && m.Status == "Approved")
                .ToList();

            var restaurant = _restaurantsRepository.Get(id);

            ViewBag.ViewType = "list";
            ViewBag.ItemType = "menuitem";
            ViewBag.RestaurantName = restaurant.Name;

            return View("Catalog", menuItems.Cast<IItemValidating>().ToList());
        }

        // Verification - shows pending items based on user role
        [HttpGet]
        public IActionResult Verification()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            string userEmail = User.Identity.Name;
            string adminEmail = "adminjeanpaul@mcast.com";

            if (userEmail == adminEmail)
            {
                // Admin sees pending restaurants
                var items = _restaurantsRepository.Get()
                    .Where(r => r.Status == "Pending")
                    .ToList();

                ViewBag.ViewType = "card";
                ViewBag.ItemType = "restaurant";
                ViewBag.IsApprovalMode = true;

                return View("Catalog", items.Cast<IItemValidating>().ToList());
            }
            else
            {
                // Owner sees pending menu items
                var items = _menuItemsRepository.Get()
                    .Where(m => m.Status == "Pending")
                    .ToList();

                ViewBag.ViewType = "list";
                ViewBag.ItemType = "menuitem";
                ViewBag.IsApprovalMode = true;

                return View("Catalog", items.Cast<IItemValidating>().ToList());
            }
        }

        // Approve selected items
        [HttpPost]
        [ServiceFilter(typeof(FiltersValidations))]
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

            return RedirectToAction("Verification");
        }
    }
}