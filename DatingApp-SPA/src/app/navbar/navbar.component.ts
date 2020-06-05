import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  model: any = {}; // object that contains the data from the form

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model) // returns an observable
      .subscribe(next => {
        console.log('Logged In Successfully');
      }, error => {
        console.log('Error While Logging In');
      });
  }

  loggedIn() {
    const token = localStorage.getItem('token'); // get the token from local storage
    return !!token; // returns true if the token is not empty
  }

  logOut() {
    localStorage.removeItem('token');
    console.log('Logged Out');
  }

}
