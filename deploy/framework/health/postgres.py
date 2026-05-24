from framework.utils.network import resolve_host


class PostgresHealth:

    def check(self, svc):
        try:
            conn = svc["connection"]

            host = resolve_host(svc)
            port = svc.connection.port

            import socket
            print(f"🔌 Checking Postgres: {host}:{port}")

            with socket.create_connection((host, port), timeout=3):
                return True

        except Exception as e:
            print(f"❌ Postgres error: {e}")
            return False
