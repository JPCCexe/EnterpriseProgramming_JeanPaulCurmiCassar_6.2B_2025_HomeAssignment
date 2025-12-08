using Domain.Interfaces;
using Domain.Models;
using Newtonsoft.Json.Linq;
using Humanizer;

namespace EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Factory
{
    public class ImportItemFactory
    {
        //Factory method to create items from JSON string
        public List<IItemValidating> Create(string json)
        {
            List<IItemValidating> items = new List<IItemValidating>();

            //Parse the JSON array
            JArray jsonArray = JArray.Parse(json);

            foreach (var jsonItem in jsonArray)
            {
                string type = jsonItem["type"]?.ToString();

                if (type == "restaurant")
                {
                    //Build Restaurant object
                    Restaurant restaurant = new Restaurant
                    {
                        Name = jsonItem["name"]?.ToString(),
                        OwnerEmailAddress = jsonItem["ownerEmailAddress"]?.ToString(),
                        Phone = jsonItem["phone"]?.ToString(),
                        Address = jsonItem["address"]?.ToString(),
                        Description = jsonItem["description"]?.ToString(),
                        Status = "Pending" //default status
                    };
                    items.Add(restaurant);
                }
                else if (type == "menuItem")
                {
                    //Build MenuItem object
                    MenuItem menuItem = new MenuItem
                    {
                        Title = jsonItem["title"]?.ToString(),
                        Price = jsonItem["price"]?.Value<float>() ?? 0,
                        Currency = jsonItem["currency"]?.ToString(),
                        RestaurantId = 0, //will be set later
                        Status = "Pending" //default status
                    };
                    items.Add(menuItem);
                }
            }

            return items;
        }
    }
}
