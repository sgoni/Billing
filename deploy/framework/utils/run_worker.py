def run_worker(svc):
    print(f"⚙️ Running worker {svc.name}...")
    subprocess.run(["docker", "compose", "run", "--rm", svc.name])