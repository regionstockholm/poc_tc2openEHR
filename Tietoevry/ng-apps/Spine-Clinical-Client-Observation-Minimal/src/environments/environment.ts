// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

const host = 'https://open-platform-migration.service.tietoevry.com';
const port = '8083';

export const environment = {
  production: false,
  HostUrl: host,
  discoveryRoute: '/web-discovery-query/api/v1/discovery/web',
  BetterPlatformUrl: `${host}:${port}/ehr/rest/v1`,
  Contract: 'c8550cac-da52-4053-9161-d8e9ff9e5f6a'
};
/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
