from framework.utils.network import resolve_host


class PostgresHealth:

    def check(self, svc):
        try:
            host = resolve_host(svc)
            port = svc["port"]

            import socket
            with socket.create_connection((host, port), timeout=3):
                return True

        except Exception as e:
            print(f"❌ {e}")
            return False
