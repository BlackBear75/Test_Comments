import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RecordService } from '../services/record.service';

@Component({
  selector: 'app-add-record',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './add-record.component.html',
  styleUrls: ['./add-record.component.css']
})
export class AddRecordComponent {
  record = {
    captcha: '',
    text: ''
  };

  captchaImageUrl = 'captcha_image_url';

  constructor(private recordService: RecordService, private router: Router) {}

  onSubmit() {
    this.recordService.addRecord(this.record).subscribe({
      next: (response) => {
        console.log('Запис додано:', response);
        this.router.navigate(['/records']);
      },
      error: (error) => {
        console.error('Помилка додавання запису:', error);
      }
    });
  }
}
