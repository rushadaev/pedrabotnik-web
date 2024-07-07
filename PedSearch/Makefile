# Define variables
IMAGE_NAME = ped-recommendations
CONTAINER_NAME = ped-recommendations-container
PORT = 5001

clean-docker:
	docker system prune -a --volumes -f
# Build the Docker image
build:
	docker build -t $(IMAGE_NAME) .

# Run the Docker container
run:
	docker run -d -p $(PORT):5000 --name $(CONTAINER_NAME) $(IMAGE_NAME)

# Stop the Docker container
stop:
	docker stop $(CONTAINER_NAME) || true
	docker rm $(CONTAINER_NAME) || true

# First start
start: clean-docker build run

# Rebuild the Docker image and run the container
rebuild: stop clean-docker build run

# Show logs from the running container
logs:
	docker logs -f $(CONTAINER_NAME)

# Remove the Docker image
clean:
	docker rmi $(IMAGE_NAME)

# List Docker containers
ps:
	docker ps -a