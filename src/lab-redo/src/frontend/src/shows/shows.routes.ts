import { Routes } from '@angular/router';
import { Shows } from './shows';
import { List } from './list';
import { Add } from './add';
export const SHOWS_ROUTES: Routes = [
  {
    path: '',
    component: Shows,
    children: [
      { path: '', component: List },
      { path: 'add', component: Add },
    ],
  },
];
