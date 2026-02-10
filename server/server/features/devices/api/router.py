from fastapi import APIRouter

router = APIRouter(prefix="/devices", tags=["devices"])


@router.get("/health")
def devices_health() -> dict[str, str]:
    return {"feature": "devices", "status": "ok"}
