from fastapi import FastAPI

from server.api.router import api_router
from server.core.lifespan import lifespan

app = FastAPI(title="Repair Dashboard API", lifespan=lifespan)
app.include_router(api_router)


@app.get("/")
def read_root() -> dict[str, str]:
    return {"Hello": "World"}
