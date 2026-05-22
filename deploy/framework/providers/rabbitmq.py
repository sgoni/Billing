import json

class RabbitMQProvider:

    def deploy(self, config, context):
        print(f"🐇 Configuring RabbitMQ {config['name']}")

        # Ejemplo futuro Vault:
        """
        client.secrets.rabbitmq.configure(...)
        client.secrets.rabbitmq.create_role(
            name=config["name"],
            vhosts=json.dumps({...})
        )
        """