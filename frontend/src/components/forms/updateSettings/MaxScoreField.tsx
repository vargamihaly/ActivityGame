import { TextField } from '@mui/material';
import React from "react";

interface MaxScoreFieldProps {
    value: number;
    error?: string;
    touched?: boolean;
    onChange: (value: number) => void;
    disabled?: boolean;
}

const MaxScoreField: React.FC<MaxScoreFieldProps> = ({ value, error, touched, onChange, disabled }) => {
    return (
        <TextField
            name="maxScore"
            label="Max Score"
            type="number"
            value={value}
            onChange={(e) => onChange(Number(e.target.value))}
            error={touched && !!error}
            helperText={touched && error}
            disabled={disabled}
        />
    );
};

export default MaxScoreField;