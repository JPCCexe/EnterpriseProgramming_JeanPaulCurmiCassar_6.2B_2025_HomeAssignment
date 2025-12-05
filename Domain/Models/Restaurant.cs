using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Restaurant : IItemValidating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string OwnerEmailAddress { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Phone { get; set; }

        // Returns the list of users who can approve this restaurant
        public List<string> GetValidators()
        {

            return new List<string> { "adminjeanpaul@mcast.com" };
        }

        // Returns the partial view name used to display this restaurant
        public string GetCardPartial()
        {
            return "_CardItemRestaurant";
        }
    }
}
