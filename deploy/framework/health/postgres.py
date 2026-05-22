import socket


class PostgresHealth:

    def check(self, config):
        host = config.get("host")
        port = int(config.get("port", 5432))

        if not host:
            print(f"❌ Postgres '{config.get('name')}' sin host")
            return False

        try:
            print(f"🔌 Trying {host}:{port}")
            with socket.create_connection((host, port), timeout=3):
                return True
        except Exception as e:
            print(f"❌ Connection failed: {e}")
            return False