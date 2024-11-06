import { Component, ChangeDetectorRef } from '@angular/core';
import { RecordService } from '../services/record.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

export interface RecordRequest {
  text: string;
  captcha: string;
}

@Component({
  selector: 'app-add-record',
  templateUrl: './add-record.component.html',
  standalone: true,
  imports: [FormsModule, CommonModule],
  styleUrls: ['./add-record.component.css']
})
export class AddRecordComponent {
  record = {
    text: '',
  };
  captcha: string = '';
  captchaUrl: string = '';
  errorMessage: string | null = null;
  successMessage: string | null = null;
  isModalVisible = false;

  constructor(private recordService: RecordService, private cdr: ChangeDetectorRef) {
    this.refreshCaptcha();
  }

  refreshCaptcha() {
    this.recordService.getCaptchaImage().subscribe(blob => {
      this.captchaUrl = URL.createObjectURL(blob);
    });
  }

  onSubmit() {
    this.errorMessage = null;
    this.successMessage = null;

    this.recordService.addRecord({ ...this.record, captcha: this.captcha }).subscribe({
      next: (response) => {
        if (response.success) {
          this.successMessage = response.message;
          this.record.text = '';
          this.captcha = '';
          this.refreshCaptcha();
          this.isModalVisible = true;
        } else {
          this.errorMessage = response.message;
          this.refreshCaptcha();
        }
      },
      error: (error) => {
        console.error('Помилка додавання запису:', error);
        console.log('Error message from server:', error.error?.message);

        this.errorMessage = error.error?.message || 'Сталася помилка. Спробуйте ще раз.';
        this.refreshCaptcha();
      }
    });
  }

  closeModal() {
    this.isModalVisible = false;
  }
}
