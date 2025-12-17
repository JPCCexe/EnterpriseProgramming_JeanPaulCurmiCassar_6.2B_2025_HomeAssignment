using Domain.Interfaces;
using Domain.Models;
using System.Text.Json;

namespace EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Factory
{
    public class ImportItemFactory
    {
        // This method takes JSON and turns it into Restaurant or MenuItem objects
        public List<IItemValidating> Create(string json)
        {
            List<IItemValidating> items = new List<IItemValidating>();

            // Parsing JSON strings
            var jsonDoc = JsonDocument.Parse(json);
            var array = jsonDoc.RootElement;

            // Loopz through each item in the json array
            foreach (var item in array.EnumerateArray())
            {
                string type = item.GetProperty("type").GetString();

                if (type == "restaurant")
                {
                    // Create Restaurant from json data
                    Restaurant restOb = new Restaurant();
                    restOb.Name = item.GetProperty("name").GetString();
                    restOb.OwnerEmailAddress = item.GetProperty("ownerEmailAddress").GetString();
                    restOb.Phone = item.GetProperty("phone").GetString();
                    restOb.Address = item.GetProperty("address").GetString();
                    restOb.Description = item.GetProperty("description").GetString();
                    restOb.Status = "Pending";

                    items.Add(restOb);
                }
                else if (type == "menuItem")
                {
                    // Create MenuItem from json data
                    MenuItem menOb = new MenuItem();
                    menOb.Title = item.GetProperty("title").GetString();
                    menOb.Price = (float)item.GetProperty("price").GetDouble();
                    menOb.Currency = item.GetProperty("currency").GetString();
                    menOb.RestaurantId = item.GetProperty("restaurantId").GetInt32();
                    menOb.Status = "Pending";

                    items.Add(menOb);
                }
            }

            return items;
        }
    }
}
