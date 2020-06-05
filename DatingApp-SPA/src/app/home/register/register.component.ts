import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  model: any = {}; // contains the data entered into the form
  @Output() cancelRegister = new EventEmitter();

  constructor(private authService: AuthService) {}

  ngOnInit(): void {}

  register() {
    this.authService.register(this.model)
      .subscribe(() => {
        console.log('Registration successful');
      }, error => {
        console.log('Error during registration)');
      });
  }

  cancel() {
    this.cancelRegister.emit(false); // emit false because we cancel the registration 
                                     // -> registerMode in HomeComponent becomes false because we cancel the registration
  }

}
