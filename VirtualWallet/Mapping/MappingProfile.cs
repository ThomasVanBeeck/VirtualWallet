using AutoMapper;
using VirtualWallet.Models;
using VirtualWallet.DTOs;

namespace VirtualWallet.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDTO>();
        CreateMap<UserDTO, User>();
        CreateMap<Stock, StockDTO>();
        CreateMap<Transfer, TransferSummaryDTO>();
        CreateMap<TransferDTO, Transfer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.WalletId, opt => opt.Ignore())
            .ForMember(dest => dest.Wallet, opt => opt.Ignore());
        
        CreateMap<OrderPostDTO, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.HoldingId, opt => opt.Ignore())
            .ForMember(dest => dest.Holding, opt => opt.Ignore());
        
        CreateMap<Wallet,  WalletSummaryDTO>()
            .ForMember(dest => dest.TransferPage, opt => opt.Ignore());
        
        CreateMap<UserRegisterDTO, User>()
            .ForMember(
                dest => dest.PasswordHash, 
                opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password))
            );
    }
}