import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  standalone: true,
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent {
  user = {
    name: 'Імя користувача' ,
    email: 'user@example.com',
    homePage: 'https://example.com'
  };

  constructor(private authService: AuthService, private router: Router) {}

  editProfile() {
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
