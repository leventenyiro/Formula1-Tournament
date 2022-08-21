import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable, Subject, tap, throwError } from "rxjs";
import { User } from "./login.model";

@Injectable({ providedIn: 'root' })
export class UserService {
    usersChanged = new Subject<User[]>();

    constructor(private http: HttpClient) {}
}