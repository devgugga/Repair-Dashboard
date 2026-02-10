import logging
from collections.abc import AsyncIterator
from contextlib import asynccontextmanager

from fastapi import FastAPI

from server.core.config import get_settings
from server.core.database import (
    check_db_connection,
    dispose_database,
    init_database,
)
from server.core.logging import configure_logging

logger = logging.getLogger(__name__)


@asynccontextmanager
async def lifespan(_: FastAPI) -> AsyncIterator[None]:
    configure_logging()
    settings = get_settings()

    if settings.database_url:
        init_database(settings.database_url)
        await check_db_connection()
        logger.info("Database connection initialized successfully.")
    else:
        logger.warning("DATABASE_URL is not set; database-dependent features are disabled.")

    try:
        yield
    finally:
        await dispose_database()
