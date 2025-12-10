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

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // checking if the use is logged in
            string userEmail = context.HttpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                context.Result = new ForbidResult();
                return;
            }

            //Get type from form
            string type = context.HttpContext.Request.Form["type"].ToString();

            bool isAuthorized = false;

            if (type == "menuitem")
            {
                //Get the menu item IDs being approved
                var menuItemIdsStr = context.HttpContext.Request.Form["menuItemIds"];

                if (menuItemIdsStr.Count > 0)
                {
                    //Get the first menu item being approved
                    var firstId = Guid.Parse(menuItemIdsStr[0]);
                    var menuItem = _menuItemsRepo.Get().FirstOrDefault(m => m.Id == firstId);

                    if (menuItem != null)
                    {
                        //Get restaurant directly using RestaurantId
                        var restaurant = _restaurantsRepo.Get(menuItem.RestaurantId);
                        if (restaurant != null)
                        {
                            isAuthorized = restaurant.OwnerEmailAddress == userEmail;
                        }
                    }
                }
            }

            else
            {
                //For restaurants, check if user is site admin
                var restaurantIdsStr = context.HttpContext.Request.Form["restaurantIds"];

                if (restaurantIdsStr.Count > 0)
                {
                    var firstId = int.Parse(restaurantIdsStr[0]);
                    var restaurant = _restaurantsRepo.Get(firstId);

                    if (restaurant != null)
                    {
                        isAuthorized = restaurant.GetValidators().Contains(userEmail);
                    }
                }
            }

            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
