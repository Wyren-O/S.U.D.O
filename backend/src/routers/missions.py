from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session
from src.db.database import get_db
from src.schemas.data import MissionRead
from src.services.generator import generate_random_mission

router = APIRouter(prefix="/missions", tags=["missions"])

@router.get("/next", response_model=MissionRead)
def get_next_mission(db: Session = Depends(get_db)):
    mission = generate_random_mission(db)
    return mission

