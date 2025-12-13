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
    
    scary: bool = False
    
class MissionCreate(MissionBase):
    pass

class MissionRead(MissionBase):
    id: int
    is_completed: bool
    
    model_config = ConfigDict(From_attributes=True)
    
    