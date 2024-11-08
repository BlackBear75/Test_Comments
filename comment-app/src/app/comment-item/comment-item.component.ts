import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IComment } from '../comments/comments.component';
import { AuthService } from '../services/auth.service'; // Додано імпорт
import { Router } from '@angular/router'; // Додано імпорт Router
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

  constructor(private authService: AuthService, private router: Router) {} // Додано Router

  openReplyModal() {
    if (this.authService.isLoggedIn()) {
      this.addReply.emit({ parentCommentId: this.comment.id });
    } else {
      // Перенаправлення на сторінку логіна
      this.router.navigate(['/login']);
    }
  }
}
