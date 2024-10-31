import {components} from "@/api/activitygame-schema";

type GameStatus = components["schemas"]["GameStatus"];
type MethodType = components["schemas"]["MethodType"];

export const GAME_STATUS: Record<GameStatus, GameStatus> = {
    Waiting: "Waiting",
    InProgress: "InProgress",
    Finished: "Finished"
};

export const METHOD_TYPE: Record<MethodType, MethodType> = {
    Drawing: "Drawing",
    Description: "Description",
    Mimic: "Mimic"
};
