## Development server

Run `npm run start` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Build

Run `npm run build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `npm run test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `npm run e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Image Build Comands

`docker build -f Dockerfile -t client-observation-minimal:6.2.0-beta31 .`

`docker login harbor.service.tieto.com`

`docker tag docker.io/client-observation-minimal:6.2.0-beta31 harbor.service.tieto.com/spine/client-observation-minimal:6.2.0-beta31`

`docker push harbor.service.tieto.com/spine/client-observation-minimal:6.2.0-beta31`

## Kubernetes installation command

`helm upgrade --install client-observation-minimal .\ng-apps\Spine-Clinical-Client-Observation-Minimal\helm_client_observation_minimal -f .\ng-apps\Spine-Clinical-Client-Observation-Minimal\helm_client_observation_minimal\environments\open-platform-migration-values.yaml  -n spine --debug`

