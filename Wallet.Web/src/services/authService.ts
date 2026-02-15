import apiClient from './apiClient';

export interface AuthResponse {
    token: string;
    expiration: string;
    userId: string;
    email: string;
    fullName: string;
}

export const authService = {
    login: async (email: string, password: string): Promise<AuthResponse> => {
        const response = await apiClient.post<AuthResponse>('/auth/login', { email, password });
        return response.data;
    },

    register: async (email: string, password: string, fullName: string): Promise<AuthResponse> => {
        const response = await apiClient.post<AuthResponse>('/auth/register', { email, password, fullName });
        return response.data;
    }
};

