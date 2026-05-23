import os


def is_running_in_docker():
    return os.path.exists("/.dockerenv")


def resolve_host(svc):
    if is_running_in_docker():
        host = svc.get("internal_host")
    else:
        host = svc.get("external_host")

    if not host:
        raise Exception(f"❌ Service '{svc['name']}' sin host definido")

    return host


def resolve_port(svc, key="port"):
    port = svc.get(key)

    if not port:
        raise Exception(f"❌ Service '{svc['name']}' sin puerto '{key}'")

    return port


def build_tcp_target(svc):
    host = resolve_host(svc)
    port = resolve_port(svc)
    return host, port


def build_http_url(svc, port_key="port", path=""):
    host = resolve_host(svc)
    port = resolve_port(svc, port_key)

    base = f"http://{host}:{port}"
    return f"{base}{path}"
