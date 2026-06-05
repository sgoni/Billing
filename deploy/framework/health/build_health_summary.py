from framework.runtime.health_factory import get_health_checker


def build_health_summary(services):
    summary = []

    for svc in services:
        status = "unknown"

        try:
            checker = get_health_checker(svc)
            ok = checker.check(svc)
            status = "healthy" if ok else "unhealthy"

        except Exception as ex:
            status = f"error: {ex}"

        summary.append({
            "name": svc.name,
            "type": svc.type,
            "status": status
        })

    return summary
