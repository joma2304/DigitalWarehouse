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
        public decimal Price { get; set; }
        // public string? ImageUrl { get; set; } //Skulle kunna lägga till senare
        [Required]
        public int Amount { get; set; }

        //Koppling till kategorier
        public int CategoryId { get; set; }

        public CategoryModel? Category { get; set; }

        public ICollection<StockChangeModel> QuantityChanges { get; set; } = new List<StockChangeModel>();

    }
}
