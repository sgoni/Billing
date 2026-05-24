from framework.runtime.health import PostgresHealth, RabbitHealth


def get_health_checker(svc):
    if svc.type == "postgres":
        return PostgresHealth()

    if svc.type == "rabbitmq":
        return RabbitHealth()

    raise ValueError(f"No health checker for {svc.type}")
