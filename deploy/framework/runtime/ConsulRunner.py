from framework.utils.resolver import resolve_consul_address


class ConsulRunner:

    def __init__(self, consul_manager):
        self.consul = consul_manager

    def register(self, services):
        print("🧭 Consul registration starting...")

        for svc in services:
            print(f"DEBUG → {svc.name} | consul: {getattr(svc, 'consul', None)}")

            # ✅ FIX: usar svc, no services
            if not getattr(svc, "consul", None) or not svc.consul.enabled:
                continue

            print(f"🧭 Registering service in Consul: {svc.name}")

            address = resolve_consul_address(svc)
            # address = "host.docker.internal"

            self.consul.register_service(
                name=svc.consul.service_name or svc.name,
                service_id=svc.consul.service_id or f"{svc.name}-1",
                address=address,
                port=svc.connection.port
            )

        print("✅ Consul registration completed")
