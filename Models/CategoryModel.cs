using System.ComponentModel.DataAnnotations;

namespace DigitalWarehouse.Models;

public class CategoryModel{
public int Id { get; set; }
[Required]
public string? Name { get; set; }
}
