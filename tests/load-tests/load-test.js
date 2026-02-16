import http from 'k6/http';
import { check, sleep } from 'k6';

// Test configuration
export const options = {
    stages: [
        { duration: '30s', target: 20 }, // Ramp-up to 20 users
        { duration: '1m', target: 20 },  // Stay at 20 users
        { duration: '30s', target: 50 }, // Spike to 50 users
        { duration: '30s', target: 0 },  // Ramp-down
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% of requests must be under 500ms
        http_req_failed: ['rate<0.01'],   // Error rate must be less than 1%
    },
};

const BASE_URL = 'http://localhost:5171/api';

export default function () {
    // 1. Registration (Stress Hashing)
    const email = `user_${__VU}_${__ITER}@example.com`;
    const registerPayload = JSON.stringify({
        email: email,
        password: 'Password123!',
        fullName: 'Load Test User'
    });

    const regParams = { headers: { 'Content-Type': 'application/json' } };
    const regRes = http.post(`${BASE_URL}/auth/register`, registerPayload, regParams);

    check(regRes, {
        'reg status is 200': (r) => r.status === 200,
    });

    if (regRes.status === 200) {
        const data = JSON.parse(regRes.body);
        const token = data.token;
        const userId = data.userId;

        const authParams = {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
        };

        // 2. Create Wallet
        const wallRes = http.post(`${BASE_URL}/wallets`, JSON.stringify({
            userId: userId,
            currency: 'USD'
        }), authParams);

        check(wallRes, {
            'wallet created': (r) => r.status === 201 || r.status === 200,
        });

        // 3. Deposit Money
        http.post(`${BASE_URL}/wallets/deposit`, JSON.stringify({
            userId: userId,
            amount: 100,
            currency: 'USD'
        }), authParams);

        // 4. Get Wallet Info (Read Heavy)
        const getRes = http.get(`${BASE_URL}/wallets/${userId}`, authParams);
        check(getRes, {
            'get wallet status is 200': (r) => r.status === 200,
        });
    }

    sleep(1);
}
