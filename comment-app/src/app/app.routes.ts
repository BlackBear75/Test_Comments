import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { CommentsComponent } from './comments/comments.component';
import { AddRecordComponent } from './add-record/add-record.component';
import { ProfileComponent } from './profile/profile.component';
import { AuthGuard } from './auth.guard';
import {FileViewerComponent} from './file-viewer/file-viewer.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'comments', component: CommentsComponent },

  { path: 'file-viewer', component: FileViewerComponent },
  { path: 'add-record', component: AddRecordComponent, canActivate: [AuthGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
];
