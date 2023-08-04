from enum import Enum
from typing import Union, Annotated

from fastapi import FastAPI, Header, status
from pydantic import BaseModel


class ModelName(str, Enum):
    alexnet = "alexnet"
    resnet = "resnet"
    lenet = "lenet"


class Item(BaseModel):
    name: str
    description: Union[str, None] = None


app = FastAPI()


@app.post("/items/", status_code=status.HTTP_201_CREATED)
async def create_item(
        item: Item,
        authorize: Annotated[str | None, Header()] = None,
):
    if authorize is None:
        pass

    return item


@app.get("/models/{model_name}")
async def get_model(model_name: ModelName):
    match model_name:
        case ModelName.alexnet:
            return {"model_name": model_name, "message": "Deep Learning FTW!"}

        case ModelName.lenet:
            return {"model_name": model_name, "message": "LeCNN"}

        case ModelName.resnet:
            return {"model_name": model_name, "message": "Have some residuals"}
