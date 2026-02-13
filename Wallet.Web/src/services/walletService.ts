import apiClient from './apiClient';

export interface Money {
    amount: number;
    currency: string;
}

export interface WalletDto {
    id: string;
    userId: string;
    balance: Money;
    transactions: TransactionDto[];
}

export interface TransactionDto {
    id: string;
    amount: Money;
    type: string; // Deposit, Withdraw, Transfer
    timestamp: string;
}

export const walletService = {
    createWallet: async (userId: string, currency: string = 'USD') => {
        return apiClient.post('/wallets', { userId, currency });
    },

    getWallet: async (userId: string): Promise<WalletDto> => {
        const response = await apiClient.get<WalletDto>(`/wallets/${userId}`);
        return response.data;
    },

    transfer: async (sourceUserId: string, destinationUserId: string, amount: number, currency: string) => {
        return apiClient.post('/wallets/transfer', {
            sourceUserId,
            destinationUserId,
            amount,
            currency
        });
    }
};
