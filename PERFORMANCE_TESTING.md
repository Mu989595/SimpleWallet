# Performance & Load Testing Guide

This guide explains how to test the SimpleWallet API "under pressure" using **k6**.

## 1. Prerequisites
You need to install **k6** on your machine.

**Windows (PowerShell):**
```powershell
winget install gnu.k6
```

## 2. Prepare the Environment
1. Ensure the Backend is running:
   ```powershell
   dotnet run --project Wallet.Api
   ```
2. Ensure you are in the `src` directory of the project.

## 3. Running the Test
Execute the following command to start the load test:

```powershell
k6 run tests/load-tests/load-test.js
```

## 4. Understanding the Results
When the test finishes, k6 will show a summary. Key metrics to watch:

| Metric | Meaning | Target Value |
|--------|---------|--------------|
| `http_req_duration` | Average time for a request | < 200ms |
| `http_req_failed` | Percentage of failed requests | 0.00% |
| `vus` | Current active Virtual Users | Look for errors as this increases |
| `iterations` | Total number of test cycles completed | - |

## 5. What to look for "Under Pressure"
- **Database Locks**: If multiple users transfer money at the exact same time, does the database handle it or do you see "Deadlock" errors?
- **Hashing Speed**: BCrypt is intentionally slow for security. If 100 users register at once, does CPU usage hit 100%?
- **Memory Leaks**: Does the memory usage of `Wallet.Api.exe` keep climbing even after the test stops?

## 6. Manual "Spam" Test (No Tools)
If you don't want to use k6, you can open multiple browser tabs and try to perform actions simultaneously, but **k6** is much more effective for "Real Pressure."
