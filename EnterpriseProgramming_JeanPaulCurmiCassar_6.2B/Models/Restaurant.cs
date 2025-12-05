using System.ComponentModel.DataAnnotations;

namespace EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Models
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

        public List<string> GetValidators()
        {

            return new List<string> { "adminjeanpaul@mcast.com" };
        }

        public string GetCardPartial()
        {
            return "_CardRestaurant";
        }
    }

}
