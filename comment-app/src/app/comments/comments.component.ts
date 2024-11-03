import { Component, OnInit } from '@angular/core';
import {RouterModule} from '@angular/router';
import {FormsModule} from '@angular/forms';

interface Comment {
  userName: string;
  text: string;
  date: Date;
}

interface Record {
  id: number;
  userName: string;
  email: string;
  text: string;
  date: Date;
  comments: Comment[];
  showCommentForm?: boolean;
  newComment?: Partial<Comment>;
}

@Component({
  selector: 'app-comments',
  standalone: true,
  imports: [RouterModule,FormsModule],
  templateUrl: './comments.component.html',
  styleUrls: ['./comments.component.css']
})
export class CommentsComponent implements OnInit {
  records: Record[] = [];
  newRecord: Partial<Record> = {};
  showNewRecordForm: boolean = false;
  currentPage: number = 1;
  recordsPerPage: number = 10;

  ngOnInit() {
    this.loadRecords();
  }

  loadRecords() {
    this.records = [];
  }

  toggleNewRecordForm() {
    this.showNewRecordForm = !this.showNewRecordForm;
  }

  addRecord() {
    const record: Record = {
      id: Date.now(),
      userName: this.newRecord.userName!,
      email: this.newRecord.email!,
      text: this.newRecord.text!,
      date: new Date(),
      comments: []
    };
    this.records.push(record);
    this.newRecord = {};
    this.showNewRecordForm = false;
  }

  toggleCommentForm(record: Record) {
    record.showCommentForm = !record.showCommentForm;
    record.newComment = {};
  }

  addComment(record: Record) {
    const comment: Comment = {
      userName: record.newComment!.userName!,
      text: record.newComment!.text!,
      date: new Date()
    };
    record.comments.push(comment);
    record.showCommentForm = false;
    record.newComment = {};
  }

  nextPage() {
    this.currentPage++;
    this.loadRecords();
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadRecords();
    }
  }

  isPaginationVisible(): boolean {
    return this.records.length > this.recordsPerPage;
  }
}
