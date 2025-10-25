# AIBridges

## Run the API

```bash
dotnet restore
dotnet run
```

### Run the API with Docker on Windows

To run the API with Docker on Windows, it is recommended to mount a volumes to the container. This avoids the need of downloading the models each time you run the container.

```powershell
docker run -d -p 5234:8080 --mount type=bind,source=C:\VolumesDocker\AIBridges,target=/app/onnx_models erabliereapi/aibridges
```

### Run the API with Docker on Linux

```bash
sudo docker run -d -p 5234:8080 --mount type=bind,source=/var/opt/docker/volumes,target=/app/onnx_models erabliereapi/aibridges
```

## Postman Collection

[Postman Collection - ErabliereAPI Workspace](https://www.postman.com/erabliereapi/erabliereapi/collection/uti5nz7/aibridges?action=share&creator=6202256)

## Supported Plaforms
- Florence2

## Supported Platforms to come
- Azure Vision
- Azure OpenAI Service
- Antropic
- BitNet
- Custom onnx model