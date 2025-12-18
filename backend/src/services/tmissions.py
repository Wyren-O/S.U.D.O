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