import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  imports: [CommonModule, FormsModule],
  standalone: true,
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  user: any = null;
  originalUser: any = null;
  errorMessage: string | null = null;
  isEditing: boolean = false;
  showLogoutModal: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.authService.getProfile().subscribe({
      next: (data) => {
        this.user = data;
        this.errorMessage = null;
      },
      error: (error) => {
        console.error('Error fetching profile', error);
        this.errorMessage = 'Не вдалося завантажити профіль. Спробуйте пізніше.';
      }
    });
  }

  isEmailValid(email: string): boolean {
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
  }

  editProfile() {
    this.isEditing = true;
    this.originalUser = { ...this.user };
  }

  saveProfile() {
    this.authService.updateProfile(this.user).subscribe({
      next: (response) => {
        if (response.success) {
          this.isEditing = false;
          this.errorMessage = null;
        } else {
          this.errorMessage = response.message;
        }
      },
      error: (error) => {
        console.error('Error updating profile', error);
        this.errorMessage = error.error.message || 'Не вдалося оновити профіль. Спробуйте пізніше.';
      }
    });
  }

  cancelEdit() {
    this.isEditing = false;
    this.user = { ...this.originalUser };
  }

  confirmLogout() {
    this.showLogoutModal = false;
    this.logout();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}