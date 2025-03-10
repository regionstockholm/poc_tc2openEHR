import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { Constants } from '../shared/models/constants';

@Component({
  selector: 'app-custom-date-filter',
  templateUrl: './custom-date-filter.html',
  styleUrls: ['./custom-date-filter.scss']
})
export class CustomDateFilterComponent implements OnInit {

  @Output() dateRangeSelected = new EventEmitter<{ fromDate: NgbDateStruct, toDate: NgbDateStruct }>();

  public menuOpened = false;
  public isCustomDateRange = false;
  public dateFilterOptions: any[] = [
    {id: 1, name: Constants.DATERANGE.last_15_Days, column: 1 },
    {id: 2, name: Constants.DATERANGE.last_30_days, column: 1 },
    {id: 3,  name: Constants.DATERANGE.last_3_months, column: 1 },
    {id: 4,  name: Constants.DATERANGE.last_6_months, column: 1 },
    {id: 5,  name: Constants.DATERANGE.last_1_year, column: 2 },
    {id: 6,  name: Constants.DATERANGE.last_2_years, column: 2 },
    {id: 7,  name: Constants.DATERANGE.last_5_years, column: 2 },
    {id: 8,  name: Constants.DATERANGE.last_10_years, column: 2 }
  ];

  public dateFilterText: string = this.dateFilterOptions[2].name;
  public fromDate: NgbDateStruct;
  public toDate: NgbDateStruct;

  public ngOnInit(): void {
    const today = new Date();
    this.fromDate = { year: today.getFullYear(), month: today.getMonth() - 2, day: today.getDate() };
    this.toDate = { year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate() };
  }

  public toogleMenuPopup(): void {
    this.menuOpened = !this.menuOpened;
  }

  public onDateRangeFilter(item: any): void {
    this.menuOpened = false;
    this.dateFilterText = item.name;
    this.isCustomDateRange = true;
    this.dateRangeSelected.emit(this.setDateRange(item.id));
  }

  public onDateFilterCancel(): void {
    this.menuOpened = false;
  }
  
  public onDateFilterApply(): void {
    this.menuOpened = false;
    this.dateRangeSelected.emit({ fromDate: this.fromDate, toDate: this.toDate });
    this.isCustomDateRange = true;
  }

  private getDateBefore(days: number): NgbDateStruct {
    const date = new Date();
    date.setDate(date.getDate() - days);
    return { year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() };
  }
  
  private getDateBeforeMonths(months: number): NgbDateStruct {
    const date = new Date();
    date.setMonth(date.getMonth() - months);
    return { year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() };
  }
  
  private getDateBeforeYears(years: number): NgbDateStruct {
    const date = new Date();
    date.setFullYear(date.getFullYear() - years);
    return { year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() };
  }
  
  private setDateRange(id: number): { fromDate: NgbDateStruct, toDate: NgbDateStruct } {
    const today = new Date();
    let fromDate: NgbDateStruct;
    const toDate: NgbDateStruct = { year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate() };
  
    switch (id) {
      case 1:
        fromDate = this.getDateBefore(15);
        break;
      case 2:
        fromDate = this.getDateBefore(30);
        break;
      case 3:
        fromDate = this.getDateBeforeMonths(3);
        break;
      case 4:
        fromDate = this.getDateBeforeMonths(6);
        break;
      case 5:
        fromDate = this.getDateBeforeYears(1);
        break;
      case 6:
        fromDate = this.getDateBeforeYears(2);
        break;
      case 7:
        fromDate = this.getDateBeforeYears(5);
        break;
      case 8:
        fromDate = this.getDateBeforeYears(10);
        break;
      default:
        fromDate = this.getDateBefore(15);
    }
    this.isCustomDateRange = false;
    return { fromDate, toDate };
  }

}
