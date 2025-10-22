import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('../shows/shows.routes').then((m) => m.SHOWS_ROUTES),
  },
];
