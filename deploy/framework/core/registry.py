class ProviderRegistry:
    def __init__(self):
        from framework.providers.postgres import PostgresProvider
        from framework.providers.rabbitmq import RabbitMQProvider
        from framework.providers.http import HttpProvider

        self.providers = {
            "postgres": PostgresProvider(),
            "rabbitmq": RabbitMQProvider(),
            "http": HttpProvider(),
        }

    def get(self, type_):
        return self.providers[type_]