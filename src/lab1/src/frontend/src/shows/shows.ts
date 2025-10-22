import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ShowsStore } from './shows.store';

@Component({
  selector: 'app-shows',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, RouterLink],
  providers: [ShowsStore],
  template: `
    <header class="text-center">
      <h1 class="font-black text-3xl">TV Shows</h1>
      <p>Share your favorite shows with your friends!</p>
      <a
        class="btn btn-success btn-xl hover:ring-4  ring-amber-200 mt-4"
        routerLink="add"
      >
        Share Your Favorite Show
      </a>
    </header>
    <div class="mt-6">
      <router-outlet></router-outlet>
    </div>
  `,
  styles: ``,
})
export class Shows {}
