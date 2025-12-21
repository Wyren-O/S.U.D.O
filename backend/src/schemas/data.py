from pydantic import BaseModel, ConfigDict
from enum import Enum

class ContentType(str, Enum):
    TEXT = "text"
    AUDIO = "audio"
    IMAGE = "image"
    
class MissionBase(BaseModel):
    title: str
    description: str
    frequency: float
    file_size: float
    mission_type: str
    mission_content: str | None = None
    scary: bool = False
    
class MissionCreate(MissionBase):
    pass

class MissionRead(MissionBase):
    id: int
    is_completed: bool
    
    model_config = ConfigDict(From_attributes=True)
    
    