import {useMutation, useQuery, useQueryClient} from '@tanstack/react-query';
import {useToast} from "@/hooks/use-toast";
import {components} from "@/api/activitygame-schema";
import {AxiosError} from "axios";
import {
    getGameDetailsAsync,
    postCreateGameAsync,
    postEndTurnAsync,
    postStartGameAsync,
    postJoinGameAsync,
    postLeaveLobbyAsync,
    getUserStatisticsAsync,
    getGlobalStatisticsAsync,
    postTimeUpAsync, updateGameSettingsAsync
} from "@/api/games-api";

const errorMessageTitle = "An error occurred";

type CreateGameResponseApiResponse = components["schemas"]["CreateGameResponseApiResponse"];
type UpdateGameSettingsRequest = components["schemas"]["UpdateGameSettingsRequest"];
type ApiResponse = components["schemas"]["ApiResponse"];
type StartGameResponseApiResponse = components["schemas"]["StartGameResponseApiResponse"];
type GetGameResponse = components["schemas"]["GameResponse"];
type EndTurnResponseApiResponse = components["schemas"]["EndTurnResponseApiResponse"];
type EndTurnRequest = components["schemas"]["EndTurnRequest"];
type GlobalStatisticsResponse = components["schemas"]["GetGlobalStatisticsResponse"];
type UserStatisticsResponse = components["schemas"]["GetUserStatisticsResponse"];


export const useGameHook = () => {
    const queryClient = useQueryClient();
    const { toast } = useToast();

    // Game Create Mutation
    const createGame = useMutation<CreateGameResponseApiResponse, AxiosError<ApiResponse>, void>({
        mutationFn: postCreateGameAsync,
        onSuccess: async (response) => {
            if (response.success && response.data?.gameId) {
                await queryClient.invalidateQueries({ queryKey: ['games', response.data.gameId] });
                console.log('Game created successfully', response.data);
            }
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || "Failed to create game";
            toast({
                title: "Game Creation Error",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });

    // Game Join Mutation
    const joinGame = useMutation<ApiResponse, AxiosError<ApiResponse>, string>({
        mutationFn: postJoinGameAsync,
        onSuccess: async (response, gameId) => {
            await queryClient.invalidateQueries({ queryKey: ['games', gameId] });
            console.log('Joined game successfully', response);
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || "Failed to join game";
            toast({
                title: "Game Join Error",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });

    // Leave Lobby Mutation
    const leaveLobby = useMutation<ApiResponse, AxiosError<ApiResponse>, string>({
        mutationFn: postLeaveLobbyAsync,
        onSuccess: async (response, gameId) => {
            await queryClient.invalidateQueries({ queryKey: ['games', gameId] });
            console.log('Left lobby successfully', response);
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || "Failed to leave lobby";
            toast({
                title: "Leave Lobby Error",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });

    // Start Game Mutation
    const startGame = useMutation<StartGameResponseApiResponse, AxiosError<ApiResponse>, string>({
        mutationFn: postStartGameAsync,
        onSuccess: async (_, gameId) => {
            await queryClient.invalidateQueries({ queryKey: ['games', gameId] });
            console.log('Game started successfully');
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || "Failed to start game";
            toast({
                title: "Game Start Error",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });

    // End Turn Mutation
    const endTurn = useMutation<EndTurnResponseApiResponse, AxiosError<ApiResponse>, {
        gameId: string;
        request: EndTurnRequest
    }>({
        mutationFn: async ({gameId, request}) => postEndTurnAsync(gameId, request),
        onSuccess: async (_, {gameId}) => {
            await queryClient.invalidateQueries({ queryKey: ['games', gameId] });
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || "Failed to end turn";
            toast({
                title: "End Turn Error",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });


    // Update Game Settings Mutation
    const updateSettings = useMutation<ApiResponse, AxiosError<ApiResponse>, {
        gameId: string;
        request: UpdateGameSettingsRequest
    }>({
        mutationFn: async ({gameId, request}) => updateGameSettingsAsync(gameId, request),
        onSuccess: async (_, {gameId}) => {
            await queryClient.invalidateQueries({ queryKey: ['games', gameId] });
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || "Failed to update settings";
            toast({
                title: "Update Settings Error",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });

    // Time Up Mutation
    const timeUp = useMutation<ApiResponse, AxiosError<ApiResponse>, string>({
        mutationFn: postTimeUpAsync,
        onSuccess: async (_, gameId) => {
            await queryClient.invalidateQueries({ queryKey: ['games', gameId] });
            console.log('Time up handled successfully');
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || "Failed to handle time up";
            toast({
                title: "Time Up Error",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });

    // Query hooks are kept as they were
    const useGameDetails = (gameId?: string, options = {}) => {
        return useQuery<GetGameResponse, AxiosError<ApiResponse>>({
            queryKey: ['games', gameId],
            queryFn: async (): Promise<GetGameResponse> => {
                if (!gameId) throw new Error('Game ID is required');
                const response = await getGameDetailsAsync(gameId);
                if (!response.data) throw new Error(response.message || "Game details not found");
                return response.data;
            },
            ...options,
        });
    };

    // Return all mutations and query functions
    return {
        // Mutations
        createGame,
        joinGame,
        leaveLobby,
        startGame,
        endTurn,
        updateSettings,
        timeUp,

        // Queries
        useGameDetails,
    };
};
    
    export const useGlobalStatistics = (options = {}) => {
    const { toast } = useToast();

    return useQuery<GlobalStatisticsResponse, AxiosError<ApiResponse>>({
        queryKey: ['globalStatistics'],
        queryFn: async (): Promise<GlobalStatisticsResponse> => {
            const response = await getGlobalStatisticsAsync();

            if (!response.data) {
                const errorMessage = response.message || "Failed to fetch global statistics";
                toast({ title: errorMessageTitle, description: errorMessage });
                throw new Error(errorMessage);
            }

            return response.data;
        },
        ...options,
    });
};

export const useUserStatistics = (options = {}) => {
    const { toast } = useToast();

    return useQuery<UserStatisticsResponse, AxiosError<ApiResponse>>({
        queryKey: ['userStatistics'],
        queryFn: async (): Promise<UserStatisticsResponse> => {
            const response = await getUserStatisticsAsync();

            if (!response.data) {
                const errorMessage = response.message || "Failed to fetch user statistics";
                toast({ title: errorMessageTitle, description: errorMessage });
                throw new Error(errorMessage);
            }

            return response.data;
        },
        ...options,
    });
};