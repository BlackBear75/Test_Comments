import { Component } from '@angular/core';
import {FormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [FormsModule,RouterModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  user = {
    name: 'Ім\'я користувача',
    email: 'user@example.com',
    homePage: 'https://example.com'
  };

  editProfile() {
    console.log('Редагування профілю');
  }
}
