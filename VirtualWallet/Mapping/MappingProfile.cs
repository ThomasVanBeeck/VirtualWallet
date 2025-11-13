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
        CreateMap<UserRegisterDTO, User>()
            .ForMember(
                dest => dest.PasswordHash, 
                opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password))
            );
    }
}