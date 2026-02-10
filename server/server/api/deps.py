from fastapi import HTTPException, Request, status

from server.core.config import get_settings


def require_api_version(request: Request) -> None:
    settings = get_settings()
    header_name = settings.api_version_header
    received_version = request.headers.get(header_name)

    if received_version is None:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail=f"Missing required API version header: {header_name}",
        )

    if received_version != settings.api_version_default:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail=(
                f"Unsupported API version '{received_version}'. "
                f"Supported version: {settings.api_version_default}"
            ),
        )
