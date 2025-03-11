using System.ComponentModel.DataAnnotations;

namespace DigitalWarehouse.Models
{
    public class StockChangeModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductModel? Product { get; set; }
        [Required]
        public int ChangeAmount { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.Now;
    }
}