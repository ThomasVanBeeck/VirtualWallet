using System.Globalization;
using AutoMapper;
using VirtualWallet.Models;
using VirtualWallet.Dtos;

namespace VirtualWallet.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        CreateMap<Stock, StockDto>();

        CreateMap<Order, OrderDto>()
            .ForMember(
                dest => dest.StockName, 
                opt => opt.MapFrom(src => src.Holding.Stock.StockName)
            );
        
        CreateMap<Transfer, TransferSummaryDto>();
        CreateMap<TransferDto, Transfer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.WalletId, opt => opt.Ignore())
            .ForMember(dest => dest.Wallet, opt => opt.Ignore());
        
        CreateMap<OrderPostDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.HoldingId, opt => opt.Ignore())
            .ForMember(dest => dest.Holding, opt => opt.Ignore())
            .ForMember(dest => dest.WalletId, opt => opt.Ignore())
            .ForMember(dest => dest.Wallet, opt => opt.Ignore());

        CreateMap<Holding, HoldingSummaryDto>()
            .ForMember(dest => dest.Amount, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentPrice, opt => opt.Ignore())
            .ForMember(dest => dest.TotalValue, opt => opt.Ignore())
            .ForMember(dest => dest.TotalProfit, opt => opt.Ignore())
            .ForMember(dest => dest.WinLossPct, opt => opt.Ignore());
        
        CreateMap<Wallet,  WalletSummaryDto>()
            .ForMember(dest => dest.TransferPage, opt => opt.Ignore());
        
        CreateMap<UserRegisterDto, User>()
            .ForMember(
                dest => dest.PasswordHash, 
                opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password))
                    )
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid())
            );
    }
}