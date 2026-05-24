#!/bin/bash
set -euo pipefail

if [ -z "${TOR_CONTROL_PASSWORD:-}" ]; then
    echo "FATAL: TOR_CONTROL_PASSWORD is not set" >&2
    exit 1
fi

HASHED=$(tor --hash-password "$TOR_CONTROL_PASSWORD" | tail -n1)
if [ -z "$HASHED" ]; then
    echo "FATAL: failed to hash control password" >&2
    exit 1
fi

TORRC=/tmp/torrc
sed "s|__HASHED_PASSWORD__|$HASHED|g" /etc/tor/torrc.template > "$TORRC"

echo "Starting tor with generated config..."
exec tor -f "$TORRC"