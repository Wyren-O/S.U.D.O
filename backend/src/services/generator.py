import random
from sqlalchemy.orm import Session
from src.models.data import Mission

PREFIXES = ["ALPHA", "BRAVO", "CHARLIE", "OMEGA", "SUDO", "ROOT"]
ACTIONS = ["DECRYPT", "INTERCEPT", "UPLOAD", "TRACE", "PURGE"]
TARGETS = ["DATA_STREAM", "PROXY_NODE", "SATELLITE_UPLINK", "CORE_KERNEL"]

TEXT_LIST = [
    "SOS. WE ARE TRAPPED IN SECTOR 4.",
    "THEY ARE COMING. CLOSE THE GATE.",
    "CODE 451. NUCLEAR LAUNCH DETECTED.",
    "DONT TRUST THE VOICE ON THE RADIO.",
    "WEATHER REPORT: ACID RAIN EXPECTED."
]

IMAGE_LIST = [          
    "secret_map.jpg",     
]

AUDIO_LIST = [
    "682401__pnmcarrierailfan__explosions-echo-blast-explos.mp3"
]

def generate_random_mission(db: Session) -> Mission:
    title = f"{random.choice(ACTIONS)}_{random.choice(PREFIXES)}-{random.randint(10,99)}"
    
    frequency = round(random.uniform(90, 108.0), 1)
    
    description = f"Intecept signal on {frequency} MHz. Target: {random.choice(TARGETS)}."
    
    file_size = round(random.uniform(10.0, 900.0), 2)
    m_type = random.choice(["TEXT", "IMAGE", "AUDIO"])
    
    content = ""
    description = ""
    
    if m_type == "TEXT":
        content = random.choice(TEXT_LIST)
        description = f"Intercept TEXT msg on {frequency} MHz."
    elif m_type == "IMAGE":
        content = random.choice(IMAGE_LIST)
        description = f"Decode IMAGE signal on {frequency} MHz."
    elif m_type == "AUDIO":
        content = random.choice(AUDIO_LIST)
        description = f"Record AUDIO signal on {frequency} MHz."
    
    new_mission = Mission(
        title=title,
        description=description,
        frequency=frequency,
        file_size=file_size,
        is_completed=False,
        mission_type=m_type,     
        mission_content=content
    )
    
    db.add(new_mission)
    db.commit()
    db.refresh(new_mission)
    
    return new_mission