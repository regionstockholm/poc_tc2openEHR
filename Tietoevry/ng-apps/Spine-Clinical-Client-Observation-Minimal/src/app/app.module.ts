import { APP_INITIALIZER, ErrorHandler, NgModule } from '@angular/core';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { BrowserModule } from '@angular/platform-browser';
import { bootstrap } from '@spine/bootstrap';
import { ActionService, AppStateService, ContextService, HostAppNotificationService, HostIntegrationService } from '@spine/host-integration';
import { RestClient, restClientFactory, HttpClient as SpineHttpClient } from '@spine/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SampleModule } from './sample/sample.module';
import { RootErrorHandler } from './shared';

// import ngx-translate and the http loader
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgbDatepickerModule, NgbModule, NgbNavModule, NgbPopoverModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { AppConfig, AppConfigService } from '@spine/primitive';
import { environment } from 'src/environments/environment';
import { ReactiveFormsModule } from '@angular/forms';


// required for AOT compilation
export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export function enableHostIntegration(hostAppService: HostIntegrationService): any {
  return () => {
    hostAppService.initialize({
      definition: {
        id: '86e29711-e065-481d-933b-060ca0259fc6',
        name: 'Spine-Clinical-Client-Observation-Minimal',
        params: undefined
      },
      interceptor: true,
      actions: [],
      params: {}
    });
  };
}

export function loadSettings(appConfigService: AppConfigService)
  : () => Promise<any> {
  return (): Promise<void> => {
    return new Promise((resolve, reject) => {
      appConfigService.loadSettings()
        .then(() => {
          const betterConfig = AppConfig.extras['better'];
          environment.HostUrl = betterConfig.host_url;
          environment.discoveryRoute = AppConfig.discovery_route;
          environment.BetterPlatformUrl = `${betterConfig.host_url}/ehr/rest/v1`;
        })
        .then(() => resolve())
        .catch(() => reject());
    });
  };
}

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    FlexLayoutModule,
    AppRoutingModule,
    NgbDatepickerModule,
    HttpClientModule,
    SampleModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    NgbModule,
    NgbNavModule,
    NgSelectModule,
    NgbPopoverModule,
    ReactiveFormsModule
  ],
  providers: [
    { provide: ErrorHandler, useClass: RootErrorHandler },
    { provide: SpineHttpClient, useValue: bootstrap.resolve<SpineHttpClient>(SpineHttpClient) },
    { provide: ActionService, useValue: bootstrap.resolve<ActionService>(ActionService) },
    { provide: ContextService, useValue: bootstrap.resolve<ContextService>(ContextService) },
    { provide: AppStateService, useValue: bootstrap.resolve<AppStateService>(AppStateService) },
    { provide: HostAppNotificationService, useValue: bootstrap.resolve<HostAppNotificationService>(HostAppNotificationService) },
    { provide: HostIntegrationService, useValue: bootstrap.resolve<HostIntegrationService>(HostIntegrationService) },
    { provide: AppConfigService, useValue: bootstrap.resolve<AppConfigService>(AppConfigService) },
    { provide: APP_INITIALIZER, useFactory: enableHostIntegration, deps: [HostIntegrationService], multi: true },
    { provide: APP_INITIALIZER, useFactory: loadSettings, deps: [AppConfigService], multi: true },
    {
      provide: SpineHttpClient,
      useValue: bootstrap.resolve<SpineHttpClient>(SpineHttpClient)
    },
    { provide: RestClient, useFactory: () => restClientFactory(AppConfig.api_endpoint,environment.discoveryRoute) }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
