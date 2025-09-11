# Posiziona questo file in ~/Desktop/LifeSuite/Makefile

.PHONY: help build up down logs clean dev prod restart

help:
	@echo "LifeSuite Docker Commands:"
	@echo "  make build     - Build all images"
	@echo "  make dev       - Start development environment"
	@echo "  make up        - Start production environment"
	@echo "  make down      - Stop all services"
	@echo "  make logs      - View all logs"
	@echo "  make clean     - Clean everything"
	@echo "  make restart   - Restart all services"

build:
	docker-compose build

dev:
	docker-compose -f docker-compose.yml -f docker-compose.dev.yml up

up:
	docker-compose up -d

down:
	docker-compose down

logs:
	docker-compose logs -f

clean:
	docker-compose down -v
	docker system prune -f
	docker image prune -f

restart:
	docker-compose restart

# Rebuild specific services
rebuild-gateway:
	docker-compose build api-gateway
	docker-compose up api-gateway

rebuild-budget:
	docker-compose build budget-manager
	docker-compose up budget-manager