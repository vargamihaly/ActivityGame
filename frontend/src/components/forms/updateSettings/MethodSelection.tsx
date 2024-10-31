import { Checkbox, FormControlLabel, FormGroup, Typography } from '@mui/material';
import React from "react";

import {METHOD_TYPE} from "@/interfaces/GameTypes";
import {components} from "@/api/activitygame-schema";

type MethodType = components['schemas']['MethodType'];

interface MethodSelectionProps {
    methods: MethodType[];
    onToggle: (method: MethodType) => void;
    error?: string;
    touched?: boolean;
    disabled?: boolean;
}

const MethodSelection: React.FC<MethodSelectionProps> = ({ methods, onToggle, error, touched, disabled }) => {
    return (
        <>
            <Typography variant="subtitle1">Game Methods:</Typography>
            <FormGroup>
                {Object.entries(METHOD_TYPE).map(([key, value]) => (
                    <FormControlLabel
                        key={key}
                        control={
                            <Checkbox
                                name="methods"
                                checked={methods.includes(value)}
                                onChange={() => onToggle(value)}
                                disabled={disabled}
                            />
                        }
                        label={key}
                    />
                ))}
            </FormGroup>
            {touched && error && <Typography color="error">{error}</Typography>}
        </>
    );
};

export default MethodSelection;