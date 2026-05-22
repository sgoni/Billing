from framework.health.http import HttpHealth
from framework.health.rabbitmq import RabbitHealth
from framework.health.postgres import PostgresHealth


class HealthRegistry:

    def __init__(self):
        self.checkers = {
            "http": HttpHealth(),
            "rabbitmq": RabbitHealth(),
            "postgres": PostgresHealth(),
        }

    def get(self, type_):
        return self.checkers.get(type_)