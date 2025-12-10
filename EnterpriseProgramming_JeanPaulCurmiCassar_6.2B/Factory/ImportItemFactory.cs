using Domain.Interfaces;
using Domain.Models;
using System.Text.Json;

namespace EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Factory
{
    public class ImportItemFactory
    {
        //Factory method to create items from json string
        public List<IItemValidating> Create(string json)
        {
            List<IItemValidating> items = new List<IItemValidating>();

            // Parse JSON to get array of objects
            var jsonDoc = JsonDocument.Parse(json);
            var array = jsonDoc.RootElement;

            // Loop through each item in JSON
            foreach (var item in array.EnumerateArray())
            {
                string type = item.GetProperty("type").GetString();

                if (type == "restaurant")
                {
                    // Create Restaurant
                    Restaurant r = new Restaurant();
                    r.Name = item.GetProperty("name").GetString();
                    r.OwnerEmailAddress = item.GetProperty("ownerEmailAddress").GetString();
                    r.Phone = item.GetProperty("phone").GetString();
                    r.Address = item.GetProperty("address").GetString();
                    r.Description = item.GetProperty("description").GetString();
                    r.Status = "Pending";

                    items.Add(r);
                }
                else if (type == "menuItem")
                {
                    // Create MenuItem
                    MenuItem m = new MenuItem();
                    m.Title = item.GetProperty("title").GetString();
                    m.Price = (float)item.GetProperty("price").GetDouble();
                    m.Currency = item.GetProperty("currency").GetString();
                    m.RestaurantId = item.GetProperty("restaurantId").GetInt32();
                    m.Status = "Pending";

                    items.Add(m);
                }
            }

            return items;
        }
    }
}
