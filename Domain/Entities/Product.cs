using Domain.Entities.Global;

namespace Domain.Entities
{
    public class Product : EntityBase
    {
        public int OrderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public Order Order { get; set; } = null!;
    }
}
