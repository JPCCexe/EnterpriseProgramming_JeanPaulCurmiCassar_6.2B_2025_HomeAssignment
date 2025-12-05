using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Models
{
    public class MenuItem : IItemValidating
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        [ForeignKey("Restaurant")]
        public int RestaurantId { get; set; }

        [Required]
        public string Status { get; set; }
        [Required]
        public string Currency { get; set; }

        public Restaurant Restaurant { get; set; }

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

        public string GetCardPartial()
        {
            return "_MenuItemCard";
        }
    }
}
