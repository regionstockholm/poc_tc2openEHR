import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SampleComponent } from './sample.component';
import { TranslateModule } from '@ngx-translate/core';  
import { NgModule } from '@angular/core';
import { CustomDateFormatPipe } from './pipes/custom-date-format';
import { NgbDatepickerModule, NgbModule, NgbNavModule, NgbPopoverModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { CustomDateFilterComponent } from '../custom-date-filter/custom-date-filter';
import { TokenProvider } from './providers/token.provider';

@NgModule({
    declarations: [ 
                   SampleComponent, 
                   CustomDateFormatPipe,
                   CustomDateFilterComponent
                ],
    imports: [CommonModule, 
              FormsModule, 
              ReactiveFormsModule, 
              FlexLayoutModule,
              TranslateModule, 
              NgbTooltipModule, 
              NgbModule,
              NgbDatepickerModule,
              NgbPopoverModule,
              NgbNavModule             
             ],
    providers: [{provide: TokenProvider},]
})
export class SampleModule { }
