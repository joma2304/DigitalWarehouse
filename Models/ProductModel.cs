using System.ComponentModel.DataAnnotations;

namespace DigitalWarehouse.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public int Price { get; set; }
  
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Amount cannot be less than zero.")]
        public int Amount { get; set; }

        public int CategoryId { get; set; }

        public CategoryModel? Category { get; set; }

        public ICollection<StockChangeModel> StockChanges { get; set; } = new List<StockChangeModel>();

    }
}
