using AutoMapper;

using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Domain.ValueObjects.Params.Security;
using Server.Domain.ValueObjects.Results.Security;

namespace Server.Application.Mapping.Core;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        // AuthRequest (DTO) → LoginCredentialsParams (Domain)
        CreateMap<AuthRequest, LoginCredentialsParams>()
            .ForMember(dest => dest.IpAddress, opt => opt.Ignore()) // Set by UseCase
            .ForMember(dest => dest.UserAgent, opt => opt.Ignore()); // Set by UseCase

        // AuthResult (Domain) → AuthResponseWithToken (DTO Wrapper)
        CreateMap<AuthResult, AuthResponseWithToken>()
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.AccessTokenExpiresAt))
            .ForMember(dest => dest.TokenType, opt => opt.MapFrom(src => "Bearer"))
            .ForMember(dest => dest.User,
                opt => opt.MapFrom(src => new AuthUserResponse
                {
                    Id = src.UserId,
                    UserName = src.UserName,
                    Email = src.Email,
                    Role = src.Role,
                    LastLogin = src.LastLogin.DateTime
                }));

        // AuthResponseWithToken (Wrapper) → AuthResponse (Public DTO)
        CreateMap<AuthResponseWithToken, AuthResponse>();
        // RefreshToken e RefreshTokenExpiresAt são ignorados (não vão para AuthResponse)
    }
}
