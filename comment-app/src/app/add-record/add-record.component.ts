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

  file: File | null = null;
  filePreviewUrl: string | ArrayBuffer | null = null;
  isImage: boolean = false;
  isFileValid: boolean = true; // Додаємо змінну для валідації файлу

  constructor(private recordService: RecordService, private cdr: ChangeDetectorRef) {
    this.refreshCaptcha();
  }

  refreshCaptcha() {
    this.recordService.getCaptchaImage().subscribe(blob => {
      this.captchaUrl = URL.createObjectURL(blob);
    });
  }

  onFileSelected(event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files[0]) {
      const file = target.files[0];
      const fileSizeLimit = 100 * 1024; // 100 KB

      if (file.size > fileSizeLimit) {
        this.errorMessage = 'Файл не повинен перевищувати 100 КБ';
        this.file = null;
        this.filePreviewUrl = null;
        this.isFileValid = false; // Файл недійсний
        return;
      }

      const fileType = file.type;
      if (fileType.startsWith('image/') || fileType === 'text/plain') {
        this.file = file;
        this.isImage = fileType.startsWith('image/');
        this.errorMessage = null;
        this.isFileValid = true; // Файл дійсний

        const reader = new FileReader();
        reader.onload = () => {
          const img = new Image();
          img.src = reader.result as string;
          img.onload = () => {
            const canvas = document.createElement('canvas');
            const ctx = canvas.getContext('2d');

            // Пропорційне зменшення зображення
            const maxWidth = 320;
            const maxHeight = 240;
            let width = img.width;
            let height = img.height;

            if (width > maxWidth || height > maxHeight) {
              const scale = Math.min(maxWidth / width, maxHeight / height);
              width = Math.floor(width * scale);
              height = Math.floor(height * scale);
            }

            canvas.width = width;
            canvas.height = height;
            ctx?.drawImage(img, 0, 0, width, height);
            this.filePreviewUrl = canvas.toDataURL('image/jpeg');
            this.cdr.detectChanges();
          };
        };
        reader.readAsDataURL(file);
      } else {
        this.errorMessage = 'Допустимі формати файлів: JPG, GIF, PNG';
        this.file = null;
        this.filePreviewUrl = null;
        this.isFileValid = false; // Файл недійсний
      }
    }
  }

  onSubmit() {
    if (!this.record.text || !this.captcha || !this.isFileValid) {
      this.errorMessage = 'Будь ласка, заповніть текст, CAPTCHA і виберіть правильний файл перед відправкою.';
      return;
    }

    this.errorMessage = null;
    this.successMessage = null;

    const formData = new FormData();
    formData.append('text', this.record.text);
    formData.append('captcha', this.captcha);

    if (this.file) {
      formData.append('file', this.file);
    }

    this.recordService.addRecord(formData).subscribe({
      next: (response) => {
        if (response.success) {
          this.successMessage = 'Запис успішно додано!';
          this.record.text = '';
          this.captcha = '';
          this.file = null;
          this.filePreviewUrl = null;
          this.refreshCaptcha();
          this.isModalVisible = true;
        } else {
          this.errorMessage = response.message;
          this.refreshCaptcha();
        }
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Сталася помилка. Спробуйте ще раз.';
        this.refreshCaptcha();
      }
    });
  }

  closeModal() {
    this.isModalVisible = false;
  }
}
