import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RecordService } from '../services/record.service';
import { FormsModule } from '@angular/forms';
import { CommentItemComponent } from '../comment-item/comment-item.component';

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
declare const lightbox: any;

export interface IComment {
  id: number;
  userName: string;
  text: string;
  date: string | Date;
  comments?: IComment[];
  showReplyField?: boolean;
  replyText?: string;
}

@Component({
  selector: 'app-comments',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule, CommentItemComponent],
  templateUrl: './comments.component.html',
  styleUrls: ['./comments.component.css']
})
export class CommentsComponent implements OnInit {
  records: IRecord[] = [];
  currentPage: number = 1;
  recordsPerPage: number = 25;
  totalRecords: number = 0;

  constructor(private recordService: RecordService) {}

  ngOnInit(): void {
    this.loadRecords();
    this.loadTotalRecordsCount();

    // Ініціалізація параметрів Lightbox
    if (typeof lightbox !== 'undefined') {
      lightbox.option({
        'resizeDuration': 200,
        'wrapAround': true
      });
    }
  }


  loadRecords() {
    this.recordService.getRecords(this.currentPage, this.recordsPerPage).subscribe({
      next: (data) => {
        this.records = data.map(record => ({
          ...record,
          showCommentField: false,
          commentText: '',
          fileData: record.fileType && record.fileData ? this.processFileData(record.fileData, record.fileType) : null
        }));
      },
      error: (error) => {
        console.error('Помилка завантаження записів:', error);
      }
    });
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

  processFileData(fileData: string, fileType: string): string {
    if (fileType.startsWith('image/')) {
      const binary = atob(fileData);
      const array = [];
      for (let i = 0; i < binary.length; i++) {
        array.push(binary.charCodeAt(i));
      }
      const blob = new Blob([new Uint8Array(array)], { type: fileType });
      return URL.createObjectURL(blob);
    }
    return atob(fileData);
  }


  addComment(recordId: number, commentText: string | undefined) {
    if (!commentText) return;

    this.recordService.addComment(recordId, commentText).subscribe({
      next: (savedComment) => {
        const record = this.records.find(r => r.id === recordId);
        if (record) {
          record.comments.push({
            ...savedComment,
            showReplyField: false,
            replyText: ''
          });
          record.commentText = '';
          record.showCommentField = false;
        }
      },
      error: (error) => {
        console.error('Помилка при додаванні коментаря:', error);
      }
    });
  }

  addNestedComment(recordId: number, { text, parentCommentId }: { text: string, parentCommentId: number }) {
    this.recordService.addComment(parentCommentId, text, recordId).subscribe({
      next: (savedComment) => {
        const record = this.records.find(r => r.id === recordId);
        if (record) {
          const parentComment = this.findCommentById(record.comments, parentCommentId);
          if (parentComment) {
            parentComment.comments = parentComment.comments || [];
            parentComment.comments.push({
              ...savedComment,
              showReplyField: false,
              replyText: ''
            });
          }
        }
      },
      error: (error) => {
        console.error('Помилка при додаванні відповіді на коментар:', error);
      }
    });
  }

  findCommentById(comments: IComment[], commentId: number): IComment | undefined {
    for (let comment of comments) {
      if (comment.id === commentId) return comment;
      const found = this.findCommentById(comment.comments || [], commentId);
      if (found) return found;
    }
    return undefined;
  }

  cancelComment(record: IRecord) {
    record.showCommentField = false;
    record.commentText = '';
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
