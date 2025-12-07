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
    public class Restaurant : IItemValidating
    {
        //[Key]
        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string OwnerEmailAddress { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

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
