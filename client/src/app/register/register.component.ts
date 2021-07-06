import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  @Output() CancelRegister = new EventEmitter();
  constructor(private accountService: AccountService, private toast: ToastrService) { }

  ngOnInit(): void {
  }

  public register()
  {
    this.accountService.register(this.model).subscribe(response =>{
      console.log(response);
      this.cancel();
    }, error => {
      console.log(error);
      this.toast.error(error.error);
      
    })
  }

  public cancel()
  {
    this.CancelRegister.emit(false);
  }

}
