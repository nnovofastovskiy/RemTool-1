import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-send-request',
  templateUrl: './send-request.component.html',
  styleUrls: ['./send-request.component.scss']
})
export class SendRequestComponent implements OnInit {
  requestForm: FormGroup;
  formPopup: boolean;
  constructor( ) { }

  ngOnInit(): void {
    this.requestForm = new FormGroup({
      contacts: new FormControl(null, [Validators.required]),
      name: new FormControl(null, []),
      text: new FormControl(null, [Validators.required]),
    })
  }

  formPopupClose(e){
    console.log(e.path[0]);
    if ((e.path[0] == document.querySelector('.request-container')) || (e.path[0] == document.querySelector('.request-form__close'))) {
      this.formPopup = false;
    }
  }

  sendRequestPopup(e){
    e.target.blur();
    this.formPopup = true;
  }
  sendRequest(e){
    e.target.blur();
    
  }

  inputFocus(e){
    e.target.previousElementSibling.focus();
  }
}
