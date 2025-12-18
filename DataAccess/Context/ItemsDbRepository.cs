using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Context
{
    public class ItemsDbRepository : IItemsRepository
    {
        private readonly RestaurantDbContext _context;

        public ItemsDbRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        //Get all items from database
        public List<IItemValidating> Get()
        {
            List<IItemValidating> items = new List<IItemValidating>();

            //Get all restaurants
            items.AddRange(_context.Restaurants.ToList());

            //Get all menu items
            items.AddRange(_context.MenuItems.ToList());

            return items;
        }

        //Get approved restaurants only
        public List<Restaurant> GetApprovedRestaurants()
        {
            return _context.Restaurants.Where(r => r.Status == "Approved").ToList();
        }

        //Get approved menu items for a specific restaurant
        public List<MenuItem> GetApprovedMenuItems(int restaurantId)
        {
            return _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId && m.Status == "Approved")
                .ToList();
        }

        //Saving the items to the database
        public void Save(List<IItemValidating> items)
        {
            //First, save all restaurants and get their IDs
            var restaurants = items.OfType<Restaurant>().ToList();
            foreach (var restaurant in restaurants)
            {
                _context.Restaurants.Add(restaurant);
            }

            //SaveChanges assigns real IDs to restaurants
            _context.SaveChanges(); 

            //Now saving menu items
            var menuItems = items.OfType<MenuItem>().ToList();

            //For now, just set RestaurantId to first restaurant's ID for testing
            foreach (var menuItem in menuItems)
            {
                if (menuItem.RestaurantId == 0 && restaurants.Any())
                {
                    menuItem.RestaurantId = restaurants.First().Id;
                }
                _context.MenuItems.Add(menuItem);
            }
            _context.SaveChanges();
        }

    }
}
