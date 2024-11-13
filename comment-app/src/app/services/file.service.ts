import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FileService {
  readonly textFileSizeLimit = 100 * 1024;
  readonly allowedFormats = ['image/jpeg', 'image/png', 'image/gif', 'text/plain'];
  readonly maxImageWidth = 320;
  readonly maxImageHeight = 240;

  validateFile(file: File): { isValid: boolean; errorMessage: string | null } {
    if (file.type === 'text/plain' && file.size > this.textFileSizeLimit) {
      return { isValid: false, errorMessage: 'Текстовий файл не повинен перевищувати 100 КБ' };
    }

    if (!this.allowedFormats.includes(file.type)) {
      return { isValid: false, errorMessage: 'Допустимі формати файлів: JPG, GIF, PNG, TXT' };
    }

    return { isValid: true, errorMessage: null };
  }

  getFilePreview(file: File, callback: (previewUrl: string | ArrayBuffer | null) => void): void {
    if (file.type.startsWith('image/')) {
      this.resizeImage(file, this.maxImageWidth, this.maxImageHeight, callback);
    } else {
      callback(null);
    }
  }

  resizeImage(
    file: File,
    maxWidth: number,
    maxHeight: number,
    callback: (resizedImage: string | ArrayBuffer | null) => void
  ) {
    const img = new Image();
    const reader = new FileReader();

    reader.onload = (e) => {
      if (e.target) {
        img.src = e.target.result as string;
      }
    };

    img.onload = () => {
      let width = img.width;
      let height = img.height;

      if (width > maxWidth || height > maxHeight) {
        const aspectRatio = width / height;
        if (width > height) {
          width = maxWidth;
          height = maxWidth / aspectRatio;
        } else {
          height = maxHeight;
          width = maxHeight * aspectRatio;
        }
      }

      const canvas = document.createElement('canvas');
      canvas.width = width;
      canvas.height = height;
      const ctx = canvas.getContext('2d');

      if (ctx) {
        ctx.drawImage(img, 0, 0, width, height);
        canvas.toBlob((blob) => {
          if (blob) {
            callback(URL.createObjectURL(blob));
          }
        }, file.type);
      } else {
        callback(null);
      }
    };

    reader.readAsDataURL(file);
  }

  getTextFileContent(file: File, callback: (textContent: string | null) => void): void {
    if (file.type === 'text/plain') {
      const reader = new FileReader();
      reader.onload = () => {
        callback(reader.result as string);
      };
      reader.onerror = () => {
        console.error('Помилка при зчитуванні файлу.');
        callback(null);
      };
      reader.readAsText(file);
    } else {
      callback(null);
    }
  }
}
