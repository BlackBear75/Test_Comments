import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IComment } from '../comments/comments.component';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

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

  constructor(private authService: AuthService, private router: Router) {}

  openReplyModal() {
    if (this.authService.isLoggedIn()) {
      this.addReply.emit({ parentCommentId: this.comment.id });
    } else {
      this.router.navigate(['/login']);
    }
  }
}
