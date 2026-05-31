class NoopHealth:
    def check(self, svc):
        print(f"⚙️ Skipping health check for {svc.name} (type={svc.type})")
        return True
