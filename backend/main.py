from fastapi import FastAPI
from src.db.database import engine, Base
from src.models import data
from src.routers import missions
from src.routers import tmissions

Base.metadata.create_all(bind=engine)

app = FastAPI()

app.include_router(missions.router)
app.include_router(tmissions.router)

@app.get("/")
def health_check():
    return {"status": "True"}