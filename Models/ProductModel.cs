namespace DigitalWarehouse.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
       // public string? ImageUrl { get; set; } //Skulle kunna lägga till senare
        public int Amount { get; set; }

        //Koppling till kategorier
        public int CategoryId { get; set; }

        public CategoryModel? Category { get; set; }
    }
}
