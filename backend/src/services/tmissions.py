import random
from sqlalchemy.orm import Session
from src.db.database import get_db
from fastapi import Depends
from src.models import data


def get_random_mission(db: Session = Depends(get_db)):
    missions = db.query(data.Mission).all()
    if not missions:
        return None
    return random.choice(missions)

def mark_complete_mission(mission_id: int, db: Session) -> bool:
    mission = db.query(data.Mission).filter(data.Mission.id == mission_id).first()
    
    if mission:
        mission.is_completed = True
        db.commit()
        db.refresh(mission)
        return True
    
    return False

