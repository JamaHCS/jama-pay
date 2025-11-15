using AutoMapper;
using Domain.DTO;
using Domain.DTO.Requests;
using Domain.DTO.Responses;
using Domain.Entities;

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


            CreateMap<ProductDTO, Product>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

            CreateMap<ProviderTaxDTO, Tax>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tax));
            CreateMap<Tax, ProviderTaxDTO>().ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.Name));
            CreateMap<Tax, TaxDTO>();
            CreateMap<TaxDTO, Tax>();
            CreateMap<Fee, FeeDTO>();
        }
    }
}
