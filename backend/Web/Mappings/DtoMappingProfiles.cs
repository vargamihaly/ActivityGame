using ActivityGameBackend.Application.Games;
using ActivityGameBackend.Application.Users;
using ActivityGameBackend.Web.Controllers.Game.Response;
using AutoMapper;

namespace ActivityGameBackend.Web.Mappings;

public sealed class DtoMappingProfiles : Profile
{
    public DtoMappingProfiles()
    {
        CreateMap<User, PlayerResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
            .ForMember(dest => dest.IsHost, opt => opt.MapFrom(src => src.IsHost));

        CreateMap<Round, RoundResponse>()
            .ForMember(dest => dest.RoundNumber, opt => opt.Ignore())
            .ForMember(dest => dest.MethodType, opt => opt.MapFrom(src => src.MethodType))
            .ForMember(dest => dest.Word, opt => opt.MapFrom(src => src.Word.Value))
            .ForMember(dest => dest.ActivePlayerUsername, opt => opt.MapFrom(src => src.ActivePlayer));

        CreateMap<Game, GameResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.HostId, opt => opt.MapFrom(src => src.Host.Id))
            .ForMember(dest => dest.TimerInMinutes, opt => opt.MapFrom(src => src.TimerInMinutes))
            .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.MaxScore))
            .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CurrentRound, opt =>
                opt.MapFrom(src => src.Rounds != null && src.Rounds.Any()
                    ? src.Rounds.Last()
                    : null))
            .ForMember(dest => dest.EnabledMethods, opt =>
                opt.MapFrom(src => src.EnabledMethods.ToList()));
    }
}
