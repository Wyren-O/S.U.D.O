from src.services import tmissions
from sqlalchemy.orm import Session
from src.db.database import get_db
from fastapi import APIRouter, Depends, HTTPException
from src.services import tmissions

router = APIRouter(prefix="/missions")

@router.get("/random")
def read_random_mission(db: Session = Depends(get_db)):
    mission = tmissions.get_random_mission(db)
    
    
    if mission is None:
        return HTTPException(status_code=404, detail="No missions found")
    return mission