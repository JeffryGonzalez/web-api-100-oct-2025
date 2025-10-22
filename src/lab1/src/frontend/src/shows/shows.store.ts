import { resource } from '@angular/core';
import { signalStore, withMethods, withProps } from '@ngrx/signals';
import { withEntities } from '@ngrx/signals/entities';

export type ShowEntity = {
  id: string;
  name: string;
  description: string;
  streamingService: string;
};
export const ShowsStore = signalStore(
  withEntities<ShowEntity>(),
  withProps(() => ({
    shows: resource<ShowEntity[], unknown>({
      loader: () => fetch('/api/shows').then((res) => res.json()),
    }),
  })),
  withMethods((store) => {
    return {
      addShow: async (show: Partial<ShowEntity>) => {
        await fetch('/api/shows', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(show),
        });
        store.shows.reload();
      },
    };
  }),
);
