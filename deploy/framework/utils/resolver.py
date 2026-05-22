import os
import re

pattern = re.compile(r"\$\{(.+?)\}")


def resolve(value):
    if isinstance(value, str):
        for match in pattern.findall(value):
            value = value.replace(f"${{{match}}}", os.environ.get(match, ""))
    return value


def resolve_dict(d):
    if isinstance(d, dict):
        return {k: resolve_dict(v) for k, v in d.items()}
    elif isinstance(d, list):
        return [resolve_dict(i) for i in d]
    else:
        return resolve(d)


def require_env(var):
    value = os.environ.get(var)
    if not value:
        raise ValueError(f"Missing env var: {var}")
    return value
