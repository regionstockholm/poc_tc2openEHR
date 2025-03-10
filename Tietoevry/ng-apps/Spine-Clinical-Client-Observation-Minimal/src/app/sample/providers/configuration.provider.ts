import { Injectable } from '@angular/core';
import { RestClient, RestRequest } from '@spine/http';
import { Observable, of } from 'rxjs';
import { HostIntegrationService } from '@spine/host-integration';

@Injectable()
export class ConfigurationProvider {
  constructor(private readonly restClient: RestClient, private hostAppService: HostIntegrationService,) { }

  public getConfiguration<T>(oid: string): Observable<T> {
    if (this.hostAppService.isIntegrable) {
      const restRequest = new RestRequest('infrastructure',
        'configuration-service', 1, `/query/api/v1/environment/${oid}`);

      return this.restClient.get<T>(restRequest);
    } else {
      return of({
        "rid": 13,
        "Oid": "spine.infrastructure.betterplatform.configuration",
        "TenantId": "e8f84760-3bfd-47d5-8af0-78ae4ba0590b",
        "Created": "2024-03-29T12:46:56.397405Z",
        "Modified": "2024-03-29T12:46:56.465454Z",
        "ConfigData": {
          "AuthenticationType": "basic", //"oauth2",//,
          "UserName": "admin",
          "Password": "Y5nP2kW7Zq3MmX6",
          "EndpointUrl": "https://open-platform-migration.service.tietoevry.com/ehr/rest/v1",
          "EndpointAdminUrl": "https://open-platform-migration.service.tietoevry.com"
        } as any
      } as any);
    }
  }

}
