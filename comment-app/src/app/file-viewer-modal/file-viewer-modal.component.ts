import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-file-viewer-modal',
  templateUrl: './file-viewer-modal.component.html',
  imports: [ CommonModule ],
  standalone: true,
  styleUrls: ['./file-viewer-modal.component.css']
})
export class FileViewerModalComponent {
  @Input() fileName!: string;
  @Input() fileType!: string;
  @Input() fileData!: string;
  @Output() onClose = new EventEmitter<void>();

  close() {
    this.onClose.emit();
  }
}
