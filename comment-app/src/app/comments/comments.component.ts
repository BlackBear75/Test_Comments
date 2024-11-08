import {Component, OnInit} from '@angular/core';
import {RecordService} from '../services/record.service';
import {RouterModule} from '@angular/router';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {CommentItemComponent} from '../comment-item/comment-item.component';
import {AddCommentModalComponent} from '../add-comment-modal/add-comment-modal.component';

export interface IRecord {
  id: number;
  userName: string;
  email: string;
  text: string;
  date: string | Date;
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
  date: string | Date;
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
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule, CommentItemComponent, AddCommentModalComponent],
  styleUrls: ['./comments.component.css']
})
export class CommentsComponent implements OnInit {
  records: IRecord[] = [];
  isCommentModalVisible = false;
  selectedRecordId?: number;
  selectedCommentId?: number;
  currentPage: number = 1;
  recordsPerPage: number = 25;
  totalRecords: number = 0;

  constructor(private recordService: RecordService) {}

  ngOnInit(): void {
    this.loadRecords();
    this.loadTotalRecordsCount();
  }

  loadRecords() {
    this.recordService.getRecords(this.currentPage, this.recordsPerPage).subscribe({
      next: (data) => {
        this.records = data.map(record => ({
          ...record,
          showCommentField: false,
          commentText: '',
          fileData: record.fileType && record.fileData ? this.processFileData(record.fileData) : null,
          comments: this.processComments(record.comments)
        }));
      },
      error: (error) => {
        console.error('Помилка завантаження записів:', error);
      }
    });
  }

  processComments(comments: IComment[]): IComment[] {
    return comments.map(comment => ({
      ...comment,
      fileData: comment.fileType && comment.fileData ? this.processFileData(comment.fileData) : null,
      comments: comment.comments ? this.processComments(comment.comments) : []
    }));
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

  processFileData(fileDataBase64: string): string | null {
    // Декодуємо Base64 у масив байтів
    const binary = atob(fileDataBase64);
    const array = Uint8Array.from(binary, char => char.charCodeAt(0));

    // Декодуємо масив байтів як UTF-8 текст
    const utf8Decoder = new TextDecoder('utf-8');
    const decodedText = utf8Decoder.decode(array);
    console.log("UTF-8 декодований текст:", decodedText);

    // Кодуємо декодований текст як URI component
    const encodedText = encodeURIComponent(decodedText);

    // Створюємо URL для FileViewerComponent
    const templateUrl = `/file-viewer?text=${encodedText}`;
    return templateUrl;
  }



  openCommentModal(recordId: number, commentId?: number) {
    console.log(`Opening modal for recordId: ${recordId}, parentCommentId: ${commentId}`);
    this.selectedRecordId = recordId;
    this.selectedCommentId = commentId;
    this.isCommentModalVisible = true;
  }

  closeCommentModal() {
    this.isCommentModalVisible = false;
    this.selectedRecordId = undefined;
    this.selectedCommentId = undefined;
  }

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
          this.closeCommentModal();
        },
        error: (error) => {
          console.error('Помилка при додаванні коментаря:', error);
        }
      });
    }
  }

  findCommentById(comments: IComment[], commentId: number): IComment | undefined {
    for (const comment of comments) {
      if (comment.id === commentId) return comment;
      const found = this.findCommentById(comment.comments || [], commentId);
      if (found) return found;
    }
    return undefined;
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
}
