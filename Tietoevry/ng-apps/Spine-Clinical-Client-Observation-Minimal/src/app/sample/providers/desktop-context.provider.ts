import { Observable, map, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { HostIntegrationService, ContextService } from '@spine/host-integration';
import { IUser } from '../models/user.model';
import { IPatient } from '../models/patient.model';

@Injectable({
    providedIn: 'root'
})
export class DesktopContextProvider {

    constructor(
        private hostAppService: HostIntegrationService,
        private readonly contextService: ContextService,
    ) { }

    public getPatient(): Observable<IPatient> {
        if (this.hostAppService.isIntegrable) {
            return this.contextService.getContextEntityAsync('patient').pipe(
                map(patient => {
                    return this.mapPatientData(patient);
                })
            );
            } else {
                return of({ ssn: '1502721-2347', name: 'Elin Lilja', rId: 1, externalId: 'f8ab19e8-53f5-4e45-95cc-08841f5d1df9' } as IPatient);
            }
    }

    public getUser(): Observable<IUser> {
        if (this.hostAppService.isIntegrable) {
            return this.contextService.getContextEntityAsync('user').pipe(
                map(user => {
                    return this.mapUserData(user);
                })
            );
        } else {
            return of({
                "userName": "Tieto Tieto",
                "family": "Tieto",
                "userId": "0030dcb1-94fa-41e1-b28a-52a8d93b901d",
                "userRId": "",
                "ssn": "",
                "roleVersionRid": "",
                "organizationId": "SE5560527466-1002",
                "organizationDescription": "Region Söderbotten",
                "careUnitId": "SE2321000040-4JK0",
                "careUnitDescription": "Hälsocentralen Södertorget",
                "functionaryId": "",
                "userGroup": ""
            } as unknown as IUser);
        }
    }

    private mapPatientData(patient: any): IPatient {
        const patientData: IPatient = { ssn: '', name: '', rId: 0, externalId: '' };

        if (patient) {
            patientData.name = patient.name ? patient.name[0] && patient.name[0].text ? patient.name[0].text : ''
                : patient.Forenames + patient.Surname;

            patientData.ssn = patient.identifier ? (patient.identifier[0] && patient.identifier[0].value ? patient.identifier[0].value : '') : patient.Ssn;
            patientData.rId = patient.identifier ? (patient.identifier[1] && patient.identifier[1].value ? patient.identifier[1].value : '') : patient.Id;
            patientData.externalId = patient.identifier ? (patient.identifier[2] && patient.identifier[2].value ? patient.identifier[2].value : '') : (patient.Extensions && patient.Extensions.ExternalId ? patient.Extensions.ExternalId : '');
        }
        return patientData;
    }

    private mapUserData(user: any): IUser {
        const userData: IUser = ({});

        if (user) {
            if (user.name) {
                userData.userName = user.name.text ? user.name.text : '';
                userData.family = user.name.family ? user.name.family[0] : '';
            }
            if (user.identifier) {
                this.getUserIdentifications(user.identifier, userData);
            }
            if (user.extras) {
                userData.roleVersionRid =
                    user.extras.role && user.extras.role.versionRid
                        ? user.extras.role.versionRid
                        : '';
                if (user.extras.organization) {
                    userData.organizationId = user.extras.organization.id
                        ? user.extras.organization.id
                        : '';
                    userData.organizationDescription = user.extras.organization
                        .description
                        ? user.extras.organization.description
                        : '';
                }
                if (user.extras.careUnit) {
                    userData.careUnitId = user.extras.careUnit.id
                        ? user.extras.careUnit.id
                        : '';
                    userData.careUnitDescription = user.extras.careUnit.description
                        ? user.extras.careUnit.description
                        : '';
                }
                if (user.extras.functionaryId) {
                    userData.functionaryId = user.extras.functionaryId.id
                        ? user.extras.functionaryId.id
                        : '';
                }
                if (user.extras.userGroup) {
                    userData.userGroup = user.extras.userGroup.id
                        ? user.extras.userGroup.id
                        : '';
                }
            }
        }

        return userData;
    }

    private getUserIdentifications(identifications: any, userData: IUser) {
        if (!identifications) return;
        identifications.forEach((identification: any) => {
            if (identification.system === 'id') {
                userData.userId = identification.value;
            }
            if (identification.system === 'rid') {
                userData.userRId = identification.value;
            }
            if (identification.system === 'ssn') {
                userData.ssn = identification.value;
            }
        });
    }

}