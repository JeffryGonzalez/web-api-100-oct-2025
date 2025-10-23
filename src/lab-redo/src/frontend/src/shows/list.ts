import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ShowsStore } from './shows.store';

@Component({
  selector: 'app-shows-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [],
  providers: [ReactiveFormsModule],
  template: `
    @if (store.shows.error()) {
      <div class="alert alert-error">
        <span>Error loading shows! Maybe the API is down?</span>
      </div>
    } @else {
      @if (store.shows.isLoading()) {
        <span class="loading loading-lg loading-bars"></span>
      } @else {
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          @for (show of store.shows.value(); track show.id) {
            <div class="card bg-base-100 shadow-xl">
              <div class="card-body">
                <h2 class="card-title">{{ show.name }}</h2>
                <p>{{ show.description }}</p>
                <div class="card-actions justify-between items-center">
                  <div class="badge badge-secondary">
                    {{ show.streamingService }}
                  </div>
                </div>
              </div>
            </div>
          }
        </div>
      }
    }
  `,
  styles: ``,
})
export class List {
  store = inject(ShowsStore);
}
