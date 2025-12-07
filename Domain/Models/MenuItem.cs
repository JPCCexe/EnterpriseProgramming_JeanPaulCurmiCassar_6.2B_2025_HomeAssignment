using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MenuItem : IItemValidating
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public float Price { get; set; }

        [ForeignKey("RestaurantFK")]
        public int RestaurantId { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }

        public Restaurant Restaurant { get; set; }

        // Returns the list of email addresses allowed to approve this menu item
        public List<string> GetValidators()
        {
            if (Restaurant != null)
            {
                return new List<string> { Restaurant.OwnerEmailAddress };
            }
            else
            {
                return new List<string>();
            }
        }

        // Returns the partial view name used to display this menu item
        public string GetCardPartial()
        {
            return "_CardMenuItem";
        }
    }
}
