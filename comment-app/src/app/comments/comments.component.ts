// comments.component.ts
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
}

export interface IComment {
  id: number;
  userName: string;
  text: string;
  date: string | Date;
  comments?: IComment[];
  showCommentField?: boolean;
  commentText?: string;
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

  ngOnInit() {
    this.loadRecords();
    this.loadTotalRecordsCount();
  }

  loadRecords() {
    this.recordService.getRecords(this.currentPage, this.recordsPerPage).subscribe({
      next: (data) => {
        this.records = data.map(record => ({
          ...record,
          showCommentField: false,
          commentText: ''
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

  // Метод для додавання коментаря до запису або коментаря
  addComment(recordId: number, commentText: string | undefined) {
    if (!commentText) return;

    this.recordService.addComment(recordId, commentText).subscribe({
      next: (savedComment) => {
        const record = this.records.find(r => r.id === recordId);
        if (record) {
          record.comments.push(savedComment);
          record.commentText = '';
          record.showCommentField = false;
        }
      },
      error: (error) => {
        console.error('Помилка при додаванні коментаря:', error);
      }
    });
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
