import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter as Router } from 'react-router-dom'
import { GoogleOAuthProvider } from '@react-oauth/google'
import { ThemeProvider } from '@mui/material/styles'
import CssBaseline from '@mui/material/CssBaseline'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

import App from './App'
import theme from './theme'
import './index.css'

const queryClient = new QueryClient();

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <GoogleOAuthProvider clientId="303304165561-7lclve1c4ohh4ghn9t0l2ss0heeu3raq.apps.googleusercontent.com">
            <ThemeProvider theme={theme}>
                <CssBaseline />
                <QueryClientProvider client={queryClient}>
                <Router>
                    <App />
                </Router>
                </QueryClientProvider>
            </ThemeProvider>
        </GoogleOAuthProvider>
    </React.StrictMode>,
)
