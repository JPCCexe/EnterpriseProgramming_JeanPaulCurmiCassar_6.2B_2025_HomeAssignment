using DataAccess.Context;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;


namespace DataAccess.Repositories
{
    //Class created to make use of the CRUD methods
    public class RestaurantsRepository
    {
        private RestaurantDbContext _context;

        public RestaurantsRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        //Get all restaurants
        public IQueryable<Restaurant> Get()
        {
            return _context.Restaurants;
        }

        // Get a restaurant using the id
        public Restaurant Get(int id)
        {
            return Get().SingleOrDefault(x => x.Id == id);
        }

        //adding a new restaurant to the database
        public void Add(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
        }

        //updateing the retaurant
        public void Update(Restaurant restaurant)
        {
            var original = Get(restaurant.Id);
            if (original != null)
            {
                original.Name = restaurant.Name;
                original.OwnerEmailAddress = restaurant.OwnerEmailAddress;
                original.Status = restaurant.Status;
                original.Phone = restaurant.Phone;
                _context.SaveChanges();
            }
        }

        //deleting a restaurant by id
        public void Delete(int id)
        {
            var original = Get(id);
            if (original != null)
            {
                _context.Restaurants.Remove(original);
                _context.SaveChanges();
            }
        }

        //used to approve multiple restaurants by id
        public void Approve(List<int> ids)
        {
            var restaurants = _context.Restaurants
                .Where(r => ids.Contains(r.Id))
                .ToList();

            foreach (var restaurant in restaurants)
            {
                restaurant.Status = "Approved";
            }

            _context.SaveChanges();
        }
    }
}
