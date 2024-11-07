import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IComment } from '../comments/comments.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-comment-item',
  templateUrl: './comment-item.component.html',
  imports: [FormsModule, CommonModule],
  standalone: true,
  styleUrls: ['./comment-item.component.css']
})
export class CommentItemComponent {
  @Input() comment!: IComment;
  @Output() addReply = new EventEmitter<{ text: string, parentCommentId: number }>();

  emitReply(replyText: string) {
    if (replyText) {
      this.addReply.emit({ text: replyText, parentCommentId: this.comment.id });
      this.comment.replyText = '';
      this.comment.showReplyField = false;
    }
  }
}
