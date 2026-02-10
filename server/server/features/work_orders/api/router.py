from fastapi import APIRouter

router = APIRouter(prefix="/work-orders", tags=["work_orders"])


@router.get("/health")
def work_orders_health() -> dict[str, str]:
    return {"feature": "work_orders", "status": "ok"}
