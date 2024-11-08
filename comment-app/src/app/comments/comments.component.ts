import { Component, OnInit } from '@angular/core';
import { RecordService } from '../services/record.service';
import { AddCommentModalComponent } from '../add-comment-modal/add-comment-modal.component';
import { CommentItemComponent } from '../comment-item/comment-item.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {Router, RouterModule} from '@angular/router';
import { AuthService } from '../services/auth.service'; // Додано імпорт AuthService

export interface IRecord {
  id: number;
  userName: string;
  email: string;
  text: string;
  creationDate: string | Date;
  comments: IComment[];
  showCommentField?: boolean;
  commentText?: string;
  captcha?: string;
  fileName?: string;
  fileType?: string;
  fileData?: string | null;
}

export interface IComment {
  id: number;
  userName: string;
  text: string;
  creationDate: string | Date;
  comments?: IComment[];
  showReplyField?: boolean;
  replyText?: string;
  fileName?: string;
  fileType?: string;
  fileData?: string | null;
}

@Component({
  selector: 'app-comments',
  templateUrl: './comments.component.html',
  imports: [RouterModule, CommonModule, FormsModule, CommentItemComponent, AddCommentModalComponent],
  standalone: true,
  styleUrls: ['./comments.component.css']
})
export class CommentsComponent implements OnInit {
  records: IRecord[] = [];
  isCommentModalVisible = false;
  selectedRecordId?: number;
  externalErrorMessage: string | null = null;
  selectedCommentId?: number;
  currentPage: number = 1;
  recordsPerPage: number = 25;
  totalRecords: number = 0;

  // Властивості для сортування
  sortField: string = 'date';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(
    private recordService: RecordService,
    private authService: AuthService,
    private router: Router // Додано Router
  ) {}

  ngOnInit(): void {
    this.loadRecords();
    this.loadTotalRecordsCount();
  }

  loadRecords() {
    this.recordService.getRecords(this.currentPage, this.recordsPerPage, this.sortField, this.sortDirection).subscribe({
      next: (data) => {
        this.records = data.map(record => {
          return {
            ...record,
            showCommentField: false,
            commentText: '',
            fileData: record.fileType && record.fileData ? this.processFileData(record.fileData, record.fileType) : null,
            comments: this.processComments(record.comments)
          };
        });
      },
      error: (error) => {
        console.error('Помилка завантаження записів:', error);
      }
    });
  }
  isUserLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }
  processComments(comments: IComment[]): IComment[] {
    return comments.map(comment => {
      return {
        ...comment,
        fileData: comment.fileType && comment.fileData ? this.processFileData(comment.fileData, comment.fileType) : null,
        comments: comment.comments ? this.processComments(comment.comments) : []
      };
    });
  }

  // Метод для обробки файлів
  processFileData(fileDataBase64: string, fileType: string): string | null {
    if (fileType.startsWith('image/')) {
      return `data:${fileType};base64,${fileDataBase64}`;
    } else if (fileType === 'text/plain') {
      const binary = atob(fileDataBase64);
      const array = Uint8Array.from(binary, char => char.charCodeAt(0));
      const utf8Decoder = new TextDecoder('utf-8');
      const decodedText = utf8Decoder.decode(array);
      const encodedText = encodeURIComponent(decodedText);
      return `/file-viewer?text=${encodedText}`;
    }
    return null;
  }

  // Метод для завантаження загальної кількості записів
  loadTotalRecordsCount() {
    this.recordService.getRecordsCount().subscribe({
      next: (count) => {
        this.totalRecords = count;
      },
      error: (error) => {
        console.error('Помилка завантаження кількості записів:', error);
      }
    });
  }

  // Метод для зміни параметрів сортування
  sortRecords(field: string) {
    if (this.sortField === field) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortDirection = 'asc';
    }
    this.loadRecords(); // Перезавантаження записів з новим сортуванням
  }

  // Метод для переходу на наступну сторінку
  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadRecords();
    }
  }

  // Метод для переходу на попередню сторінку
  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadRecords();
    }
  }

  get totalPages(): number {
    return Math.ceil(this.totalRecords / this.recordsPerPage);
  }

  isPaginationVisible(): boolean {
    return this.totalRecords > this.recordsPerPage;
  }

  // Метод для відкриття модального вікна коментаря
  openCommentModal(recordId: number, commentId?: number) {
    if (this.isUserLoggedIn()) {
      this.selectedRecordId = recordId;
      this.selectedCommentId = commentId;
      this.isCommentModalVisible = true;
      this.externalErrorMessage = null;
    } else {
      this.router.navigate(['/login']); // Перенаправлення на сторінку входу, якщо не авторизований
    }
  }

  // Метод для обробки доданого коментаря
  handleCommentAdded(commentData: { text: string; captcha: string; file: File | null }) {
    const formData = new FormData();
    formData.append('text', commentData.text);
    formData.append('captcha', commentData.captcha);
    formData.append('file', commentData.file || new Blob());

    const recordIdToUse = this.selectedCommentId ?? this.selectedRecordId;

    if (recordIdToUse) {
      this.recordService.addComment(recordIdToUse, formData).subscribe({
        next: (savedComment) => {
          console.log('Коментар успішно додано:', savedComment);
          this.isCommentModalVisible = false;
          this.externalErrorMessage = null;
          (document.querySelector('app-add-comment-modal') as any)?.clearForm();
        },
        error: (error) => {
          console.error('Помилка при додаванні коментаря:', error);
          this.externalErrorMessage = error.error?.message || 'Сталася помилка при додаванні коментаря.';
          this.isCommentModalVisible = true;
        }
      });
    }
  }

  // Метод для закриття модального вікна
  closeCommentModal() {
    this.isCommentModalVisible = false;
    this.selectedRecordId = undefined;
    this.selectedCommentId = undefined;
    this.externalErrorMessage = null;
  }
}
