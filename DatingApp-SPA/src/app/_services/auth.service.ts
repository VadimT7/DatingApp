import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl = 'http//localhost:5000/api/auth';

  constructor(private http: HttpClient) {}

  // model is the object containing the data from the form (username, password)
  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response; // returns the token object

        if (user) {
          localStorage.setItem('token', user.token); // key:value pair
        }
      })
    ); // transform the response from the server (token)
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }
}
