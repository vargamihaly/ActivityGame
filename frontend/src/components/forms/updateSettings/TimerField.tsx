import { TextField } from '@mui/material';
import React from "react";

interface TimerFieldProps {
    value: number;
    error?: string;
    touched?: boolean;
    onChange: (value: number) => void;
    disabled?: boolean;
}

const TimerField: React.FC<TimerFieldProps> = ({ value, error, touched, onChange, disabled }) => {
    return (
        <TextField
            name="timer"
            label="Timer (minutes)"
            type="number"
            value={value}
            onChange={(e) => onChange(Number(e.target.value))}
            error={touched && !!error}
            helperText={touched && error}
            disabled={disabled}
        />
    );
};

export default TimerField;
