class PostgresProvider:

    def deploy(self, config, context):
        print(f"🐘 Configuring Postgres {config['name']}")

        # Aquí luego conectas Vault / Consul