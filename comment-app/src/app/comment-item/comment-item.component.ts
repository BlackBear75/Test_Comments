import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IComment } from '../comments/comments.component';
import {RouterModule} from '@angular/router';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-comment-item',
  templateUrl: './comment-item.component.html',
  imports: [ FormsModule,CommonModule],
  standalone: true,
  styleUrls: ['./comment-item.component.css']
})
export class CommentItemComponent {
  @Input() comment!: IComment;
  @Output() addReply = new EventEmitter<string>();

  onAddReply(commentText: string | undefined) {
    if (commentText) {
      this.addReply.emit(commentText);
      this.comment.showCommentField = false;
      this.comment.commentText = '';
    }
  }
}
