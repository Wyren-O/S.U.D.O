from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from src.db.database import get_db
from src.schemas.data import MissionRead
from src.services.generator import generate_random_mission
from src.models.data import Mission
from src.services.tmissions import mark_complete_mission

router = APIRouter(prefix="/missions", tags=["missions"])

@router.get("/next", response_model=MissionRead)
def get_next_mission(db: Session = Depends(get_db)):
    mission = generate_random_mission(db)
    return mission

@router.post("/{mission_id}/complete")
def complete_mission(mission_id: int, db: Session = Depends(get_db)):
    success = mark_complete_mission(mission_id, db)
    
    if not success: 
        raise HTTPException(status_code=404, detail="Mission not found")
    
    return {"status": "success", "message": "Mission completed"}

