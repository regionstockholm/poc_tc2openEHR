import { ErrorHandler, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RootErrorHandler implements ErrorHandler {
  public handleError(error: any): void {
    throw error;
  }
}
