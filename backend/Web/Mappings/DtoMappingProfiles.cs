using ActivityGameBackend.Application.Chat;
using ActivityGameBackend.Application.Games;
using ActivityGameBackend.Application.Statistics;
using ActivityGameBackend.Application.Users;
using ActivityGameBackend.Web.Controllers.Game.Response;
using ActivityGameBackend.Web.Controllers.Sse.Response;
using ActivityGameBackend.Web.Controllers.Statistics.Response;
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
            .ForMember(dest => dest.ActivePlayerUsername, opt => opt.MapFrom(src => src.ActivePlayer.Username));

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

        CreateMap<UserStatistics, GetUserStatisticsResponse>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.GamesPlayed, opt => opt.MapFrom(src => src.GamesPlayed))
            .ForMember(dest => dest.GamesWon, opt => opt.MapFrom(src => src.GamesWon))
            .ForMember(dest => dest.GamesLost, opt => opt.MapFrom(src => src.GamesLost))
            .ForMember(dest => dest.WinRate, opt => opt.MapFrom(src => src.WinRate))
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => src.AverageScore));

        CreateMap<GlobalStatistics, GetGlobalStatisticsResponse>()
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => src.AverageScore))
            .ForMember(dest => dest.WinRate, opt => opt.MapFrom(src => src.WinRate))
            .ForMember(dest => dest.LossRate, opt => opt.MapFrom(src => src.LossRate))
            .ForMember(dest => dest.PlayerRankings, opt => opt.MapFrom(src => src.PlayerRankings));

        CreateMap<PlayerRanking, PlayerRankingResponse>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.TotalScore, opt => opt.MapFrom(src => src.TotalScore));

        CreateMap<ChatMessage, ChatMessageResponse>();
    }
}
