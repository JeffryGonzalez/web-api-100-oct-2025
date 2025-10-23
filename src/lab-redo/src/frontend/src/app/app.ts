import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  template: `
    <main class="container mx-auto p-12">
      <router-outlet />
    </main>
  `,
  styles: [],
  imports: [RouterOutlet],
})
export class App {}
