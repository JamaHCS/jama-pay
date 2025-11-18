using AutoMapper;
using Domain.DTO;
using Domain.DTO.Requests;
using Domain.DTO.Responses;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateOrderRequestDTO, Order>()
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products))
                .ForMember(dest => dest.Amount, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProviderOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.ProviderName, opt => opt.Ignore())
                .ForMember(dest => dest.Fees, opt => opt.Ignore())
                .ForMember(dest => dest.Taxes, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Order, OrderResponseDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderName))
               .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method))
               .ForMember(dest => dest.Fees, opt => opt.MapFrom(src => src.Fees))
               .ForMember(dest => dest.Taxes, opt => opt.MapFrom(src => src.Taxes))
               .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products))
               .ForMember(dest => dest.GrandTotal, opt => opt.MapFrom(src => src.Amount + src.GetTotalFees() + src.Taxes.Sum(t => t.Amount)));

            CreateMap<Order, OrderDetailsResponseDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.ProviderOrderId, opt => opt.MapFrom(src => src.ProviderOrderId.HasValue ? src.ProviderOrderId.Value.ToString() : null))
               .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderName))
               .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method))
               .ForMember(dest => dest.Fees, opt => opt.MapFrom(src => src.Fees))
               .ForMember(dest => dest.Taxes, opt => opt.MapFrom(src => src.Taxes))
               .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products))
               .ForMember(dest => dest.TotalFees, opt => opt.MapFrom(src => src.GetTotalFees()))
               .ForMember(dest => dest.TotalTaxes, opt => opt.MapFrom(src => src.Taxes.Sum(t => t.Amount)))
               .ForMember(dest => dest.TotalCharges, opt => opt.MapFrom(src => src.GetTotalFees() + src.Taxes.Sum(t => t.Amount)))
               .ForMember(dest => dest.GrandTotal, opt => opt.MapFrom(src => src.Amount + src.GetTotalFees() + src.Taxes.Sum(t => t.Amount)))
               .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));


            CreateMap<ProviderOrderResponseDTO, Order>()
                .ForMember(dest => dest.ProviderOrderId, opt => opt.MapFrom(src => Guid.Parse(src.OrderId)))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapProviderStatusToEnum(src.Status)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => MapProviderMethodToEnum(src.Method)))
                .ForMember(dest => dest.Fees, opt => opt.MapFrom(src => src.Fees))
                .ForMember(dest => dest.Taxes, opt => opt.MapFrom(src => src.Taxes ?? new List<ProviderTaxDTO>()))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProviderName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


            CreateMap<ProductDTO, Product>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

            CreateMap<Tax, ProviderTaxDTO>().ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.Name));
            CreateMap<Tax, TaxDTO>();
            CreateMap<TaxDTO, Tax>();
            CreateMap<Fee, FeeDTO>();

            CreateMap<ProviderFeeDTO, Fee>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetName()))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<ProviderTaxDTO, Tax>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tax ?? "Tax"))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<CreateOrderRequestDTO, ProviderCreateOrderRequestDTO>()
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => MapPaymentModeToProviderString(src.Method)))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        }

        private static OrderStatus MapProviderStatusToEnum(string status)
        {
            return status?.ToLower() switch
            {
                "pending" => OrderStatus.Pending,
                "paid" => OrderStatus.Paid,
                "cancelled" => OrderStatus.Cancelled,
                _ => OrderStatus.None
            };
        }
        private static PaymentMethod MapProviderMethodToEnum(string method)
        {
            return method?.ToLower() switch
            {
                "cash" => PaymentMethod.Cash,
                "card" or "creditcard" => PaymentMethod.Card,
                "transfer" => PaymentMethod.Transfer,
                _ => PaymentMethod.None
            };
        }
        private static string MapPaymentModeToProviderString(PaymentMethod mode, string providerName = "")
        {
            return mode switch
            {
                PaymentMethod.Cash => "Cash",
                PaymentMethod.Card => "Card",
                PaymentMethod.Transfer => "Transfer",
                _ => "None"
            };
        }
    }
}
