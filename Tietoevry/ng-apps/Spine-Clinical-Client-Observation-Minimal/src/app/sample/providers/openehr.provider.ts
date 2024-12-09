import { Location } from "@angular/common";
import { Injectable } from '@angular/core';
import { HttpClient, IHttpOptions } from "@spine/http";
import { Observable, of } from 'rxjs';
import { TokenProvider } from './token.provider';
import { environment } from 'src/environments/environment';
import { HostIntegrationService } from '@spine/host-integration';

@Injectable({
    providedIn: 'root'
})
export class OpenEhrProvider {

    constructor(private readonly httpClient: HttpClient, private readonly tokenProvider: TokenProvider,
        private hostAppService: HostIntegrationService,
    ) { }

    public getEhrId(externalId: string): Observable<any> {
        if (this.hostAppService.isIntegrable) {
            const httpOptions: IHttpOptions = {
                headers: new Headers({ 'Content-Type': 'application/json' })
            };
            const params = new URLSearchParams({
                subjectId: externalId, //this.tokenProvider.getPatientExternalId(),
                subjectNamespace: 'spine'
            });
            const url = Location.joinWithSlash(environment.BetterPlatformUrl, `/ehr?${params}`);
            return this.httpClient.get(url, httpOptions);
        } else {
            return of({
                    "meta": {
                      "href": "https://open-platform-migration.service.tietoevry.com/api/v1/ehrs/1"
                    },
                    "ehrStatus": {
                      "subjectId": "1",
                      "subjectNamespace": "spine",
                      "queryable": true,
                      "modifiable": true
                    },
                    "ehrId": "b4394eb9-96a4-45ac-a240-e773d69dccf0"
            });
        }
    }

    public createEhrId(externalId: string): Observable<any> {
        if (this.hostAppService.isIntegrable) {
            const httpOptions: IHttpOptions = {
                headers: new Headers({ 'Content-Type': 'application/json' })
            };
            const params = new URLSearchParams({
                subjectId: externalId, //this.tokenProvider.getPatientExternalId(),
                subjectNamespace: 'spine'
            });
            const url = Location.joinWithSlash(environment.BetterPlatformUrl, `/ehr?${params}`);
            return this.httpClient.post(url, null, httpOptions);
        } else {
            return of('2fa153b9-ade7-4514-9b20-b3fdea694c01');
        }
    }

}