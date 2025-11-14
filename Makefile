SOLUTION=CardValidation.sln
API=src/CardValidation.Web
UNIT=tests/CardValidation.UnitTests
INT=tests/CardValidation.IntegrationTests
ART=artifacts

.PHONY: restore build run test unit int clean docker-build docker-test artifacts

restore:
	dotnet restore $(SOLUTION)

build: restore
	dotnet build -c Release $(SOLUTION)

run:
	dotnet run --project $(API) --urls http://0.0.0.0:5000

unit:
	dotnet test $(UNIT) -c Release \
	  --logger "trx;LogFileName=unit.trx" \
	  /p:CollectCoverage=true \
	  /p:CoverletOutput=$(ART)/coverage/ \
	  /p:CoverletOutputFormat=cobertura

int:
	dotnet run --project $(API) -c Release --urls http://0.0.0.0:5000 & \
	echo $$! > $(ART)/api.pid; \
	sleep 5; \
	dotnet test $(INT) -c Release; \
	kill `cat $(ART)/api.pid` || true

test: unit int

docker-build:
	docker build -f Dockerfile.tests -t card-tests .

docker-test:
	mkdir -p $(ART)
	docker run --rm -p 5000:5000 -v $$PWD/$(ART):/artifacts card-tests

artifacts:
	mkdir -p $(ART)

clean:
	rm -rf $(ART) || true
	dotnet clean $(SOLUTION)
