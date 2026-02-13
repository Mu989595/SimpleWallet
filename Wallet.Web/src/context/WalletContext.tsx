import React, { createContext, useContext, useState, useCallback } from 'react';
import { walletService, WalletDto, TransactionDto } from '../services/walletService';
import { useAuth } from './AuthContext';
import { toast } from 'react-toastify';

interface WalletContextType {
    wallet: WalletDto | null;
    isLoading: boolean;
    refreshWallet: () => Promise<void>;
    transferMoney: (toUserId: string, amount: number, currency: string) => Promise<void>;
    createWallet: (currency: string) => Promise<void>;
}

const WalletContext = createContext<WalletContextType | null>(null);

export const WalletProvider = ({ children }: { children: React.ReactNode }) => {
    const { user } = useAuth();
    const [wallet, setWallet] = useState<WalletDto | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const refreshWallet = useCallback(async () => {
        if (!user?.userId) return;

        try {
            setIsLoading(true);
            const data = await walletService.getWallet(user.userId);
            setWallet(data);
        } catch (error) {
            console.error("Failed to fetch wallet", error);
        } finally {
            setIsLoading(false);
        }
    }, [user]);

    const transferMoney = async (toUserId: string, amount: number, currency: string) => {
        if (!user?.userId) return;
        try {
            await walletService.transfer(user.userId, toUserId, amount, currency);
            toast.success('Transfer successful!');
            await refreshWallet(); // Auto-update UI
        } catch (error) {
            // Error handled by global interceptor, but we catch here to stop generic flow if needed
            throw error;
        }
    };

    const createWallet = async (currency: string) => {
        if (!user?.userId) return;
        await walletService.createWallet(user.userId, currency);
        await refreshWallet();
    };

    return (
        <WalletContext.Provider value={{ wallet, isLoading, refreshWallet, transferMoney, createWallet }}>
            {children}
        </WalletContext.Provider>
    );
};

export const useWallet = () => {
    const context = useContext(WalletContext);
    if (!context) throw new Error('useWallet must be used within WalletProvider');
    return context;
};
