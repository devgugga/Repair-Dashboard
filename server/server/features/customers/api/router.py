from fastapi import APIRouter

router = APIRouter(prefix="/customers", tags=["customers"])


@router.get("/health")
def customers_health() -> dict[str, str]:
    return {"feature": "customers", "status": "ok"}
