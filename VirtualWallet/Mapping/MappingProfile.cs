using AutoMapper;
using VirtualWallet.Models;
using VirtualWallet.DTOs;

namespace VirtualWallet.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDTO>();
    }
}