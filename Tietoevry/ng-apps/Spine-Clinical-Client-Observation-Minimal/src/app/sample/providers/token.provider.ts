import { Injectable } from "@angular/core";
import { isNullOrUndefined } from "@spine/primitive";
import { IContextData } from "./context.model";

export interface ITokenProvider {
    getToken(): string;
}

// export const TOKEN_INFO = 'TOKEN_INFO';
export const ACCESS_TOKEN = 'access_token';
export const PATIENT_CONTEXT = 'patient_context-1';

@Injectable()
export class TokenProvider implements ITokenProvider {

    public getToken(): string {
        const token = this.getAccessToken();
        if (!isNullOrUndefined(token)) {
            return token;
        }
        throw new Error('Token not found.');
    }

    private getAccessToken(): string {
        let access_token = '';
        const temp = sessionStorage.getItem(ACCESS_TOKEN);
        if (temp) {
            access_token = temp?.toString();
        }
        return access_token;
    }

    private decodeToken(): any {  
        const token = this.getToken();  
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64).split('').map((c) => {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join('')          
            
        );
        return JSON.parse(jsonPayload);
    }

    public getContextData(): IContextData {
        const token = this.decodeToken();
        return {
            userId: token.preferred_username,
            userName: token.name,
            unitId: token.care_unit,
            unitName: token.care_unit_name,
            providerId: token.organization,
            providerName: token.organization_name
        } as IContextData;
    }

    private getPatientContext(): any {
        let patient_context = '';
        const context = sessionStorage.getItem(PATIENT_CONTEXT);
        if (context) {
            patient_context = context?.toString();
        }
        // debugger;
        return patient_context;
    }

    public getPatientExternalId(): string {
        let patientExternalId = '';
        const patientContext = this.getPatientContext();
        if (patientContext) {
            patientExternalId = JSON.parse(patientContext).identifier.find((x: any) => x.system === "externalid").value;
        }
        return patientExternalId;
    }
}