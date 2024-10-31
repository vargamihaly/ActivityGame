import React, { useEffect } from 'react';
import { Formik, Form, useFormikContext } from 'formik';
import * as Yup from 'yup';
import { Box, Button } from '@mui/material';
import { useToast } from '@/hooks/use-toast';
import { useUpdateSettings } from '@/hooks/gameHooks';
import MaxScoreField from './MaxScoreField';
import MethodSelection from './MethodSelection';
import TimerField from "@/components/forms/updateSettings/TimerField";
import { components } from "@/api/activitygame-schema";
import {METHOD_TYPE} from "@/interfaces/GameTypes";

type MethodType = components['schemas']['MethodType'];

const SettingsSchema = Yup.object().shape({
    timer: Yup.number().min(1, 'Timer must be at least 1 minute').required('Timer is required'),
    maxScore: Yup.number().min(1, 'Max score must be at least 1').required('Max score is required'),
    methods: Yup.array().of(Yup.mixed<MethodType>().oneOf(Object.values(METHOD_TYPE))).min(1, 'At least one method must be selected').required('Methods are required'),
});

interface UpdateSettingsFormProps {
    gameId: string;
    timer: number;
    maxScore: number;
    enabledMethods: MethodType[];
    onSuccess: () => void;
    isDisabled?: boolean;
}

const FormikUpdater: React.FC<{ timer: number; maxScore: number; enabledMethods: MethodType[] }> = ({
                                                                                                        timer,
                                                                                                        maxScore,
                                                                                                        enabledMethods
                                                                                                    }) => {
    const { setFieldValue } = useFormikContext();

    useEffect(() => {
        const updateFields = async () => {
            try {
                await Promise.all([
                    setFieldValue('timer', timer || 1),
                    setFieldValue('maxScore', maxScore || 1),
                    setFieldValue('methods', enabledMethods || [])
                ]);
            } catch (error) {
                console.error('Error updating form fields:', error);
            }
        };

        updateFields();
    }, [timer, maxScore, enabledMethods, setFieldValue]);

    return null;
};

const UpdateSettingsForm: React.FC<UpdateSettingsFormProps> = ({
                                                                   gameId,
                                                                   timer,
                                                                   maxScore,
                                                                   enabledMethods,
                                                                   onSuccess,
                                                                   isDisabled = false,
                                                               }) => {
    const updateSettingsMutation = useUpdateSettings();
    const { toast } = useToast();

    return (
        <Formik
            initialValues={{
                timer: timer || 1,
                maxScore: maxScore || 1,
                methods: enabledMethods || [],
            }}
            validationSchema={SettingsSchema}
            onSubmit={async (values, { setSubmitting }) => {
                if (isDisabled) return;

                const requestPayload = {
                    timer: values.timer,
                    maxScore: values.maxScore,
                    enabledMethods: values.methods,
                };

                try {
                    await updateSettingsMutation.mutateAsync({ gameId, request: requestPayload });
                    onSuccess();
                    toast({
                        title: 'Success',
                        description: 'Game settings updated successfully!',
                    });
                } catch (error) {
                    toast({
                        title: 'Error',
                        description: `Failed to update settings: ${(error as Error).message}`,
                        variant: 'destructive',
                    });
                } finally {
                    setSubmitting(false);
                }
            }}
        >
            {({ errors, touched, isSubmitting, values, setFieldValue }) => (
                <Form>
                    <FormikUpdater timer={timer} maxScore={maxScore} enabledMethods={enabledMethods} />
                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                        <TimerField
                            value={values.timer}
                            error={errors.timer}
                            touched={touched.timer}
                            onChange={(value) => setFieldValue('timer', value)}
                            disabled={isDisabled}
                        />
                        <MaxScoreField
                            value={values.maxScore}
                            error={errors.maxScore}
                            touched={touched.maxScore}
                            onChange={(value) => setFieldValue('maxScore', value)}
                            disabled={isDisabled}
                        />
                        <MethodSelection
                            methods={values.methods}
                            onToggle={(method: MethodType) => {
                                if (isDisabled) return;
                                const newMethods = values.methods.includes(method)
                                    ? values.methods.filter((m) => m !== method)
                                    : [...values.methods, method];
                                setFieldValue('methods', newMethods);
                            }}
                            error={errors.methods as string}
                            touched={touched.methods}
                            disabled={isDisabled}
                        />
                        <Button type="submit" variant="contained" disabled={isSubmitting || isDisabled}>
                            {isSubmitting ? 'Updating...' : 'Update Settings'}
                        </Button>
                    </Box>
                </Form>
            )}
        </Formik>
    );
};

export default UpdateSettingsForm;