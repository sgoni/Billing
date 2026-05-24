import os


def running_in_docker():
    return os.path.exists("/.dockerenv")


def resolve_host(svc):
    conn = svc.connection

    if running_in_docker():
        return conn.internal_host  # Docker
    else:
        return conn.external_host  # Host


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
