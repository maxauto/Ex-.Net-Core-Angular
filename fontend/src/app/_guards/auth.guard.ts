import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(public authService: AuthService, public alertify: AlertifyService, private router: Router){ }

  canActivate(): boolean {
    if (this.authService.loggedIn()){
      return true;
    }

    this.router.navigate(['']);
    return false;
  }
}
