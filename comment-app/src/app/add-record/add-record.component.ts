import { Component, ChangeDetectorRef, Input, Output, EventEmitter } from '@angular/core';
import { RecordService } from '../services/record.service';
import { FileService } from '../services/file.service';
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
  @Input() parentRecordId?: number;
  @Output() onRecordAdded = new EventEmitter<void>();

  record = {
    text: '',
  };
  previousText: string = '';
  captcha: string = '';
  captchaUrl: string = '';
  errorMessage: string | null = null;
  successMessage: string | null = null;
  isModalVisible = false;

  file: File | null = null;
  filePreviewUrl: string | ArrayBuffer | null = null;
  isImage: boolean = false;
  isFileValid: boolean = true;

  constructor(
    private recordService: RecordService,
    private fileService: FileService,
    private cdr: ChangeDetectorRef
  ) {
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
      const validation = this.fileService.validateFile(file);

      this.isFileValid = validation.isValid;
      this.errorMessage = validation.errorMessage;
      this.file = this.isFileValid ? file : null;

      if (this.isFileValid) {
        this.fileService.getFilePreview(file, (preview) => {
          this.filePreviewUrl = preview;
          this.cdr.detectChanges();
        });
      }
    }
  }

  insertTag(tag: string, attribute: string = '') {
    this.previousText = this.record.text;
    const openTag = `<${tag} ${attribute}>`;
    const closeTag = `</${tag}>`;

    const textArea = document.querySelector('#text') as HTMLTextAreaElement;
    const startPos = textArea.selectionStart;
    const endPos = textArea.selectionEnd;

    const currentText = this.record.text;
    this.record.text =
      currentText.slice(0, startPos) +
      openTag +
      currentText.slice(startPos, endPos) +
      closeTag +
      currentText.slice(endPos);

    textArea.focus();
  }

  undoLastInsert() {
    this.record.text = this.previousText;
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
          this.clearForm();
          this.refreshCaptcha();
          this.isModalVisible = true;
          this.onRecordAdded.emit();
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

  clearForm() {
    this.record.text = '';
    this.captcha = '';
    this.file = null;
    this.filePreviewUrl = null;
  }

  closeModal() {
    this.isModalVisible = false;
  }
}
