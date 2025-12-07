using DataAccess.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    //Class created to make use of the crud methods
    public class MenuItemsRepository
    {
        private RestaurantDbContext _context;

        public MenuItemsRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        //getting all the menu itmems
        public IQueryable<MenuItem> Get()
        {
            return _context.MenuItems.Include(m => m.Restaurant);
        }

        // getting a menu item using the ID
        public MenuItem Get(Guid id)
        {
            return Get().SingleOrDefault(x => x.Id == id);
        }

        //adding a new menu item to the database
        public void Add(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            _context.SaveChanges();
        }

        //updating an existing menu item
        public void Update(MenuItem menuItem)
        {
            var original = Get(menuItem.Id);
            if (original != null)
            {
                original.Title = menuItem.Title;
                original.Price = menuItem.Price;
                original.RestaurantId = menuItem.RestaurantId;
                original.Status = menuItem.Status;
                original.Currency = menuItem.Currency;
                _context.SaveChanges();
            }
        }

        //deleting a menu itme using an ID
        public void Delete(Guid id)
        {
            var original = Get(id);
            if (original != null)
            {
                _context.MenuItems.Remove(original);
                _context.SaveChanges();
            }
        }

        //Used to approve multiple MenuItmes by ID
        public void Approve(List<Guid> ids)
        {
            var menuItems = _context.MenuItems
                .Where(m => ids.Contains(m.Id))
                .ToList();

            foreach (var menuItem in menuItems)
            {
                menuItem.Status = "Approved";
            }

            _context.SaveChanges();

        }
    }
}

