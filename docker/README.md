# boinc-parser stack

Tor (SOCKS5 9050 + ControlPort 9051) + FlareSolverr (8191) у спільній мережі.
FlareSolverr ходить на boincstats через Tor; worker дергає `SIGNAL NEWNYM` коли ловить капчу.

## Запуск

```bash
cp .env.example .env
# Відредагуй .env — встав свій TOR_CONTROL_PASSWORD
docker compose up -d --build
```

Перевірка:

```bash
# Тор готовий
curl --socks5-hostname localhost:9050 https://check.torproject.org/api/ip

# FlareSolverr живий
curl -X POST http://localhost:8191/v1 \
  -H "Content-Type: application/json" \
  -d '{"cmd":"request.get","url":"https://check.torproject.org/api/ip","maxTimeout":60000,"proxy":{"url":"socks5h://tor:9050"}}'
```

В обох випадках `IsTor: true` має бути.

## Ротація identity

Worker сам шле `SIGNAL NEWNYM` через ControlPort коли детектить блок. Перевірити вручну:

```bash
echo -e 'AUTHENTICATE "<password>"\r\nSIGNAL NEWNYM\r\nQUIT' | nc localhost 9051
```

## Логи

```bash
docker compose logs -f tor
docker compose logs -f flaresolverr
```