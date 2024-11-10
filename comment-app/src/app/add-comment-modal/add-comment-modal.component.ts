import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RecordService } from '../services/record.service';
import { FileService } from '../services/file.service';
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
  @Input() parentRecordId?: number; // ID батьківського запису
  @Input() parentCommentId?: number; // ID батьківського коментаря, якщо потрібне вкладене коментування
  @Input() externalErrorMessage: string | null = null;
  @Output() onClose = new EventEmitter<void>();
  @Output() onCommentAdded = new EventEmitter<void>();

  commentText: string = '';
  captcha: string = '';
  captchaUrl: string = '';
  file: File | null = null;
  filePreviewUrl: string | ArrayBuffer | null = null;
  errorMessage: string | null = null;
  isFileValid: boolean = true;

  constructor(private recordService: RecordService, private fileService: FileService) {
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
      const validation = this.fileService.validateFile(selectedFile);

      this.isFileValid = validation.isValid;
      this.errorMessage = validation.errorMessage;
      this.file = this.isFileValid ? selectedFile : null;

      if (this.isFileValid) {
        this.fileService.getFilePreview(selectedFile, (preview) => {
          this.filePreviewUrl = preview;
        });
      }
    }
  }

  onSubmit() {
    if (!this.commentText || !this.captcha) {
      this.errorMessage = 'Заповніть всі обов’язкові поля.';
      return;
    }

    const formData = new FormData();
    formData.append('text', this.commentText);
    formData.append('captcha', this.captcha);

    if (this.file) {
      formData.append('file', this.file);
    }

    // Передаємо parentRecordId у formData, якщо він заданий
    if (this.parentCommentId) {
      formData.append('parentRecordId', this.parentCommentId.toString());
    } else if (this.parentRecordId) {
      formData.append('parentRecordId', this.parentRecordId.toString());
    }

    this.recordService.addRecord(formData).subscribe({
      next: () => {
        this.clearForm();
        this.onCommentAdded.emit();
        this.closeModal();
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Сталася помилка при додаванні коментаря.';
        this.refreshCaptcha();
      }
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
    this.filePreviewUrl = null;
    this.errorMessage = null;
    this.isFileValid = true;
  }
}
