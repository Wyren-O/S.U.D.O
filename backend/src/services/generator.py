import random
from sqlalchemy.orm import Session
from src.models.data import Mission

PREFIXES = ["ALPHA", "BRAVO", "CHARLIE", "OMEGA", "SUDO", "ROOT"]
ACTIONS = ["DECRYPT", "INTERCEPT", "UPLOAD", "TRACE", "PURGE"]
TARGETS = ["DATA_STREAM", "PROXY_NODE", "SATELLITE_UPLINK", "CORE_KERNEL"]

def generate_random_mission(db: Session) -> Mission:
    title = f"{random.choice(ACTIONS)}_{random.choice(PREFIXES)}-{random.randint(10,99)}"
    
    frequency = round(random.uniform(90, 108.0), 1)
    
    description = f"Intecept signal on {frequency} MHz. Target: {random.choice(TARGETS)}."
    
    file_size = round(random.uniform(10.0, 900.0), 2)
    
    new_mission = Mission(
        title=title,
        description=description,
        frequency=frequency,
        file_size=file_size,
        is_completed=False
    )
    
    db.add(new_mission)
    db.commit()
    db.refresh(new_mission)
    
    return new_mission