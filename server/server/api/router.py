from fastapi import APIRouter, Depends

from server.api.deps import require_api_version
from server.features.customers.api.router import router as customers_router
from server.features.devices.api.router import router as devices_router
from server.features.work_orders.api.router import router as work_orders_router

api_router = APIRouter(prefix="/api", dependencies=[Depends(require_api_version)])
api_router.include_router(customers_router)
api_router.include_router(devices_router)
api_router.include_router(work_orders_router)
