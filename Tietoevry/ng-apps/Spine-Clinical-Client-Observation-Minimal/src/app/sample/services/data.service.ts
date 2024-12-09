import { Injectable } from '@angular/core';
import { HostIntegrationService } from '@spine/host-integration';
import { HttpClient, IHttpOptions } from "@spine/http";
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private baseUrl = 'https://open-platform-migration.service.tietoevry.com/ehr/rest/v1/view/';
  private access_token = 'Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJBYS1LWU9WVUJhdzNpMzJxOGpXMWhqRDJwVXNxcEw1S1hma0hSVndhUnEwIn0.eyJleHAiOjE3MzI1MTgwMDMsImlhdCI6MTczMjUxNjgwMywiYXV0aF90aW1lIjoxNzMyNTE2ODAxLCJqdGkiOiI2MWIyMTgwMi05YWIwLTQ3ZjMtOWQ2NS1hMWI0ZmRiNmU1MTUiLCJpc3MiOiJodHRwczovL29wZW4tcGxhdGZvcm0tZGV2LnNlcnZpY2UudGlldG9ldnJ5LmNvbS9zcGluZS1pZHAvcmVhbG1zL2RldiIsImF1ZCI6WyJ0ZXJtaW5vbG9neSIsImVoci1zZXJ2ZXIiLCJ2YWxpZGF0b3IiLCJicm9rZXIiLCJlaHIiLCJzbWFydCIsImFiYWMiLCJhY2NvdW50IiwiZGVtb2dyYXBoaWNzIl0sInN1YiI6Ijk3MGUyYTc2LTg2MmEtNGE0ZS1hMTE0LWE5MjQ2MTU5YWY5ZiIsInR5cCI6IkJlYXJlciIsImF6cCI6IndlYmZ4Iiwibm9uY2UiOiJPSE41ZURCUlZsWlhSbWRPVmxCa2NYUjROemQ1T0doWVZsSk9XbGhZTTBOTVlURjFhbXN6YzNFMmIyeDUiLCJzZXNzaW9uX3N0YXRlIjoiMTZmZWM0OTctNTgxOC00MThjLWFmZWItOTg5N2RiMWZhYTdkIiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6WyIqIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJkZWZhdWx0LXJvbGVzLWRldiIsIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJ0ZXJtaW5vbG9neSI6eyJyb2xlcyI6WyJST0xFX1JFQUQiXX0sImVoci1zZXJ2ZXIiOnsicm9sZXMiOlsidW1hX3Byb3RlY3Rpb24iXX0sInZhbGlkYXRvciI6eyJyb2xlcyI6WyJWQUxJREFUT1JfU0NSSVBUX0FETUlOIiwiVkFMSURBVE9SX0FUVFJJQlVURV9BRE1JTiIsIlZBTElEQVRPUl9BVVRIRU5USUNBVElPTl9ERVRBSUxfQURNSU4iLCJWQUxJREFUT1JfQURNSU4iXX0sImJyb2tlciI6eyJyb2xlcyI6WyJyZWFkLXRva2VuIl19LCJlaHIiOnsicm9sZXMiOlsiUk9MRV9QUkVTRU5UQVRJT04iLCJST0xFX1ZJRVciLCJST0xFX1dSSVRFIiwiUk9MRV9WSUVXX0FETUlOIiwiUk9MRV9XUklURV9FSFIiLCJST0xFX0ZJTEVfU1RPUkUiLCJST0xFX1VTRVJfQURNSU4iLCJST0xFX1dSSVRFX0dFTkVSQVRFRCIsIlJPTEVfUVVFUlkiLCJST0xFX0FETUlOIiwiUk9MRV9FVkVOVF9BRE1JTiIsIlJPTEVfVEVNUExBVEVfQURNSU4iLCJST0xFX1JFQUQiLCJST0xFX0ZPUk1fQURNSU4iXX0sInNtYXJ0Ijp7InJvbGVzIjpbIlJPTEVfU01BUlQiXX0sImFiYWMiOnsicm9sZXMiOlsiUk9MRV9BRE1JTiJdfSwiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19LCJkZW1vZ3JhcGhpY3MiOnsicm9sZXMiOlsiREVNT0dSQVBISUNTX1NFQVJDSCIsIkRFTU9HUkFQSElDU19BRE1JTiIsIkRFTU9HUkFQSElDU19XUklURSIsIkRFTU9HUkFQSElDU19BRE1JTl9MT0NBVElPTl9ISUVSQVJDSFkiLCJERU1PR1JBUEhJQ1NfUkVBRCIsIkRFTU9HUkFQSElDU19BRE1JTl9URU5BTlQiLCJERU1PR1JBUEhJQ1NfRVhURU5ERURfT1BFUkFUSU9OIiwiREVNT0dSQVBISUNTX0FETUlOX0lOREVYIl19fSwic2NvcGUiOiJzY29wZV9ncm91cF9hdHRyaWJ1dGUgZW1haWwgb3BlbmlkIG9mZmxpbmVfYWNjZXNzIHNjb3BlX3VzZXJfYXR0cmlidXRlcyBwcm9maWxlIiwic2lkIjoiMTZmZWM0OTctNTgxOC00MThjLWFmZWItOTg5N2RiMWZhYTdkIiwiY2FyZV91bml0X25hbWUiOiJIw6Rsc29jZW50cmFsZW4gU8O2ZGVydG9yZ2V0IiwicHJhY3RpdGlvbmVyX3JvbGVfbmFtZSI6IkzDpGthcmUgQWt1dCBOU0MiLCJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsIm9yZ2FuaXphdGlvbiI6IlNFNTU2MDUyNzQ2Ni0xMDAyIiwibmFtZSI6IlRpZXRvIFRlc3QgdXNlciIsImNhcmVfdW5pdCI6IlNFMjMyMTAwMDA0MC00SkswIiwicHJlZmVycmVkX3VzZXJuYW1lIjoidGlldG8iLCJvcmdhbml6YXRpb25fbmFtZSI6IlJlZ2lvbiBTw7ZkZXJib3R0ZW4iLCJnaXZlbl9uYW1lIjoiVGlldG8iLCJmYW1pbHlfbmFtZSI6IlRlc3QgdXNlciIsInByYWN0aXRpb25lcl9yb2xlIjoiU0UyMzIxMDAwMDQwLTROMlYiLCJlbWFpbCI6InRpZXRvQGdtYWlsLmNvbSJ9.C_2SFwzWm7O6KO1sh2jyMSk2rn9Hr2xrBcd0k-RamCY3SHbGBHeOiFbtTqjTKhACEZbIahCyT98WaAq1tfd8behlsoPZ554IxNltUMiO5SM4ZJiW4Bzh7MDMnLxCcllYGoB7y4VhDo_P_YebyiPOs9sEnFKFCYANf11dltHUuj09qg8KcVYPGcvcjQGlK1ze_u0vIZ_32o2vySr2pksi_AdaAT5f2gLjBmcz_8gvG1EgVSsfnXCAVrLsGx1Jj0Q07hG1cZ2a0xK4qFKE2leGtsGMIrjIQS4f25ieX6IrICIfC7pCJZkC8WP4A9N8eISIv-iwiL_imE_I_7F6z5n1bg';
  
  constructor(private http: HttpClient, private hostAppService: HostIntegrationService) { }

  getMeasurements(ehrId: string, filter: any): Observable<any> {
    const measurementsView = '/RSK.View.Measurements.Data';
    if (this.hostAppService.isIntegrable) {
      console.log('is integrated data')
      const httpOptions: IHttpOptions = {
        headers: new Headers({ 'Content-Type': 'application/json',
            'authorization': this.access_token
         })
      };
      const apiUrl = `${this.baseUrl}${ehrId}${measurementsView}?fromDate=${filter.fromDate}&toDate=${filter.toDate}`;
      return this.http.get<any>(apiUrl, httpOptions);
    } else {
        // return of([]);
      console.log('not integrated data')
      return of([
        {
          "CompositionId": "e0eaa788-4104-4b3b-8e9a-55d19b21cd00::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "402",
          "Name": "Andningsfrekvens",
          "Value": {
            "@class": "DV_QUANTITY",
            "magnitude": 16,
            "units": "/min"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-11-11T16:16:13Z",
          "DateTime": "2024-11-11T08:00:00Z"
        },
        {
            "CompositionId": "e0eaa788-4104-4b3b-8e9a-55d19b21cd00::default::1",
            "Dokumentnamn": "Mätningar",
            "Dokumentationskod": "MÄT",
            "Id": "402",
            "Name": "Andningsfrekvens",
            "Value": {
              "@class": "DV_QUANTITY",
              "magnitude": 18,
              "units": "/min"
            },
            "Kommetar": null,
            "SkalaText": null,
            "Dokumentationstidpunkt": "2024-11-11T18:16:13Z",
            "DateTime": "2024-11-11T18:00:00Z"
          },
        {
          "CompositionId": "c9891783-4848-450b-a78b-54cf0a3cbe63::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "402",
          "Name": "Andningsfrekvens",
          "Value": {
            "@class": "DV_QUANTITY",
            "magnitude": 20,
            "units": "/min"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-11-14T09:07:19Z",
          "DateTime": "2024-11-13T08:00:00Z"
        },
        {
          "CompositionId": "e0eaa788-4104-4b3b-8e9a-55d19b21cd00::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "3720",
          "Name": "Blodtryck diastoliskt - nedre",
          "Value": {
            "@class": "DV_QUANTITY",
            "magnitude": 80,
            "units": "mm[Hg]"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-11-11T16:16:13Z",
          "DateTime": "2024-11-11T08:00:00Z"
        },
        {
          "CompositionId": "c9891783-4848-450b-a78b-54cf0a3cbe63::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "3720",
          "Name": "Blodtryck diastoliskt - nedre",
          "Value": {
            "@class": "DV_QUANTITY",
            "magnitude": 80,
            "units": "mm[Hg]"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-11-14T09:07:19Z",
          "DateTime": "2024-11-13T08:00:00Z"
        },
        {
          "CompositionId": "e0eaa788-4104-4b3b-8e9a-55d19b21cd00::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "3719",
          "Name": "Blodtryck systoliskt - övre",
          "Value": {
            "@class": "DV_QUANTITY",
            "magnitude": 160,
            "units": "mm[Hg]"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-11-11T16:16:13Z",
          "DateTime": "2024-11-11T08:00:00Z"
        },
        {
          "CompositionId": "c9891783-4848-450b-a78b-54cf0a3cbe63::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "3719",
          "Name": "Blodtryck systoliskt - övre",
          "Value": {
            "@class": "DV_QUANTITY",
            "magnitude": 140,
            "units": "mm[Hg]"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-11-14T09:07:19Z",
          "DateTime": "2024-11-13T08:00:00Z"
        },
        {
          "CompositionId": "e0eaa788-4104-4b3b-8e9a-55d19b21cd00::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "2025",
          "Name": "Kroppstemperatur",
          "Value": {
            "@class": "DV_QUANTITY",
            "magnitude": 37.5,
            "units": "Cel"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-11-11T16:16:13Z",
          "DateTime": "2024-11-11T08:00:00Z"
        },
        {
          "CompositionId": "c9891783-4848-450b-a78b-54cf0a3cbe63::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "2025",
          "Name": "Kroppstemperatur",
          "Value": {
            "@class": "DV_QUANTITY",
            "magnitude": 38.6,
            "units": "Cel"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-11-14T09:07:19Z",
          "DateTime": "2024-11-13T08:00:00Z"
        },
        {
          "CompositionId": "e0eaa788-4104-4b3b-8e9a-55d19b21cd00::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "1978",
          "Name": "Pulsfrekvens",
          "Value": {
            "@class": "DV_QUANTITY",
            "magnitude": 56,
            "units": "/min"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-11-11T16:16:13Z",
          "DateTime": "2024-11-11T08:00:00Z"
        },
        {
          "CompositionId": "c9891783-4848-450b-a78b-54cf0a3cbe63::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "1978",
          "Name": "Pulsfrekvens",
          "Value": {
            "@class": "DV_QUANTITY",
            "magnitude": 67,
            "units": "/min"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-11-14T09:07:19Z",
          "DateTime": "2024-11-13T08:00:00Z"
        },
          {
              "CompositionId": "c396e4fc-518a-44e0-8b0b-37acfde0666c::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "402",
              "Name": "Andningsfrekvens",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 9,
                  "units": "/min"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-05T16:51:43Z",
              "DateTime": "2024-09-17T16:51:00Z"
          },
          {
              "CompositionId": "deb08f03-52dd-4b77-b902-0ff71f7973c3::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "402",
              "Name": "Andningsfrekvens",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 11,
                  "units": "/min"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-05T16:52:23Z",
              "DateTime": "2024-09-19T12:00:00Z"
          },
          {
              "CompositionId": "c396e4fc-518a-44e0-8b0b-37acfde0666c::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3720",
              "Name": "Blodtryck diastoliskt - nedre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 75,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-05T16:51:43Z",
              "DateTime": "2024-09-17T16:51:00Z"
          },
          {
              "CompositionId": "deb08f03-52dd-4b77-b902-0ff71f7973c3::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3720",
              "Name": "Blodtryck diastoliskt - nedre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 80,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-05T16:52:23Z",
              "DateTime": "2024-09-19T12:00:00Z"
          },
          {
              "CompositionId": "0c39856b-5618-454f-b3f3-4545719c7ae1::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3720",
              "Name": "Blodtryck diastoliskt - nedre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 75,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-18T09:42:58Z",
              "DateTime": "2024-09-19T12:00:00Z"
          },
          {
              "CompositionId": "26087aa2-a7c8-4079-9c1a-bf937fd46b99::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3720",
              "Name": "Blodtryck diastoliskt - nedre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 80,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-18T09:43:58Z",
              "DateTime": "2024-09-21T05:00:00Z"
          },
          {
              "CompositionId": "1c1f4f75-197d-4742-9fb7-e5ce9bd062a8::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3720",
              "Name": "Blodtryck diastoliskt - nedre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 70,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-18T09:43:33Z",
              "DateTime": "2024-09-20T06:00:00Z"
          },
          {
              "CompositionId": "afd1705c-bb60-47a1-9a96-d58dd3905be5::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3720",
              "Name": "Blodtryck diastoliskt - nedre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 70,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-18T09:42:33Z",
              "DateTime": "2024-09-18T01:00:00Z"
          },
          {
              "CompositionId": "c396e4fc-518a-44e0-8b0b-37acfde0666c::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3719",
              "Name": "Blodtryck systoliskt - övre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 122,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-05T16:51:43Z",
              "DateTime": "2024-09-17T16:51:00Z"
          },
          {
              "CompositionId": "0c39856b-5618-454f-b3f3-4545719c7ae1::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3719",
              "Name": "Blodtryck systoliskt - övre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 180,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-18T09:42:58Z",
              "DateTime": "2024-09-19T12:00:00Z"
          },
          {
              "CompositionId": "afd1705c-bb60-47a1-9a96-d58dd3905be5::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3719",
              "Name": "Blodtryck systoliskt - övre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 190,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-18T09:42:33Z",
              "DateTime": "2024-09-18T01:00:00Z"
          },
          {
              "CompositionId": "deb08f03-52dd-4b77-b902-0ff71f7973c3::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3719",
              "Name": "Blodtryck systoliskt - övre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 130,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-05T16:52:23Z",
              "DateTime": "2024-09-19T12:00:00Z"
          },
          {
              "CompositionId": "26087aa2-a7c8-4079-9c1a-bf937fd46b99::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3719",
              "Name": "Blodtryck systoliskt - övre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 165,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-18T09:43:58Z",
              "DateTime": "2024-09-21T05:00:00Z"
          },
          {
              "CompositionId": "1c1f4f75-197d-4742-9fb7-e5ce9bd062a8::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "3719",
              "Name": "Blodtryck systoliskt - övre",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 160,
                  "units": "mm[Hg]"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-18T09:43:33Z",
              "DateTime": "2024-09-20T06:00:00Z"
          },
          {
              "CompositionId": "c396e4fc-518a-44e0-8b0b-37acfde0666c::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "2025",
              "Name": "Kroppstemperatur",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 37.3,
                  "units": "Cel"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-05T16:51:43Z",
              "DateTime": "2024-09-17T16:51:00Z"
          },
          {
              "CompositionId": "deb08f03-52dd-4b77-b902-0ff71f7973c3::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "2025",
              "Name": "Kroppstemperatur",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 37.5,
                  "units": "Cel"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-05T16:52:23Z",
              "DateTime": "2024-09-19T12:00:00Z"
          },
          {
              "CompositionId": "63c6ba4a-dc64-4d74-97ce-ec0031238b88::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "9366",
              "Name": "Perifer venkateter (PVK) 1 In/Ut/Byte",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "1"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:36:55Z",
              "DateTime": "2024-09-17T14:00:00Z"
          },
          {
              "CompositionId": "6ebc4aac-d4f6-4add-bfb5-06cb66662e08::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "9366",
              "Name": "Perifer venkateter (PVK) 1 In/Ut/Byte",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "0"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:37:41Z",
              "DateTime": "2024-09-17T16:30:00Z"
          },
          {
              "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "9366",
              "Name": "Perifer venkateter (PVK) 1 In/Ut/Byte",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "0"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
              "DateTime": "2024-09-17T07:30:00Z"
          },
          {
              "CompositionId": "c396e4fc-518a-44e0-8b0b-37acfde0666c::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "1978",
              "Name": "Pulsfrekvens",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 65,
                  "units": "/min"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-05T16:51:43Z",
              "DateTime": "2024-09-17T16:51:00Z"
          },
          {
              "CompositionId": "deb08f03-52dd-4b77-b902-0ff71f7973c3::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "1978",
              "Name": "Pulsfrekvens",
              "Value": {
                  "@class": "DV_QUANTITY",
                  "magnitude": 70,
                  "units": "/min"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-11-05T16:52:23Z",
              "DateTime": "2024-09-19T12:00:00Z"
          },
          {
              "CompositionId": "6ebc4aac-d4f6-4add-bfb5-06cb66662e08::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "11519",
              "Name": "PVK 1, placering",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "2"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:37:41Z",
              "DateTime": "2024-09-17T16:30:00Z"
          },
          {
              "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "11519",
              "Name": "PVK 1, placering",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "0"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
              "DateTime": "2024-09-17T07:30:00Z"
          },
          {
              "CompositionId": "6ebc4aac-d4f6-4add-bfb5-06cb66662e08::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "11520",
              "Name": "PVK 1, sida",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "1"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:37:41Z",
              "DateTime": "2024-09-17T16:30:00Z"
          },
          {
              "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "11520",
              "Name": "PVK 1, sida",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "0"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
              "DateTime": "2024-09-17T07:30:00Z"
          },
          {
              "CompositionId": "6ebc4aac-d4f6-4add-bfb5-06cb66662e08::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "11521",
              "Name": "PVK 1, stickförsök",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "0"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:37:41Z",
              "DateTime": "2024-09-17T16:30:00Z"
          },
          {
              "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "11521",
              "Name": "PVK 1, stickförsök",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "0"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
              "DateTime": "2024-09-17T07:30:00Z"
          },
          {
              "CompositionId": "6ebc4aac-d4f6-4add-bfb5-06cb66662e08::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "11518",
              "Name": "PVK 1, storlek",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "3"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:37:41Z",
              "DateTime": "2024-09-17T16:30:00Z"
          },
          {
              "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
              "Dokumentnamn": "Mätningar",
              "Dokumentationskod": "MÄT",
              "Id": "11518",
              "Name": "PVK 1, storlek",
              "Value": {
                  "@class": "DV_TEXT",
                  "value": "4"
              },
              "Kommetar": null,
              "SkalaText": null,
              "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
              "DateTime": "2024-09-17T07:30:00Z"
          },
          {
            "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
            "Dokumentnamn": "Mätningar",
            "Dokumentationskod": "MÄT",
            "Id": "11518",
            "Name": "PVK 1, storlek",
            "Value": {
                "@class": "DV_TEXT",
                "value": "4"
            },
            "Kommetar": null,
            "SkalaText": null,
            "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
            "DateTime": "2024-09-22T07:30:00Z"
        }
        ,
          {
            "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
            "Dokumentnamn": "Mätningar",
            "Dokumentationskod": "MÄT",
            "Id": "11519",
            "Name": "PVK 1, placering",
            "Value": {
                "@class": "DV_TEXT",
                "value": "4"
            },
            "Kommetar": null,
            "SkalaText": null,
            "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
            "DateTime": "2024-09-23T07:30:00Z"
        },
        {
          "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "11519",
          "Name": "PVK 1, placering",
          "Value": {
              "@class": "DV_TEXT",
              "value": "4"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
          "DateTime": "2024-09-24T07:30:00Z"
      }
      ,
        {
          "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
          "Dokumentnamn": "Mätningar",
          "Dokumentationskod": "MÄT",
          "Id": "11519",
          "Name": "PVK 1, placering",
          "Value": {
              "@class": "DV_TEXT",
              "value": "4"
          },
          "Kommetar": null,
          "SkalaText": null,
          "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
          "DateTime": "2024-09-25T07:30:00Z"
      },
      {
        "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
        "Dokumentnamn": "Mätningar",
        "Dokumentationskod": "MÄT",
        "Id": "11519",
        "Name": "PVK 1, placering",
        "Value": {
            "@class": "DV_TEXT",
            "value": "4"
        },
        "Kommetar": null,
        "SkalaText": null,
        "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
        "DateTime": "2024-09-26T07:30:00Z"
    },
    {
        "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
        "Dokumentnamn": "Mätningar",
        "Dokumentationskod": "MÄT",
        "Id": "11519",
        "Name": "PVK 1, placering",
        "Value": {
            "@class": "DV_TEXT",
            "value": "4"
        },
        "Kommetar": null,
        "SkalaText": null,
        "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
        "DateTime": "2024-09-27T07:30:00Z"
    },
    {
        "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
        "Dokumentnamn": "Mätningar",
        "Dokumentationskod": "MÄT",
        "Id": "11519",
        "Name": "PVK 1, placering",
        "Value": {
            "@class": "DV_TEXT",
            "value": "4"
        },
        "Kommetar": null,
        "SkalaText": null,
        "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
        "DateTime": "2024-09-28T07:30:00Z"
    },
    {
        "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
        "Dokumentnamn": "Mätningar",
        "Dokumentationskod": "MÄT",
        "Id": "11519",
        "Name": "PVK 1, placering",
        "Value": {
            "@class": "DV_TEXT",
            "value": "4"
        },
        "Kommetar": null,
        "SkalaText": null,
        "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
        "DateTime": "2024-09-29T07:30:00Z"
    },
    {
        "CompositionId": "3056f03b-01d8-4a35-a65d-a4faeccad001::default::1",
        "Dokumentnamn": "Mätningar",
        "Dokumentationskod": "MÄT",
        "Id": "11519",
        "Name": "PVK 1, placering",
        "Value": {
            "@class": "DV_TEXT",
            "value": "4"
        },
        "Kommetar": null,
        "SkalaText": null,
        "Dokumentationstidpunkt": "2024-09-27T10:36:08Z",
        "DateTime": "2023-09-29T07:30:00Z"
    }
      ]);
    }
  }

  getChemistryData(ehrId: string, filter: any): Observable<any> {
    const view = '/RSK.View.Chemistry.Data';
    if (this.hostAppService.isIntegrable) {
        console.log('is integrated data')
        const httpOptions: IHttpOptions = {
          headers: new Headers({ 'Content-Type': 'application/json',
              'authorization': this.access_token
           })
        };
        const apiUrl = `${this.baseUrl}${ehrId}${view}?fromDate=${filter.fromDate}&toDate=${filter.toDate}`;
        return this.http.get<any>(apiUrl, httpOptions);
      } else {
        return of([
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU01960",
                "Name": "B-Erytrocyter",
                "Value": 4.8,
                "Unit": "x10(12)/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU01960",
                "Name": "B-Erytrocyter",
                "Value": 3.5,
                "Unit": "x10(12)/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU01961",
                "Name": "B-EVF",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU01961",
                "Name": "B-EVF",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU27300",
                "Name": "B-HbA1c (IFCC)",
                "Value": 42,
                "Unit": "mmol/mol",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU28309",
                "Name": "B-Hemoglobin",
                "Value": 118,
                "Unit": "g/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU28309",
                "Name": "B-Hemoglobin",
                "Value": 111,
                "Unit": "g/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU02593",
                "Name": "B-Leukocyter",
                "Value": 4,
                "Unit": "x10(9)/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU02593",
                "Name": "B-Leukocyter",
                "Value": 4.1,
                "Unit": "x10(9)/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU03404",
                "Name": "B-SR",
                "Value": 27,
                "Unit": "mm",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU03568",
                "Name": "B-Trombocyter",
                "Value": 165,
                "Unit": "x10(9)/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU03568",
                "Name": "B-Trombocyter",
                "Value": 139,
                "Unit": "x10(9)/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU26880",
                "Name": "Erc(B)-MCH",
                "Value": 32,
                "Unit": "pg",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU26880",
                "Name": "Erc(B)-MCH",
                "Value": 32,
                "Unit": "pg",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU01944",
                "Name": "Erc(B)-MCV",
                "Value": 97,
                "Unit": "fL",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU01944",
                "Name": "Erc(B)-MCV",
                "Value": 85,
                "Unit": "fL",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU03620",
                "Name": "fP-Triglycerid",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "NPU12520",
                "Name": "kB-BaseExess(st)",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00450",
                "Name": "kB-Calciumjon, fri",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00109",
                "Name": "kB-COHb",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00301",
                "Name": "kB-Hemoglobin",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00810",
                "Name": "kB-Kalium",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX01790",
                "Name": "kB-Klorid",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00344",
                "Name": "kB-MetHb",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00809",
                "Name": "kB-Natrium",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "NPU12481",
                "Name": "kB-pCO2",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "NPU12490",
                "Name": "kB-pH",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "NPU12500",
                "Name": "kB-pO2",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "NPU10197",
                "Name": "kB-sO2",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00474",
                "Name": "kB-St.bikarbonat",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19981",
                "Name": "P-ALAT",
                "Value": 28.3,
                "Unit": "mikrokat/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU19673",
                "Name": "P-Albumin",
                "Value": 39,
                "Unit": "g/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19673",
                "Name": "P-Albumin",
                "Value": 23,
                "Unit": "g/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU53078",
                "Name": "P-ALP (Alk fosfatas)",
                "Value": 2.2,
                "Unit": "mikrokat/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU54111",
                "Name": "P-Antitrombin(Tromb)",
                "Value": 0.18,
                "Unit": "kIE/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU01682",
                "Name": "P-APT-tid",
                "Value": 42,
                "Unit": "s",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU22279",
                "Name": "P-ASAT",
                "Value": 15.2,
                "Unit": "mikrokat/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU01370",
                "Name": "P-Bilirubin",
                "Value": 363,
                "Unit": "mikromol/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "FLX00438",
                "Name": "P-Ca-fosfatprodukt",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU01443",
                "Name": "P-Calcium",
                "Value": 2.38,
                "Unit": "mmol/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU01443",
                "Name": "P-Calcium",
                "Value": 2.28,
                "Unit": "mmol/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "FLX00437",
                "Name": "P-Calcium, alb korr",
                "Value": 1.98,
                "Unit": "mmol/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU19748",
                "Name": "P-CRP",
                "Value": 3,
                "Unit": "mg/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19748",
                "Name": "P-CRP",
                "Value": 23,
                "Unit": "mg/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU23745",
                "Name": "P-Cystatin C",
                "Value": 0.94,
                "Unit": "mg/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU28289",
                "Name": "P-Fibrin-D-Dimer",
                "Value": 17.7,
                "Unit": "mg/L FEU",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19768",
                "Name": "P-Fibrinogen (koag)",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU03096",
                "Name": "P-Fosfat",
                "Value": 2.6,
                "Unit": "mmol/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU02192",
                "Name": "P-Glukos",
                "Value": 8.7,
                "Unit": "mmol/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU22283",
                "Name": "P-GT",
                "Value": 1.5,
                "Unit": "mikrokat/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU02508",
                "Name": "P-Järn",
                "Value": 26,
                "Unit": "mikromol/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU03230",
                "Name": "P-Kalium",
                "Value": 4.5,
                "Unit": "mmol/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU03230",
                "Name": "P-Kalium",
                "Value": 3.6,
                "Unit": "mmol/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU01566",
                "Name": "P-Kolesterol",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU04998",
                "Name": "P-Kreatinin",
                "Value": 69,
                "Unit": "mikromol/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU04998",
                "Name": "P-Kreatinin",
                "Value": 50,
                "Unit": "mikromol/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU02647",
                "Name": "P-Magnesium",
                "Value": 0.83,
                "Unit": "mmol/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU03429",
                "Name": "P-Natrium",
                "Value": 144,
                "Unit": "mmol/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU03429",
                "Name": "P-Natrium",
                "Value": 135,
                "Unit": "mmol/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU01685",
                "Name": "P-PK(INR)",
                "Value": 3,
                "Unit": "INR",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "37beb264-575d-4956-ba78-dfe99e544620::default::1",
                "Id": "NPU01685",
                "Name": "P-PK(INR)",
                "Value": 2.8,
                "Unit": "INR",
                "DateTime": "2024-10-31T09:23:11Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU26470",
                "Name": "P-Transferrin",
                "Value": 1.19,
                "Unit": "g/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU04191",
                "Name": "P-Transferrinmättnad",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU27501",
                "Name": "P-Troponin T",
                "Value": 12,
                "Unit": "nanog/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU01459",
                "Name": "P-Urea",
                "Value": 2.45,
                "Unit": "mmol/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU01459",
                "Name": "P-Urea",
                "Value": 1.2,
                "Unit": "mmol/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "SWE05405",
                "Name": "Pt-eGFR(CysC)relativ",
                "Value": 87,
                "Unit": "mL/min/1.7",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "SWE05406",
                "Name": "Pt-eGFR(Krea)relativ",
                "Value": 68,
                "Unit": "mL/min/1.7",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "SWE05406",
                "Name": "Pt-eGFR(Krea)relativ",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "FLX01836",
                "Name": "S-Albumin",
                "Value": 24,
                "Unit": "g/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19692",
                "Name": "S-Alfa-1-antitrypsin",
                "Value": 1.4,
                "Unit": "g/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19766",
                "Name": "S-alfa-Fetoprotein",
                "Value": 63,
                "Unit": "mikrog/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU01450",
                "Name": "S-CA 19-9",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19719",
                "Name": "S-CEA",
                "Value": 1.9,
                "Unit": "mikrog/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "FLX01837",
                "Name": "S-CRP",
                "Value": 22,
                "Unit": "mg/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19788",
                "Name": "S-Haptoglobin",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19795",
                "Name": "S-Immunglobulin A",
                "Value": 6.8,
                "Unit": "g/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19814",
                "Name": "S-Immunglobulin G",
                "Value": 37.6,
                "Unit": "g/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19825",
                "Name": "S-Immunglobulin M",
                "Value": 0.94,
                "Unit": "g/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "NPU19873",
                "Name": "S-Orosomukoid",
                "Value": 0.39,
                "Unit": "g/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "FLX00804",
                "Name": "S-Proteinfraktioner",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "f12cb6e2-0ac0-46c8-afaa-f39693997763::default::1",
                "Id": "FLX00779",
                "Name": "S-TSH",
                "Value": 0.08,
                "Unit": "mE/L",
                "DateTime": "2024-10-18T14:50:04Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU19677",
                "Name": "U-Albumin",
                "Value": 24,
                "Unit": "mg/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU28842",
                "Name": "U-Albumin/Krea",
                "Value": 5.4,
                "Unit": "mg/mmol",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "9c06ec30-4175-4ce4-8c4c-4a0aef08b858::default::1",
                "Id": "NPU09102",
                "Name": "U-Kreatinin",
                "Value": 3.6,
                "Unit": "mmol/L",
                "DateTime": "2024-11-04T07:23:06Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "NPU12521",
                "Name": "vB-BaseExess(st)",
                "Value": 4,
                "Unit": "mmol/L",
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX01741",
                "Name": "vB-Calciumjon, fri",
                "Value": 1.24,
                "Unit": "mmol/L",
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00288",
                "Name": "vB-COHb",
                "Value": 1.1,
                "Unit": "%",
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00285",
                "Name": "vB-Hemoglobin",
                "Value": 135,
                "Unit": "g/L",
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00812",
                "Name": "vB-Kalium",
                "Value": 4.4,
                "Unit": "mmol/L",
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX01791",
                "Name": "vB-Klorid",
                "Value": 103,
                "Unit": "mmol/L",
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00289",
                "Name": "vB-MetHb",
                "Value": 1.2,
                "Unit": "%",
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00811",
                "Name": "vB-Natrium",
                "Value": 139,
                "Unit": "mmol/L",
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "NPU10029",
                "Name": "vB-pCO2",
                "Value": 6.4,
                "Unit": "kPa",
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "NPU03995",
                "Name": "vB-pH",
                "Value": null,
                "Unit": null,
                "DateTime": "2024-10-22T09:24:18Z"
            },
            {
                "CompositionId": "a8d229bf-5dbb-40ef-8aa1-f971d845f15c::default::1",
                "Id": "FLX00360",
                "Name": "vB-St.bikarbonat",
                "Value": 27,
                "Unit": "mmol/L",
                "DateTime": "2024-10-22T09:24:18Z"
            }
        ]);
      }
  }
}