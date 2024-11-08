import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, RouterModule} from '@angular/router';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-file-viewer',
  templateUrl: './file-viewer.component.html',
  imports: [CommonModule, FormsModule],
  standalone: true,
  styleUrls: ['./file-viewer.component.css']
})
export class FileViewerComponent implements OnInit {
  textContent: string | null = null;

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.textContent = params['text'];
    });
  }
}
