import http from 'k6/http';
import { sleep } from 'k6';

export default function () {
    // Simple check on the root API or a public endpoint
    // Note: Most wallet endpoints require [Authorize] so they need a token!
    // This is a simple test for the Swagger UI or Auth path.
    http.get('http://localhost:5171/swagger/index.html');
    sleep(1);
}
