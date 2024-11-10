import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IComment } from '../comments/comments.component';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-comment-item',
  templateUrl: './comment-item.component.html',
  standalone: true,
  imports: [FormsModule, CommonModule],
  styleUrls: ['./comment-item.component.css']
})
export class CommentItemComponent {
  @Input() comment!: IComment;
  @Output() addReply = new EventEmitter<{ parentCommentId: number }>();

  sanitizedFileDataUrl?: SafeUrl;

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
}
