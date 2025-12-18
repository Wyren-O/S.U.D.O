from sqlalchemy.orm import Mapped, mapped_column 
from src.db.database import Base

class Mission(Base):
    __tablename__ = "missions"
    
    id: Mapped[int] = mapped_column(primary_key=True, index=True)
    title: Mapped[str] = mapped_column(index=False)
    description: Mapped[str]
    frequency: Mapped[float]
    file_size: Mapped[float]
    is_completed: Mapped[bool] = mapped_column(default=False)
    mission_type: Mapped[str] = mapped_column(default="text")
    