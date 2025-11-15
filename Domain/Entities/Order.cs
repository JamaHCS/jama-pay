using Domain.Entities.Global;
using Domain.Enums;

namespace Domain.Entities
{
    public class Order : EntityBase
    {
        public Guid? ProviderOrderId { get; set; } = Guid.NewGuid();
        public string ProviderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.None;
        public PaymentMethod Method { get; set; } = PaymentMethod.None;
        public List<Fee> Fees { get; set; } = new();
        public List<Tax> Taxes { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public string? CreatedBy { get; set; }
        public decimal GetTotalFees() => Fees.Sum(f => f.Amount);
        public decimal GetTotalWithFees() => Amount + GetTotalFees();
    }
}
