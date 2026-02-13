import axios from 'axios';
import { toast } from 'react-toastify';

const apiClient = axios.create({
    baseURL: 'http://localhost:5171/api', // Matches backend port
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request Interceptor: Attach Token
apiClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Response Interceptor: Global Error Handling
apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        const message = error.response?.data?.message || 'Something went wrong';

        // Handle 401 Unauthorized
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            window.location.href = '/login';
        } else {
            toast.error(message);
        }

        return Promise.reject(error);
    }
);

export default apiClient;
