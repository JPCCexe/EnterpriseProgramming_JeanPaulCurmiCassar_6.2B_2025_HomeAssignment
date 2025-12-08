using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //Saving the items to the database
        public void Save(List<IItemValidating> items)
        {
            foreach (var item in items)
            {
                if (item is Restaurant restaurant)
                {
                    _context.Restaurants.Add(restaurant);
                }
                else if (item is MenuItem menuItem)
                {
                    _context.MenuItems.Add(menuItem);
                }
            }
            _context.SaveChanges();
        }

    }
}
