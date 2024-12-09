import { Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Pipe({
  name: 'customDateFormat'
})
export class CustomDateFormatPipe implements PipeTransform {

  private enMonths = [
    'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
  ];

  private svMonths = [
    'Jan', 'Feb', 'Mars', 'Apr', 'Maj', 'Juni',
    'Juli', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'
  ];

  private currentLang = 'en';

  constructor(private translateService: TranslateService) {
    this.currentLang = this.translateService.currentLang;
  }

  transform(value: string, onlyYear: boolean, isDateTime: boolean): string {
    const months = this.currentLang.includes('en') ? this.enMonths : this.svMonths;
    if (onlyYear) {
      return value.split('-')[0];
    } else if(isDateTime) {
      const [year, month, day] = value.split('T')[0].split('-');
      const date = new Date(value);
      const hours = date.getUTCHours().toString().padStart(2, '0');
      const minutes = date.getUTCMinutes().toString().padStart(2, '0');
      return `${day}-${months[+month - 1]}-${year} ${hours}:${minutes}`;
    } else {
      const [ year, month, day] = value.split('-');
      console.log(year);
      return `${day} ${months[+month - 1]}`;
    }
  }
}