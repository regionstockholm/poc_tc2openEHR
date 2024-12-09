
import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DataService } from './services/data.service';
import { ActivatedRoute } from '@angular/router';
import { DesktopContextProvider } from './providers/desktop-context.provider';
import { OpenEhrProvider } from './providers/openehr.provider';

@Component({
  selector: 'app-sample',
  templateUrl: './sample.component.html',
  styleUrls: ['./sample.component.scss']
})
export class SampleComponent implements OnInit {

  @ViewChild('table') table: ElementRef;

  columns: any[] = [];
  rows: any[] = [];
  patientContext: any;
  ehrId: string;
  type: string;
  filter: {fromDate: string, toDate: string} = {fromDate: '', toDate: ''};
  noData: boolean = false;

  constructor(private readonly desktopContextProvider: DesktopContextProvider,
     private readonly openEhrProvider: OpenEhrProvider,
     private readonly dataService: DataService,
     private readonly route: ActivatedRoute,
     private readonly changeDetectorRef: ChangeDetectorRef) { 
      const today = new Date();
      const fromDate = { year: today.getFullYear(), month: today.getMonth() - 2, day: today.getDate() };
      const toDate = { year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate() };
      this.filter.fromDate = `${fromDate.year}-${fromDate.month}-${fromDate.day}`;
      this.filter.toDate = `${toDate.year}-${toDate.month}-${toDate.day}`; 
  }

  ngOnInit() {
    this.setTypeOfWidget();
  }

  public onDateRangeChange(event: any) {
    this.filter.fromDate = `${event.fromDate.year}-${event.fromDate.month}-${event.fromDate.day}`;
    this.filter.toDate = `${event.toDate.year}-${event.toDate.month}-${event.toDate.day}`;
    if (this.ehrId) {
      this.getData();
    }
  }

  private scrollToRight() {
    if (this.table?.nativeElement) {
      const tableElement = this.table.nativeElement;
      tableElement.scrollLeft = tableElement.scrollWidth;
    }
  }

  private getPatientContext(): void {
    this.desktopContextProvider.getPatient().subscribe((patientContext) => {
      this.patientContext = patientContext;
      console.log('patientContext',this.patientContext);
      this.getEhrId();
    });
  }

  private getEhrId(): void {
    this.openEhrProvider.getEhrId(this.patientContext.externalId).subscribe((ehrIdResponse) => {
      this.ehrId = ehrIdResponse.ehrId;
      console.log('ehrId',this.ehrId);
      this.getData();
    });
  }

  private getData(): void {
    const observable = this.type === 'RSK.Form.Widget.Measurements' ? this.dataService.getMeasurements(this.ehrId, this.filter) : this.dataService.getChemistryData(this.ehrId, this.filter);
    observable.subscribe((data) => {
      this.formatData(data);
      this.table.nativeElement.scrollRight = 0;
    });
  }

  private formatData(records) {
    let measurements = [];
    if (records?.length > 0) {
        this.noData = false;
        const uniquiIds = [...new Set(records?.map(x => x.Id))];
        const dates = [...new Set(records?.map(x => x.DateTime.split('T')[0]))];
        this.columns = dates.sort((a: Date, b: Date) => new Date(a).getTime() - new Date(b).getTime());
        uniquiIds.forEach(id => {
          const data = records.filter(y => y.Id === id);
          const measurement: any = {};
            measurement.id = data?.[0]?.Id;
            measurement.title = data?.[0]?.Name;
            measurement.unit = data?.[0]?.Value?.units || '';
            measurement.values = {};
            if (data?.length > 0) {
              data.forEach(x => {
                if (measurement.values[x.DateTime.split('T')[0]]?.length > 0) {
                  measurement.values[x.DateTime.split('T')[0]].push({date: x.DateTime, value: this.getValue(x)})
                } else {
                  measurement.values[x.DateTime.split('T')[0]] = [{date: x.DateTime, value: this.getValue(x)}];
                }
              });
            }
            const keys = Object.keys(measurement.values);
            keys.forEach(x => {
              measurement.values[x].sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
            });
            measurements.push(measurement);
        });

        if (measurements && measurements.length > 0 && this.type === 'RSK.Form.Widget.Measurements') {
          measurements = this.changeBloodPressureOrder("3719", "3720", measurements);
          measurements = this.changeBloodPressureOrder("4378", "4379", measurements);
        }
      this.scrollToRight();
    } else {
      this.columns = [];
      this.noData = true;
    }
    this.rows = measurements;
    this.changeDetectorRef.detectChanges();
  }

  private getValue(x): string {
    return this.type === 'RSK.Form.Widget.Measurements' ? x.Value?.units ? x.Value?.magnitude :  x.Value?.['value'] : x.Value;
  }

  private changeBloodPressureOrder(sysKey: string, diaKey: string, measurements: any) {
    const indexSystolisktOvre = measurements.findIndex(item => item.id === sysKey);
    const indexDiastolisktNedre = measurements.findIndex(item => item.id === diaKey);
    if (indexSystolisktOvre !== -1 && indexDiastolisktNedre !== -1) {
    const systolisktOvre = measurements.splice(indexSystolisktOvre, 1)[0];
    const diastolisktNedre = measurements.splice(indexDiastolisktNedre, 1)[0];
    if (indexSystolisktOvre > indexDiastolisktNedre) {
            measurements.splice(indexDiastolisktNedre, 0, systolisktOvre);
            measurements.splice(indexSystolisktOvre, 0, diastolisktNedre);
        } else {
            measurements.splice(indexDiastolisktNedre, 0, diastolisktNedre);
            measurements.splice(indexSystolisktOvre, 0, systolisktOvre);
        }
    }
    return measurements;
  }

  private setTypeOfWidget(): void {
    this.route.queryParams.subscribe(params => {
      this.type = params['formName'] || 'RSK.Form.Widget.Measurements';
      this.getPatientContext();
    });
  }
}
