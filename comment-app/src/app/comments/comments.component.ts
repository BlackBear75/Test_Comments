import { Component, OnInit } from '@angular/core';
import { RecordService } from '../services/record.service';
import { AddCommentModalComponent } from '../add-comment-modal/add-comment-modal.component';
import { CommentItemComponent } from '../comment-item/comment-item.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../services/auth.service';
import {FileService} from '../services/file.service';
import {FileViewerModalComponent} from '../file-viewer-modal/file-viewer-modal.component';

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
  imports: [RouterModule, CommonModule, FormsModule, CommentItemComponent, AddCommentModalComponent, FileViewerModalComponent],
  standalone: true,
  styleUrls: ['./comments.component.css']
})
export class CommentsComponent implements OnInit {
  records: IRecord[] = [];
  isCommentModalVisible = false;
  selectedRecordId?: number;
  selectedCommentId?: number;
  externalErrorMessage: string | null = null;
  currentPage: number = 1;
  recordsPerPage: number = 25;
  totalRecords: number = 0;
  isFileViewerModalVisible = false;
  selectedFileName = '';
  selectedFileType = '';
  selectedFileData = '';
  sortField: string = 'date';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(
    private recordService: RecordService,
    private authService: AuthService,
    private router: Router,
    private fileService: FileService,
  ) {}

  ngOnInit(): void {
    this.loadRecords();
    this.loadTotalRecordsCount();
  }
  openFileViewerModal(fileName: string | undefined, fileType: string, fileData: string | null | undefined) {
    if (fileType === 'text/plain' && fileData) {
      this.selectedFileName = fileName || 'Unnamed File';
      this.selectedFileType = fileType;

      this.selectedFileData = this.processFileData(fileData, fileType) || '';

      console.log('Декодований текстовий файл:', this.selectedFileData);

      this.isFileViewerModalVisible = true;
    }
  }



  closeFileViewerModal() {
    this.isFileViewerModalVisible = false;
  }
  loadRecords() {
    this.recordService.getRecords(this.currentPage, this.recordsPerPage, this.sortField, this.sortDirection).subscribe({
      next: (data) => {
        this.records = data.map(record => ({
          ...record,
          showCommentField: false,
          commentText: '',
          fileData: record.fileType && record.fileData ? this.processFileData(record.fileData, record.fileType) : null,
          comments: this.processComments(record.comments)
        }));
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
    return comments.map(comment => ({
      ...comment,
      fileData: comment.fileType && comment.fileData ? this.processFileData(comment.fileData, comment.fileType) : null,
      comments: comment.comments ? this.processComments(comment.comments) : []
    }));
  }

  processFileData(fileDataBase64: string, fileType: string): string | null {
    if (fileType.startsWith('image/')) {
      return `data:${fileType};base64,${fileDataBase64}`;
    } else if (fileType === 'text/plain') {
      if (!this.isBase64(fileDataBase64)) {
        console.warn('Попередження: Файл не є у форматі Base64. Повертаємо вміст напряму.');
        return decodeURIComponent(fileDataBase64);
      }

      const binary = atob(fileDataBase64);
      const array = Uint8Array.from(binary, char => char.charCodeAt(0));
      const utf8Decoder = new TextDecoder('utf-8');
      const decodedText = utf8Decoder.decode(array);

      console.log('Декодований текстовий файл:', decodedText);

      return decodedText;
    }
    return null;
  }


  isBase64(str: string): boolean {
    try {
      return btoa(atob(str)) === str;
    } catch (err) {
      return false;
    }
  }


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

  sortRecords(field: string) {
    if (this.sortField === field) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortDirection = 'asc';
    }
    this.loadRecords();
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadRecords();
    }
  }

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

  openCommentModal(recordId: number, commentId?: number) {
    if (this.isUserLoggedIn()) {
      this.selectedRecordId = recordId;
      this.selectedCommentId = commentId;
      this.isCommentModalVisible = true;
      this.externalErrorMessage = null;
    } else {
      this.router.navigate(['/login']);
    }
  }

  closeCommentModal() {
    this.isCommentModalVisible = false;
    this.selectedRecordId = undefined;
    this.selectedCommentId = undefined;
    this.externalErrorMessage = null;
  }

  handleCommentAdded() {
    this.loadRecords();
    this.closeCommentModal();
  }
}
