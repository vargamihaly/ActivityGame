import {useMutation, useQuery, useQueryClient} from '@tanstack/react-query';
import {useToast} from "@/hooks/use-toast";
import {components} from "@/api/activitygame-schema";

import {AxiosError} from "axios";
import {
    getMeAsync,
    getCurrentGameAsync,
    postLogoutAsync,
    postUsernameAsync,
    getGameDetailsAsync,
    postCreateGameAsync,
    postEndTurnAsync,
    postStartGameAsync,
    putGameSettingsAsync,
    postJoinGameAsync,
    postLeaveLobbyAsync, getUserStatisticsAsync, getGlobalStatisticsAsync
} from "@/api/games-api";
import {useGame} from "@/context/GameContext";


const errorMessageTitle = "An error occurred";

type CreateGameResponseApiResponse = components["schemas"]["CreateGameResponseApiResponse"];
type UpdateGameSettingsRequest = components["schemas"]["UpdateGameSettingsRequest"];
type ApiResponse = components["schemas"]["ApiResponse"];
type StartGameResponseApiResponse = components["schemas"]["StartGameResponseApiResponse"];
type GetGameResponseApiResponse = components["schemas"]["GameResponseApiResponse"];
type GetGameResponse = components["schemas"]["GameResponse"];
type EndTurnResponseApiResponse = components["schemas"]["EndTurnResponseApiResponse"];
type EndTurnRequest = components["schemas"]["EndTurnRequest"];
type UserResponseApiResponse = components["schemas"]["UserResponseApiResponse"];
type UserResponse = components["schemas"]["UserResponse"];
type SetUsernameRequest = components["schemas"]["SetUsernameRequest"];
type GlobalStatisticsResponse = components["schemas"]["GetGlobalStatisticsResponse"];
type UserStatisticsResponse = components["schemas"]["GetUserStatisticsResponse"];


export const useCreateGame = () => {
    const queryClient = useQueryClient();
    const {toast} = useToast();

    return useMutation<CreateGameResponseApiResponse, AxiosError<ApiResponse>, void>({
        mutationFn: postCreateGameAsync,
        onSuccess: async (response) => {
            if (response.success) {
                const gameId = response.data?.gameId;
                if (gameId) {
                    await queryClient.invalidateQueries({
                        queryKey: ['games', gameId]
                    });
                }
                console.log('Game created successfully', response.data);
            }
        },
        onError: (error) => {
            console.error('Error creating game:', error);
            const errorMessage = error.response?.data?.message || error.message || "An unexpected error occurred. Please try again.";
            toast({
                title: "An Error Occurred",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });
};

export const useUpdateSettings = () => {
    const queryClient = useQueryClient();
    const {toast} = useToast();

    return useMutation<ApiResponse, AxiosError<ApiResponse>, { gameId: string; request: UpdateGameSettingsRequest }>({
        mutationFn: async ({gameId, request}) => putGameSettingsAsync(gameId, request),
        onSuccess: async (_, {gameId}) => {
            await queryClient.invalidateQueries({
                queryKey: ['games', gameId]
            });
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || error.message || "An unexpected error occurred.";
            toast({title: "Error", description: errorMessage, variant: "destructive"});
        }
    });
};

export const useLeaveLobby = () => {
    const queryClient = useQueryClient();
    const {toast} = useToast();

    return useMutation<ApiResponse, AxiosError<ApiResponse>, string>({
        mutationFn: postLeaveLobbyAsync,
        onSuccess: async (response, gameId) => {
            await queryClient.invalidateQueries({
                queryKey: ['games', gameId]
            });
            console.log('Left lobby successfully', response);
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || error.message || "An unexpected error occurred while leaving the lobby.";
            toast({
                title: "Error Leaving Lobby",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });
};

export const useJoinGame = () => {
    const queryClient = useQueryClient();
    const {toast} = useToast();

    return useMutation<ApiResponse, AxiosError<ApiResponse>, string>({
        mutationFn: postJoinGameAsync,
        onSuccess: async (response, gameId) => {
            await queryClient.invalidateQueries({
                queryKey: ['games', gameId]
            });
            console.log('Joined game successfully', response);
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || error.message || "An unexpected error occurred while joining the game.";
            toast({
                title: "Error Joining Game",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });
};

export const useLogout = () => {
    const queryClient = useQueryClient();
    const {toast} = useToast();

    return useMutation<ApiResponse, AxiosError<ApiResponse>, void>({
        mutationFn: postLogoutAsync,
        onSuccess: async () => {
            // Clear all queries when logging out
            await queryClient.clear();
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || error.message || "An unexpected error occurred while logging out.";
            toast({
                title: "Logout Error",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });
};

export const useGetMe = (options = {}) => {
    const {toast} = useToast();

    return useQuery<UserResponse, AxiosError<ApiResponse>>({
        queryKey: ['me'],
        queryFn: async (): Promise<UserResponse> => {

            const response = await getMeAsync();

            if (!response.data) {
                const errorMessage = response.message || "Failed to fetch user details.";
                toast({title: errorMessageTitle, description: errorMessage});
                throw new Error(errorMessage);
            }

            return response.data;
        },
        ...options,
    });
};

export const useSetUsername = () => {
    const queryClient = useQueryClient();
    const {toast} = useToast();

    return useMutation<UserResponseApiResponse, AxiosError<ApiResponse>, SetUsernameRequest>({
        mutationFn: postUsernameAsync,
        onSuccess: async () => {
            await queryClient.invalidateQueries({queryKey: ['me']});
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || error.message || "Failed to update username.";
            toast({
                title: "Error",
                description: errorMessage,
                variant: "destructive"
            });
        }
    });
};

export const useCurrentGame = (options = {}) => {
    const {toast} = useToast();
    const {currentGame, setCurrentGame, setIsInGame} = useGame();
    const currentGameId = currentGame?.id;
    return useQuery<GetGameResponse, AxiosError<ApiResponse>>({
        queryKey: ['currentGame'],
        queryFn: async (): Promise<GetGameResponse> => {
            if (!currentGameId) {
                throw new Error('Game ID is required');
            }

            const response = await getCurrentGameAsync();

            if (!response.data) {
                const errorMessage = response.message || "Game details not found";
                toast({title: errorMessageTitle, description: errorMessage});
                throw new Error(errorMessage);
            }

            return response.data;
        },
        ...options,
    });
};

//Should not be called directly for state manage the CURRENT game, use GameContext instead
export const useGameDetails = (gameId?: string, options = {}) => {
    const {toast} = useToast();

    return useQuery<GetGameResponse, AxiosError<ApiResponse>>({
        queryKey: ['games', gameId],
        queryFn: async (): Promise<GetGameResponse> => {
            if (!gameId) {
                throw new Error('Game ID is required');
            }

            const response = await getGameDetailsAsync(gameId);

            if (!response.data) {
                const errorMessage = response.message || "Game details not found";
                toast({title: errorMessageTitle, description: errorMessage});
                throw new Error(errorMessage);
            }

            return response.data;
        },
        ...options,
    });
};

export const useStartGame = () => {
    const queryClient = useQueryClient();
    const {toast} = useToast();

    return useMutation<StartGameResponseApiResponse, AxiosError<ApiResponse>, string>({
        mutationFn: postStartGameAsync,
        onSuccess: async (_, gameId) => {
            await queryClient.invalidateQueries({
                queryKey: ['games', gameId]
            });
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || error.message || "An unexpected error occurred.";
            toast({title: errorMessageTitle, description: errorMessage});
            throw error;
        }
    });
};


export const useEndTurn = () => {
    const queryClient = useQueryClient();
    const {toast} = useToast();

    return useMutation<EndTurnResponseApiResponse, AxiosError<ApiResponse>, {
        gameId: string;
        request: EndTurnRequest
    }>({
        mutationFn: async ({gameId, request}) => postEndTurnAsync(gameId, request),
        onSuccess: async (_, {gameId}) => {
            await queryClient.invalidateQueries({
                queryKey: ['games', gameId],
            });
        },
        onError: (error) => {
            const errorMessage = error.response?.data?.message || error.message || "An unexpected error occurred.";
            toast({title: errorMessageTitle, description: errorMessage});
            throw error;
        },
    });
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