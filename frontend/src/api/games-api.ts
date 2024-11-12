import axios, { AxiosResponse, AxiosRequestConfig } from 'axios';
import { components } from "@/api/activitygame-schema";
import { CONFIG } from "@/config/config";

// Define types
type CreateGameResponseApiResponse = components['schemas']['CreateGameResponseApiResponse'];
type ApiResponse = components['schemas']['ApiResponse'];
type StartGameResponseApiResponse = components['schemas']['StartGameResponseApiResponse'];
type GetGameResponseApiResponse = components['schemas']['GameResponseApiResponse'];
type EndTurnResponseApiResponse = components['schemas']['EndTurnResponseApiResponse'];
type EndTurnRequest = components['schemas']['EndTurnRequest'];
type UpdateGameSettingsRequest = components['schemas']['UpdateGameSettingsRequest'];
type SetUsernameRequest = components['schemas']['SetUsernameRequest'];
type UserResponseApiResponse = components['schemas']['UserResponseApiResponse'];
type GlobalStatisticsResponseApiResponse = components['schemas']['GetGlobalStatisticsResponseApiResponse'];
type UserStatisticsResponseApiResponse = components['schemas']['GetUserStatisticsResponseApiResponse'];

let authToken: string | null = null;

const axiosClient = axios.create({
    baseURL: CONFIG.activityGameApi.endpoint,
    timeout: CONFIG.activityGameApi.timeout,
    withCredentials: CONFIG.activityGameApi.withCredentials,
});

axiosClient.interceptors.request.use((config) => {
    if (authToken) {
        config.headers['Authorization'] = `Bearer ${authToken}`;
    }
    return config;
});

export const setAuthToken = (token: string | null) => {
    authToken = token;
};

const API_PATHS = {
    register: '/Auth/register',
    me: '/Auth/me',
    logout: '/Auth/logout',
    username: '/Auth/username',
    gameEvents: '/GameEvents',
    gameDetails: '/games/details',
    currentGame: '/games/current',
    createGame: '/games/create',
    joinGame: '/games/join',
    startGame: '/games/start',
    endTurn: '/games/end-turn',
    gameSettings: '/games/settings',
    leaveLobby: 'games/leave-lobby',
    globalStatistics: '/statistics/statistics/global',
    userStatistics: '/statistics/statistics/user',
} as const;

const apiCall = async <T>(method: 'get' | 'post' | 'put', url: string, data?: any): Promise<AxiosResponse<T>> => {
    const config: AxiosRequestConfig = { method, url };
    if (data) {
        config.data = data;
    }
    return await axiosClient(config);
};

// Auth endpoints
export const postRegisterAsync = async (): Promise<UserResponseApiResponse> => {
    const response = await apiCall<UserResponseApiResponse>('post', API_PATHS.register);
    return response.data;
};

export const getMeAsync = async (): Promise<UserResponseApiResponse> => {
    const response = await apiCall<UserResponseApiResponse>('get', API_PATHS.me);
    return response.data;
};

export const postLogoutAsync = async (): Promise<ApiResponse> => {
    const response = await apiCall<ApiResponse>('post', API_PATHS.logout);
    return response.data;
};

//not used atm
export const postUsernameAsync = async (request: SetUsernameRequest): Promise<UserResponseApiResponse> => {
    const response = await apiCall<UserResponseApiResponse>('post', API_PATHS.username, request);
    return response.data;
};

// Game events endpoint
export const connectToGameEventStreamAsync = async (gameId: string): Promise<EventSource> => {
    const response = await apiCall<void>('get', `${API_PATHS.gameEvents}/${gameId}`);

    if (response.status !== 200) {
        throw new Error(`Failed to connect to game event stream. Status: ${response.status}`);
    }

    const eventSource = new EventSource(`${CONFIG.activityGameApi.endpoint}${API_PATHS.gameEvents}/${gameId}`);
    return eventSource;
};

// Game endpoints
export const getGameDetailsAsync = async (gameId: string): Promise<GetGameResponseApiResponse> => {
    const response = await apiCall<GetGameResponseApiResponse>('get', `${API_PATHS.gameDetails}/${gameId}`);
    return response.data;
};

export const getCurrentGameAsync = async (): Promise<GetGameResponseApiResponse> => {
    const response = await apiCall<GetGameResponseApiResponse>('get', `${API_PATHS.currentGame}`);
    return response.data;
};

export const postCreateGameAsync = async (): Promise<CreateGameResponseApiResponse> => {
    const response = await apiCall<CreateGameResponseApiResponse>('post', API_PATHS.createGame);
    return response.data;
};

export const postJoinGameAsync = async (gameId: string): Promise<ApiResponse> => {
    const response = await apiCall<ApiResponse>('post', `${API_PATHS.joinGame}/${gameId}`);
    return response.data;
};

export const postStartGameAsync = async (gameId: string): Promise<StartGameResponseApiResponse> => {
    const response = await apiCall<StartGameResponseApiResponse>('post', `${API_PATHS.startGame}/${gameId}`);
    return response.data;
};

//postLeaveLobby
export const postLeaveLobbyAsync = async (gameId: string): Promise<ApiResponse> => {
    const response = await apiCall<ApiResponse>('post', `${API_PATHS.leaveLobby}/${gameId}`);
    return response.data;
};

export const postEndTurnAsync = async (gameId: string, request: EndTurnRequest): Promise<EndTurnResponseApiResponse> => {
    const response = await apiCall<EndTurnResponseApiResponse>('post', `${API_PATHS.endTurn}/${gameId}`, request);
    return response.data;
};

export const putGameSettingsAsync = async (gameId: string, request: UpdateGameSettingsRequest): Promise<ApiResponse> => {
    const response = await apiCall<ApiResponse>('put', `${API_PATHS.gameSettings}/${gameId}`, request);
    return response.data;
};

export const getGlobalStatisticsAsync = async (): Promise<GlobalStatisticsResponseApiResponse> => {
    const response = await apiCall<GlobalStatisticsResponseApiResponse>('get', API_PATHS.globalStatistics);
    return response.data;
};

export const getUserStatisticsAsync = async (): Promise<UserStatisticsResponseApiResponse> => {
    const response = await apiCall<UserStatisticsResponseApiResponse>('get', API_PATHS.userStatistics);
    return response.data;
};