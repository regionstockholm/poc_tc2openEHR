import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AppStateService, HostIntegrationService } from '@spine/host-integration';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  private readonly defautlLanguage = "sv-SE";

  constructor(private readonly appStateService: AppStateService,
    private readonly hostIntegrationService: HostIntegrationService,
    private readonly translateService: TranslateService) { }

  public ngOnInit() {
    this.setLanguage();
  }

  private setLanguage(): void {

    const uiculture = localStorage.getItem('uiculture-webfx');
    if (uiculture) {
      console.log('uiculture-webfx', uiculture.replaceAll('"', ''));
      this.translateService.use(uiculture.replaceAll('"', ''));
    } else {
      this.translateService.use(this.defautlLanguage);
    }
    // if (this.hostIntegrationService.isInitialized) {
    //   this.appStateService.getApplicationUICulture().subscribe(
    //     async (culture) => {
    //       const lang = culture ? culture : this.defautlLanguage;
    //       this.translateService.use(lang);
    //     }
    //   );
    // } else {
    //   this.translateService.use(this.defautlLanguage);
    // }
  }
}