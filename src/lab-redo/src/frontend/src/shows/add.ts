import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import {
  FormGroup,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ShowsStore } from './shows.store';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  template: `
    <div class="max-w-2xl mx-auto p-6">
      <h2 class="text-3xl font-bold mb-6">Add a New Show</h2>
      <div class="card bg-base-100 shadow-xl">
        <div class="card-body">
          <form [formGroup]="form" (ngSubmit)="onSubmit()" class="space-y-6">
            <div class="form-control">
              <label class="label" for="title">
                <span class="label-text font-medium">Title</span>
              </label>
              <input
                id="title"
                type="text"
                formControlName="name"
                class="input input-bordered w-full"
                [class.input-error]="
                  form.get('name')?.invalid && form.get('name')?.touched
                "
                placeholder="Enter the show title"
                required
                aria-describedby="title-help"
              />
              <div class="label">
                <span class="label-text-alt" id="title-help"
                  >The name of the TV show</span
                >
              </div>
              @if (form.get('name')?.invalid && form.get('name')?.touched) {
                <div class="text-error text-sm mt-1">
                  @if (form.get('name')?.errors?.['required']) {
                    <span>Title is required</span>
                  }
                  @if (form.get('name')?.errors?.['minlength']) {
                    <span>Title must be at least 3 characters long</span>
                  }
                  @if (form.get('name')?.errors?.['maxlength']) {
                    <span>Title must be no more than 100 characters long</span>
                  }
                </div>
              }
            </div>

            <div class="form-control">
              <label class="label" for="description">
                <span class="label-text font-medium">Description</span>
              </label>
              <textarea
                id="description"
                formControlName="description"
                class="textarea textarea-bordered w-full min-h-24"
                [class.textarea-error]="
                  form.get('description')?.invalid &&
                  form.get('description')?.touched
                "
                placeholder="Enter a brief description of the show"
                rows="4"
                aria-describedby="description-help"
              ></textarea>
              <div class="label">
                <span class="label-text-alt" id="description-help"
                  >A brief summary of what the show is about</span
                >
              </div>
              @if (
                form.get('description')?.invalid &&
                form.get('description')?.touched
              ) {
                <div class="text-error text-sm mt-1">
                  @if (form.get('description')?.errors?.['required']) {
                    <span>Description is required</span>
                  }
                  @if (form.get('description')?.errors?.['minlength']) {
                    <span>Description must be at least 10 characters long</span>
                  }
                  @if (form.get('description')?.errors?.['maxlength']) {
                    <span
                      >Description must be no more than 500 characters
                      long</span
                    >
                  }
                </div>
              }
            </div>

            <div class="form-control">
              <label class="label" for="streamingService">
                <span class="label-text font-medium">Streaming Service</span>
              </label>
              <input
                id="streamingService"
                type="text"
                list="streaming-services"
                formControlName="streamingService"
                class="input input-bordered w-full"
                [class.input-error]="
                  form.get('streamingService')?.invalid &&
                  form.get('streamingService')?.touched
                "
                placeholder="e.g., Netflix, Hulu, Amazon Prime"
                required
                aria-describedby="streaming-help"
              />
              <div class="label">
                <span class="label-text-alt" id="streaming-help"
                  >Where the show can be watched</span
                >
              </div>
              @if (
                form.get('streamingService')?.invalid &&
                form.get('streamingService')?.touched
              ) {
                <div class="text-error text-sm mt-1">
                  @if (form.get('streamingService')?.errors?.['required']) {
                    <span>Streaming service is required</span>
                  }
                </div>
              }
              <datalist id="streaming-services">
                <option value="Netflix"></option>
                <option value="Hulu"></option>
                <option value="Amazon Prime"></option>
                <option value="Disney+"></option>
                <option value="HBO Max"></option>
                <option value="Apple TV+"></option>
                <option value="Peacock"></option>
                <option value="Paramount+"></option>
                <option value="YouTube TV"></option>
                <option value="Discovery+"></option>
              </datalist>
            </div>

            <div class="form-control mt-8">
              <div class="flex gap-4">
                <button type="submit" class="btn btn-primary btn-lg flex-1">
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-5 w-5 mr-2"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M12 4v16m8-8H4"
                    />
                  </svg>
                  Add Show
                </button>
                <button
                  type="button"
                  class="btn btn-secondary btn-lg flex-1"
                  (click)="router.navigate(['..'])"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-5 w-5 mr-2"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M10 19l-7-7m0 0l7-7m-7 7h18"
                    />
                  </svg>
                  Cancel
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: ``,
})
export class Add {
  store = inject(ShowsStore);
  router = inject(Router);
  form = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100),
      ],
    }),
    description: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.minLength(10),
        Validators.maxLength(500),
      ],
    }),
    streamingService: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    } else {
      const newShow = this.form.value;
      this.store.addShow(newShow);
      this.router.navigate(['..']);
    }
  }
}
