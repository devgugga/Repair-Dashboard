from collections.abc import AsyncIterator

from fastapi import HTTPException, status
from sqlalchemy import text
from sqlalchemy.ext.asyncio import (
    AsyncEngine,
    AsyncSession,
    async_sessionmaker,
    create_async_engine,
)

_engine: AsyncEngine | None = None
_sessionmaker: async_sessionmaker[AsyncSession] | None = None


def init_database(database_url: str) -> None:
    global _engine, _sessionmaker
    if _engine is not None:
        return

    _engine = create_async_engine(
        database_url,
        pool_pre_ping=True,
    )
    _sessionmaker = async_sessionmaker(
        bind=_engine,
        autoflush=False,
        expire_on_commit=False,
    )


async def dispose_database() -> None:
    global _engine, _sessionmaker
    if _engine is not None:
        await _engine.dispose()
    _engine = None
    _sessionmaker = None


async def check_db_connection() -> None:
    if _engine is None:
        return

    async with _engine.connect() as connection:
        await connection.execute(text("SELECT 1"))


async def get_db_session() -> AsyncIterator[AsyncSession]:
    if _sessionmaker is None:
        raise HTTPException(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            detail="Database is not configured.",
        )

    async with _sessionmaker() as session:
        yield session
