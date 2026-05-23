class PostgresProvider:

    def deploy(self, config, context):
        print(f"🐘 Configuring Postgres {config['name']}")

        # Aquí luego conectas Vault / Consul
        def deploy(self, svc, context):
            context.vault.inject_postgres(svc, context)

            print(f"🐘 Configuring Postgres {svc['name']}")