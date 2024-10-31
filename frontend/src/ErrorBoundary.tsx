import React, { ErrorInfo, ReactNode } from 'react';

interface ErrorBoundaryProps {
    children: ReactNode;
}

interface ErrorBoundaryState {
    hasError: boolean;
}

class ErrorBoundary extends React.Component<ErrorBoundaryProps, ErrorBoundaryState> {
    constructor(props: ErrorBoundaryProps) {
        super(props);
        this.state = { hasError: false };
    }

    static getDerivedStateFromError(_: Error): ErrorBoundaryState {
        return { hasError: true };
    }

    componentDidCatch(error: Error, errorInfo: ErrorInfo) {
        console.error('ErrorBoundary caught an error:', error, errorInfo);
    }

    render() {
        if (this.state.hasError) {
            console.log('ErrorBoundary: Rendering error state');
            return <h1>Something went wrong.</h1>;
        }

        console.log('ErrorBoundary: Rendering children');
        return this.props.children;
    }
}

export default ErrorBoundary;