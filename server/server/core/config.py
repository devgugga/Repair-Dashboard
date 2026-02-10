from functools import lru_cache

from pydantic import Field
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    app_env: str = Field(default="local", validation_alias="APP_ENV")
    database_url: str | None = Field(default=None, validation_alias="DATABASE_URL")
    api_version_header: str = Field(default="X-API-Version", validation_alias="API_VERSION_HEADER")
    api_version_default: str = Field(default="1", validation_alias="API_VERSION_DEFAULT")

    model_config = SettingsConfigDict(
        env_file=".env",
        env_file_encoding="utf-8",
        extra="ignore",
    )


@lru_cache
def get_settings() -> Settings:
    return Settings()
