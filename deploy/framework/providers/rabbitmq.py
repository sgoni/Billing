class RabbitMQProvider:

    def deploy(self, svc, context):
        print(f"🐇 Configuring RabbitMQ {svc['name']}")

        # ✅ PROTEGER ACCESO A VAULT
        if context.vault:
            context.vault.inject_rabbitmq(svc, context)
        else:
            print("⚠️ Skipping Vault injection (Vault not available)")
