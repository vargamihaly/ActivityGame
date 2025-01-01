using ActivityGameBackend.Application.Chat;
using ActivityGameBackend.Application.Users;
using ActivityGameBackend.Persistence.Mssql.Chat;
using ActivityGameBackend.Persistence.Mssql.Games;
using AutoMapper;

public class EntityToDomainMappingProfile : Profile
{
    public EntityToDomainMappingProfile()
    {
        // Word Entity -> Domain
        CreateMap<WordEntity, Word>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method));

        // User Entity -> Domain
        CreateMap<UserEntity, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Score, opt => opt.Ignore());

        CreateMap<GamePlayerEntity, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.IsHost, opt => opt.MapFrom(src => src.IsHost))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));

        // Round Entity -> Domain
        CreateMap<RoundEntity, Round>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId))
            .ForMember(dest => dest.RoundWinnerId, opt => opt.MapFrom(src => src.RoundWinnerId))
            .ForMember(dest => dest.MethodType, opt => opt.MapFrom(src => src.MethodType))
            .ForMember(dest => dest.Word, opt => opt.MapFrom(src => src.Word))
            .ForMember(dest => dest.ActivePlayer, opt =>
                opt.MapFrom(src => src.ActivePlayer))
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(src => src.CreatedAtUtc));

        // Game Entity -> Domain
        CreateMap<GameEntity, Game>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Host, opt => opt.MapFrom(src => src.Host))
            .ForMember(dest => dest.TimerInMinutes, opt => opt.MapFrom(src => src.TimerInMinutes))
            .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.MaxScore))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.EnabledMethods, opt => opt.MapFrom(src => src.EnabledMethods))
            .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.GamePlayers))
            .ForMember(dest => dest.Winner, opt => opt.MapFrom(src => src.Winner))
            .ForMember(dest => dest.Rounds, opt => opt.MapFrom(src => src.Rounds));

        // ChatMessageEntity -> Domain
        CreateMap<ChatMessageEntity, ChatMessage>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp));

        // Reverse mappings (Domain -> Entity)
        CreateMap<Word, WordEntity>()
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore());

        CreateMap<User, UserEntity>()
            .ForMember(dest => dest.GamePlayers, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore());

        CreateMap<Round, RoundEntity>()
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.RoundWinner, opt => opt.Ignore())
            .ForMember(dest => dest.Word, opt => opt.Ignore())
            .ForMember(dest => dest.WordId, opt => opt.MapFrom(src => src.Word.Id))
            .ForMember(dest => dest.ActivePlayer, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore());

        CreateMap<Game, GameEntity>()
            .ForMember(dest => dest.Host, opt => opt.Ignore())
            .ForMember(dest => dest.Winner, opt => opt.Ignore())
            .ForMember(dest => dest.GamePlayers, opt => opt.Ignore())
            .ForMember(dest => dest.Rounds, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore());

        CreateMap<ChatMessage, ChatMessageEntity>()
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.Sender, opt => opt.Ignore())
            .ForMember(dest => dest.Timestamp, opt => opt.Ignore());

        CreateMap<ChatMessage, ChatMessageEntity>()
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.Sender, opt => opt.Ignore());
    }
}
