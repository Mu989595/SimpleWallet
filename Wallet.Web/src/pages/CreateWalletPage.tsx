import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useWallet } from '../context/WalletContext';
import { Wallet as WalletIcon, Loader2, Sparkles } from 'lucide-react';
import { toast } from 'react-toastify';

export default function CreateWalletPage() {
    const [currency, setCurrency] = useState('USD');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { createWallet } = useWallet();
    const navigate = useNavigate();

    const handleCreate = async () => {
        try {
            setIsSubmitting(true);
            await createWallet(currency);
            toast.success("Wallet created successfully! Enjoy your new digital wallet.");
            navigate('/dashboard');
        } catch (error) {
            // Error handled by global interceptor
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center p-6">
            <div className="max-w-md w-full bg-white rounded-2xl shadow-xl overflow-hidden border border-gray-100">
                <div className="bg-blue-600 p-8 text-center text-white">
                    <div className="inline-flex items-center justify-center w-16 h-16 bg-white/20 rounded-full mb-4">
                        <WalletIcon size={32} />
                    </div>
                    <h1 className="text-2xl font-bold">Start Your Journey</h1>
                    <p className="text-blue-100 mt-2 text-sm">Initialize your wallet to start handling transactions</p>
                </div>

                <div className="p-8 space-y-6">
                    <div className="space-y-2">
                        <label className="text-sm font-semibold text-gray-700">Choose Currency</label>
                        <select
                            value={currency}
                            onChange={(e) => setCurrency(e.target.value)}
                            className="w-full px-4 py-3 border border-gray-200 rounded-xl bg-gray-50 focus:ring-2 focus:ring-blue-500 outline-none transition"
                        >
                            <option value="USD">🇺🇸 USD - US Dollar</option>
                            <option value="EGP">🇪🇬 EGP - Egyptian Pound</option>
                            <option value="EUR">🇪🇺 EUR - Euro</option>
                            <option value="GBP">🇬🇧 GBP - British Pound</option>
                        </select>
                        <p className="text-xs text-gray-400">You can change your default currency later from settings.</p>
                    </div>

                    <div className="bg-blue-50 p-4 rounded-xl flex gap-3 text-blue-700 text-sm italic">
                        <Sparkles size={20} className="shrink-0" />
                        <p>Your wallet will be secured with our atomic transaction engine and optimistic locking.</p>
                    </div>

                    <button
                        onClick={handleCreate}
                        disabled={isSubmitting}
                        className="w-full bg-blue-600 text-white py-4 rounded-xl font-bold hover:bg-blue-700 transition shadow-lg shadow-blue-200 flex items-center justify-center gap-2 disabled:opacity-50"
                    >
                        {isSubmitting ? (
                            <Loader2 className="animate-spin" />
                        ) : (
                            "Initialize My Wallet"
                        )}
                    </button>

                    <button
                        onClick={() => navigate('/dashboard')}
                        className="w-full text-gray-500 text-sm hover:text-gray-700 transition font-medium"
                    >
                        Not now, take me back
                    </button>
                </div>
            </div>
        </div>
    );
}
