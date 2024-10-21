using AutoMapper;
using System.Runtime.InteropServices;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Oreder_Aggragate;
using Talabat.Core.Entities.Product;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
            .ForMember(P => P.Brand, O => O.MapFrom(s => s.Brand.Name))
            .ForMember(P => P.Category, O => O.MapFrom(s => s.Category.Name))
            .ForMember(P => P.PictureUrl, O => O.MapFrom<PictureUrlResolver>());

            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();

            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();

            CreateMap<AddressDto, Core.Entities.Oreder_Aggragate.Address>();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d=>d.DeliveryMethod, O=>O.MapFrom(S=>S.DeliveryMethod.ShortName))
                .ForMember(d=>d.DeliveryMethodCost, O=>O.MapFrom(S=>S.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d=>d.ProductId, O=>O.MapFrom(S=>S.Product.ProductId))
                .ForMember(d=>d.ProductName, O=>O.MapFrom(S=>S.Product.ProductName))
                .ForMember(d=>d.PictureUrl, O=>O.MapFrom(S=>S.Product.PictureUrl))
                .ForMember(d=>d.PictureUrl, O=>O.MapFrom<OrderItemPictureUrlResolver>());
        }
    }
}
