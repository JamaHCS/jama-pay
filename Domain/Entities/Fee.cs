using Domain.Entities.Global;

namespace Domain.Entities
{
    public class Fee
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
