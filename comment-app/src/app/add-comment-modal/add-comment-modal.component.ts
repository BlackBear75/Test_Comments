import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RecordService } from '../services/record.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-comment-modal',
  templateUrl: './add-comment-modal.component.html',
  imports: [FormsModule, CommonModule],
  standalone: true,
  styleUrls: ['./add-comment-modal.component.css']
})
export class AddCommentModalComponent {
  @Input() isVisible: boolean = false;
  @Input() parentRecordId?: number;
  @Input() parentCommentId?: number;
  @Input() externalErrorMessage: string | null = null;
  @Output() onClose = new EventEmitter<void>();
  @Output() onCommentAdded = new EventEmitter<{ text: string, captcha: string, file: File | null }>();

  commentText: string = '';
  captcha: string = '';
  captchaUrl: string = '';
  file: File | null = null;
  errorMessage: string | null = null;
  isFileValid: boolean = true;

  constructor(private recordService: RecordService) {
    this.refreshCaptcha();
  }

  refreshCaptcha() {
    this.recordService.getCaptchaImage().subscribe({
      next: (blob) => {
        this.captchaUrl = URL.createObjectURL(blob);
      },
      error: () => {
        this.errorMessage = 'Не вдалося завантажити CAPTCHA. Спробуйте ще раз.';
      }
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const selectedFile = input.files[0];
      const fileSizeLimit = 100 * 1024; // 100 KB

      // Reset file validity and error message
      this.isFileValid = true;
      this.errorMessage = null;

      if (selectedFile.size > fileSizeLimit) {
        this.errorMessage = 'Файл не повинен перевищувати 100 КБ';
        this.isFileValid = false;
        this.file = null;
        return;
      }

      const fileType = selectedFile.type;
      if (fileType.startsWith('image/')) {
        this.file = selectedFile;
        this.isFileValid = true; // File type is valid
        this.errorMessage = null;
      } else {
        this.errorMessage = 'Допустимі формати файлів: JPG, GIF, PNG';
        this.file = null;
        this.isFileValid = false; // File type is invalid
      }
    }
  }

  onSubmit() {
    if (!this.commentText || !this.captcha) {
      this.errorMessage = 'Заповніть всі обов’язкові поля.';
      return;
    }

    this.errorMessage = null;
    this.onCommentAdded.emit({
      text: this.commentText,
      captcha: this.captcha,
      file: this.file
    });
  }

  closeModal() {
    this.clearForm();
    this.onClose.emit();
    this.isVisible = false;
  }

  clearForm() {
    this.commentText = '';
    this.captcha = '';
    this.file = null;
    this.errorMessage = null;
    this.isFileValid = true;
  }
}
