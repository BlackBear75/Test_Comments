import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IComment } from '../comments/comments.component';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import {FileViewerModalComponent} from '../file-viewer-modal/file-viewer-modal.component';
import {CommonModule, DatePipe} from '@angular/common';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-comment-item',
  templateUrl: './comment-item.component.html',
  standalone: true,
  imports: [
    FileViewerModalComponent,
    DatePipe,
    CommonModule,
    FormsModule
  ],
  styleUrls: ['./comment-item.component.css']
})
export class CommentItemComponent {
  @Input() comment!: IComment;
  @Output() addReply = new EventEmitter<{ parentCommentId: number }>();

  sanitizedFileDataUrl?: SafeUrl;
  isFileViewerModalVisible = false;
  selectedFileName = '';
  selectedFileType = '';
  selectedFileData = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit() {
    this.sanitizeFileData();
  }

  sanitizeFileData() {
    if (this.comment.fileData) {
      this.sanitizedFileDataUrl = this.sanitizer.bypassSecurityTrustUrl(this.comment.fileData);
    }
  }

  openReplyModal() {
    if (this.authService.isLoggedIn()) {
      this.addReply.emit({ parentCommentId: this.comment.id });
    } else {
      this.router.navigate(['/login']);
    }
  }

  openFileViewerModal() {
    if (this.comment.fileType === 'text/plain' && this.comment.fileData) {
      this.selectedFileName = this.comment.fileName || 'Unnamed File';
      this.selectedFileType = this.comment.fileType;
      this.selectedFileData = this.comment.fileData;
      this.isFileViewerModalVisible = true;
    }
  }

  closeFileViewerModal() {
    this.isFileViewerModalVisible = false;
  }
}
