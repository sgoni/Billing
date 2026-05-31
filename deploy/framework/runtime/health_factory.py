from framework.runtime.health import PostgresHealth, RabbitHealth
from framework.health.HttpHealthChecker import HttpHealth
from framework.health.NoopHealth import NoopHealth


def get_health_checker(svc):
    if svc.type == "postgres":
        return PostgresHealth()

    if svc.type == "rabbitmq":
        return RabbitHealth()

    if svc.type == "http":
        return HttpHealth()

    if svc.type == "worker":
        return NoopHealth()

    raise ValueError(f"No health checker for {svc.type}")
