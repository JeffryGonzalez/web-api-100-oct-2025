import { http, delay, HttpResponse } from 'msw';
import { ShowEntity } from '../shows/shows.store';

const fakeShows: ShowEntity[] = [
  {
    id: '1',
    name: 'Breaking Bad',
    description:
      'A high school chemistry teacher turned methamphetamine producer.',
    streamingService: 'Netflix',
  },
  {
    id: '2',
    name: 'Game of Thrones',
    description:
      'A fantasy drama series based on the novels by George R.R. Martin.',
    streamingService: 'HBO',
  },
  {
    id: '3',
    name: 'Stranger Things',
    description:
      'A group of kids uncovering supernatural mysteries in their town.',
    streamingService: 'Netflix',
  },
  {
    id: '4',
    name: 'The Mandalorian',
    description: 'A lone bounty hunter in the outer reaches of the galaxy.',
    streamingService: 'Disney+',
  },
  {
    id: '5',
    name: 'The Crown',
    description: 'A historical drama about the reign of Queen Elizabeth II.',
    streamingService: 'Netflix',
  },
];

export const showsHandler = [
  http.post('/api/shows', async ({ request }) => {
    const newShow: ShowEntity = (await request.json()) as unknown as ShowEntity;
    newShow.id = crypto.randomUUID(); //mulate ID generation
    fakeShows.push(newShow);
    return HttpResponse.json(newShow, { status: 201 });
  }),
  http.get('api/shows', async () => {
    await delay(1000);
    // return new HttpResponse(null, {
    //   status: 404,
    //   statusText: 'Not Found',
    // });
    return HttpResponse.json(fakeShows);
  }),
];
