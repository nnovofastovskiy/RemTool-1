import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RequestServiceService {

  constructor() { }

  invokeEvent: Subject<any> = new Subject();

  callMethodOfSecondComponent(e) {
    this.invokeEvent.next(e);
  }
}
