using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DataAccess.Repositories;

namespace EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Filters
{
    public class FiltersValidations : IActionFilter
    {
        private readonly RestaurantsRepository _restaurantsRepo;
        private readonly MenuItemsRepository _menuItemsRepo;

        public FiltersValidations(RestaurantsRepository restaurantsRepo, MenuItemsRepository menuItemsRepo)
        {
            _restaurantsRepo = restaurantsRepo;
            _menuItemsRepo = menuItemsRepo;
        }

        //runs after the execution does into the controller's method
        public void OnActionExecuted(ActionExecutedContext context) { }

        //runs before the execution goes into the controller's method
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // checking if the user is logged in
            string userEmail = context.HttpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                //If not logged in it will give you an error 403
                context.Result = new ForbidResult();
                return;
            }

            //Get type of item are you approving
            //Either Restaurant or MenuItem
            string type = context.HttpContext.Request.Form["type"].ToString();
            bool isAuthorized = false;

            if (type == "menuitem")
            {
                //Checking if the Menu Item can be approved by the user
                var menuItemIds = context.HttpContext.Request.Form["menuItemIds"];

                if (menuItemIds.Count > 0)
                {
                    // Get first menu item ID and parse it
                    // Get the restaurant for this menu item
                    // after checking if the user is the owner
                    Guid itemId = Guid.Parse(menuItemIds[0]);
                    var menuItem = _menuItemsRepo.Get().FirstOrDefault(m => m.Id == itemId);                    
                    var restaurant = _restaurantsRepo.Get(menuItem.RestaurantId);
                    isAuthorized = (restaurant.OwnerEmailAddress == userEmail);
                }
            }
            else
            {
                //For restaurants, check if user is site admin
                var restaurantIds = context.HttpContext.Request.Form["restaurantIds"];

                if (restaurantIds.Count > 0)
                {
                    int restaurantId = int.Parse(restaurantIds[0]);
                    var restaurant = _restaurantsRepo.Get(restaurantId);

                    //checking fi the email is in the Method GetValidators()
                    isAuthorized = restaurant.GetValidators().Contains(userEmail);
                }
            }

            //error 403 forbidden
            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}