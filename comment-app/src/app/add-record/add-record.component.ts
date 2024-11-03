import { Component } from '@angular/core';
import {RouterModule} from '@angular/router';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-add-record',
  standalone: true,
  imports: [RouterModule,FormsModule],
  templateUrl: './add-record.component.html',
  styleUrl: './add-record.component.css'
})
export class AddRecordComponent {
  record = {
    userName: '',
    email: '',
    homePage: '',
    captcha: '',
    text: ''
  };

  onSubmit() {
    console.log('Запис додано:', this.record);
    // Логіка для збереження запису в базу даних
  }
}
