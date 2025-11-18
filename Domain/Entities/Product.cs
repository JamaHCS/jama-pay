using Domain.Entities.Global;

namespace Domain.Entities
{
    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }
}
