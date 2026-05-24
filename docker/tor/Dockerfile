FROM alpine:3.20

RUN apk add --no-cache tor bash \
    && mkdir -p /var/lib/tor /etc/tor \
    && chown -R tor:tor /var/lib/tor /etc/tor

COPY torrc.template /etc/tor/torrc.template
COPY entrypoint.sh /usr/local/bin/entrypoint.sh
RUN chmod +x /usr/local/bin/entrypoint.sh

USER tor

EXPOSE 9050 9051

ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]