import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

import { AuthProvider } from './context/AuthContext';
import { WalletProvider } from './context/WalletContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';

function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <WalletProvider>
                    <Routes>
                        <Route path="/login" element={<LoginPage />} />

                        <Route element={<ProtectedRoute />}>
                            <Route path="/dashboard" element={<DashboardPage />} />
                            <Route path="/" element={<Navigate to="/dashboard" replace />} />
                        </Route>
                    </Routes>
                    <ToastContainer position="bottom-right" />
                </WalletProvider>
            </AuthProvider>
        </BrowserRouter>
    );
}

export default App;
