import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from './user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  isFetching = false;
  error = "";

  constructor(private userService: UserService, private router: Router) { }

  ngOnInit(): void {
  }

  onLogin(data: any) {
    /*this.isFetching = true
    /*if (condition) {
      data.form.controls["name"].status = "INVALID"
      this.errorName = "Name must be 3 char"
    } else {
      this.userService.addUser(data.value).subscribe({
        next: () => this.router.navigate(['user']),
        error: err => this.error = err,
        complete: () => this.isFetching = false
      })
    //}*/
  }
}
