def validate_services(services):
    for svc in services:
        if "internal_host" not in svc:
            raise Exception(f"{svc['name']} missing internal_host")

        if "external_host" not in svc:
            raise Exception(f"{svc['name']} missing external_host")

        if "port" not in svc:
            raise Exception(f"{svc['name']} missing port")