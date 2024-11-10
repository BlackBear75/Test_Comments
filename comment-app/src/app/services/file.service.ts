import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FileService {
  readonly fileSizeLimit = 100 * 1024; // 100 KB
  readonly allowedFormats = ['image/jpeg', 'image/png', 'image/gif', 'text/plain'];

  validateFile(file: File): { isValid: boolean; errorMessage: string | null } {
    if (file.size > this.fileSizeLimit) {
      return { isValid: false, errorMessage: 'Файл не повинен перевищувати 100 КБ' };
    }
    if (!this.allowedFormats.includes(file.type)) {
      return { isValid: false, errorMessage: 'Допустимі формати файлів: JPG, GIF, PNG, TXT' };
    }
    return { isValid: true, errorMessage: null };
  }

  getFilePreview(file: File, callback: (previewUrl: string | ArrayBuffer | null) => void): void {
    if (file.type.startsWith('image/')) {
      const reader = new FileReader();
      reader.onload = () => {
        callback(reader.result);
      };
      reader.readAsDataURL(file);
    } else {
      callback(null);
    }
  }
}
