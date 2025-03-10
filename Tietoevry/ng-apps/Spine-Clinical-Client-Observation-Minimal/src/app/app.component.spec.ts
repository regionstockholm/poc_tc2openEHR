import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateService } from '@ngx-translate/core';
import { AppStateService, HostIntegrationService } from '@spine/host-integration';

import { AppComponent } from './app.component';

describe('AppComponent', () => {

    const mockAppstateSetvice = jasmine.createSpyObj('AppStateService', {
        getApplicationUICulture: {}
    });
    const mockHostIntegrationService = jasmine.createSpyObj('HostIntegrationService', {
        isInitialized: true
    });
    const mockTranslateService = jasmine.createSpyObj('TranslateService', {
        use: {}
    });
    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                RouterTestingModule
            ],
            declarations: [
                AppComponent
            ],
            providers: [
                { provide: AppStateService, useValue: mockAppstateSetvice },
                { provide: HostIntegrationService, useValue: mockHostIntegrationService },
                { provide: TranslateService, useValue: mockTranslateService },
            ]
        }).compileComponents();
    });

    it('should create the app', () => {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.debugElement.componentInstance;
        expect(app).toBeTruthy();
    });
});
