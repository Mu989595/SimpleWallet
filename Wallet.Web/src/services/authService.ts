import apiClient from './apiClient';

export interface LoginRequest {
    email: string; // The API uses generic login, but for our Wallet we used UserId? 
    // Wait, the API I implemented doesn't have a Login endpoint yet!
    // The user requirement says "JWT Authentication" was ALREADY implemented.
    // I need to assume there is an AuthClient or I might need to clarify.
    // "I already have a Clean Architecture .NET API implemented with... JWT Authentication"
    // I will assume standard endpoint: POST /api/Auth/login
}

export interface AuthResponse {
    token: string;
    userId: string;
    email: string;
}

export const authService = {
    login: async (email: string, password: string): Promise<AuthResponse> => {
        // Mocking login for now if endpoint is missing, or assuming it exists.
        // Given I implemented WalletsController, I did NOT implement AuthController.
        // But user said "I already have... JWT Authentication".
        // I will assume the endpoint exists at /api/auth/login.
        const response = await apiClient.post<AuthResponse>('/auth/login', { email, password });
        return response.data;
    },

    register: async (email: string, password: string) => {
        return apiClient.post('/auth/register', { email, password });
    }
};
