import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IComment } from '../comments/comments.component';
import {FormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';

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

  openReplyModal() {
    // Викликаємо подію для відкриття модального вікна у батьківському компоненті
    this.addReply.emit({ parentCommentId: this.comment.id });
  }
}
