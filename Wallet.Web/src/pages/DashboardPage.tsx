import { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { useWallet } from '../context/WalletContext';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { Loader2, ArrowRightLeft, DollarSign, Wallet as WalletIcon } from 'lucide-react';
import clsx from 'clsx';

// Validation Schema
const transferSchema = z.object({
    recipientId: z.string().uuid("Invalid User ID format"),
    amount: z.number().min(0.01, "Amount must be greater than 0"),
    currency: z.string().length(3, "Currency code must be 3 chars").default("USD")
});

type TransferForm = z.infer<typeof transferSchema>;

import { useNavigate } from 'react-router-dom';

export default function DashboardPage() {
    const { user, logout } = useAuth();
    const { wallet, isLoading, refreshWallet, transferMoney } = useWallet();
    const [isTransferring, setIsTransferring] = useState(false);
    const navigate = useNavigate();

    const { register, handleSubmit, reset, formState: { errors } } = useForm<TransferForm>({
        resolver: zodResolver(transferSchema)
    });

    useEffect(() => {
        refreshWallet();
    }, [refreshWallet]);

    const onSubmit = async (data: TransferForm) => {
        try {
            setIsTransferring(true);
            await transferMoney(data.recipientId, data.amount, data.currency);
            reset();
        } catch (e) {
            // Handled by global toast
        } finally {
            setIsTransferring(false);
        }
    };

    if (isLoading && !wallet) {
        return (
            <div className="flex h-screen items-center justify-center">
                <Loader2 className="animate-spin h-10 w-10 text-blue-600" />
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 flex flex-col">
            {/* Navbar */}
            <nav className="bg-white shadow-sm p-4 flex justify-between items-center">
                <div className="flex items-center gap-2 text-blue-700 font-bold text-xl">
                    <WalletIcon /> SimpleWallet
                </div>
                <div className="flex items-center gap-4">
                    <span className="text-gray-600">{user?.email}</span>
                    <button onClick={logout} className="text-red-500 hover:text-red-700 text-sm font-medium">Sign Out</button>
                </div>
            </nav>

            <main className="flex-1 p-8 max-w-5xl mx-auto w-full grid grid-cols-1 md:grid-cols-2 gap-8">

                {/* Left Column: Wallet Status */}
                <div className="space-y-6">
                    {!wallet ? (
                        <div className="bg-white p-6 rounded-xl shadow-sm text-center">
                            <h2 className="text-lg font-semibold text-gray-700 mb-2">No Wallet Found</h2>
                            <p className="text-gray-500 text-sm mb-6">Create a wallet to start making transactions and tracking your balance.</p>
                            <button
                                onClick={() => navigate('/create-wallet')}
                                className="bg-blue-600 text-white px-8 py-3 rounded-xl font-bold hover:bg-blue-700 transition shadow-lg shadow-blue-100"
                            >
                                Setup My Wallet
                            </button>
                        </div>
                    ) : (
                        <>
                            {/* Balance Card */}
                            <div className="bg-gradient-to-r from-blue-600 to-indigo-700 rounded-2xl p-8 text-white shadow-lg">
                                <p className="text-blue-100 text-sm font-medium uppercase tracking-wider">Total Balance</p>
                                <div className="flex items-baseline mt-2">
                                    <span className="text-5xl font-bold tracking-tight">
                                        {wallet.balance.amount.toLocaleString()}
                                    </span>
                                    <span className="text-xl ml-2 text-blue-200">{wallet.balance.currency}</span>
                                </div>
                                <div className="mt-6 flex items-center gap-2 text-blue-200 text-sm">
                                    <WalletIcon size={16} />
                                    <span>Wallet ID: {wallet.id.substring(0, 8)}...</span>
                                </div>
                            </div>

                            {/* Transactions List */}
                            <div className="bg-white rounded-xl shadow-sm overflow-hidden border border-gray-100">
                                <div className="p-4 border-b border-gray-100 bg-gray-50">
                                    <h3 className="font-semibold text-gray-700">Recent Transactions</h3>
                                </div>
                                <div className="divide-y divide-gray-100 max-h-[400px] overflow-y-auto">
                                    {wallet.transactions.length === 0 ? (
                                        <div className="p-8 text-center text-gray-400 text-sm">No transactions yet</div>
                                    ) : (
                                        wallet.transactions.map((tx) => (
                                            <div key={tx.id} className="p-4 flex justify-between items-center hover:bg-gray-50">
                                                <div className="flex items-center gap-3">
                                                    <div className={clsx(
                                                        "p-2 rounded-full",
                                                        tx.type === 'Deposit' ? "bg-green-100 text-green-600" :
                                                            tx.type === 'Withdraw' ? "bg-red-100 text-red-600" :
                                                                "bg-blue-100 text-blue-600"
                                                    )}>
                                                        {tx.type === 'Transfer' ? <ArrowRightLeft size={18} /> :
                                                            <DollarSign size={18} />}
                                                    </div>
                                                    <div>
                                                        <p className="font-medium text-gray-900">{tx.type}</p>
                                                        <p className="text-xs text-gray-500">{new Date(tx.timestamp).toLocaleDateString()}</p>
                                                    </div>
                                                </div>
                                                <span className={clsx(
                                                    "font-bold",
                                                    tx.type === 'Deposit' ? "text-green-600" : "text-gray-900"
                                                )}>
                                                    {tx.type === 'Deposit' ? '+' : '-'}{tx.amount.amount} {tx.amount.currency}
                                                </span>
                                            </div>
                                        ))
                                    )}
                                </div>
                            </div>
                        </>
                    )}
                </div>

                {/* Right Column: Transfer Form */}
                <div className="space-y-6">
                    <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-100">
                        <h3 className="text-lg font-bold text-gray-800 mb-4 flex items-center gap-2">
                            <ArrowRightLeft className="text-blue-500" /> Transfer Money
                        </h3>

                        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">Recipient User ID</label>
                                <input
                                    {...register('recipientId')}
                                    placeholder="e.g. 3fa85f64..."
                                    className={clsx(
                                        "w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none transition",
                                        errors.recipientId ? "border-red-300 bg-red-50" : "border-gray-200"
                                    )}
                                />
                                {errors.recipientId && <p className="text-red-500 text-xs mt-1">{errors.recipientId.message}</p>}
                            </div>

                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">Amount</label>
                                    <input
                                        type="number"
                                        step="0.01"
                                        {...register('amount', { valueAsNumber: true })}
                                        className={clsx(
                                            "w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none transition",
                                            errors.amount ? "border-red-300 bg-red-50" : "border-gray-200"
                                        )}
                                    />
                                    {errors.amount && <p className="text-red-500 text-xs mt-1">{errors.amount.message}</p>}
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">Currency</label>
                                    <select
                                        {...register('currency')}
                                        className="w-full px-3 py-2 border border-gray-200 rounded-lg bg-white"
                                    >
                                        <option value="USD">USD</option>
                                        <option value="EGP">EGP</option>
                                        <option value="EUR">EUR</option>
                                    </select>
                                </div>
                            </div>

                            <button
                                type="submit"
                                disabled={isTransferring || !wallet}
                                className="w-full bg-black text-white py-3 rounded-lg font-medium hover:bg-gray-800 transition disabled:opacity-50 disabled:cursor-not-allowed flex justify-center items-center gap-2"
                            >
                                {isTransferring && <Loader2 className="animate-spin h-4 w-4" />}
                                {isTransferring ? 'Processing...' : 'Transfer Now'}
                            </button>
                        </form>

                        <div className="mt-4 p-3 bg-blue-50 rounded-lg text-xs text-blue-700">
                            <p>💡 <b>Tip:</b> Transfers are atomic. If the recipient doesn't exist, your money will not be deducted.</p>
                        </div>
                    </div>
                </div>

            </main>
        </div>
    );
}
