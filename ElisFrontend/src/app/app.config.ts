import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { graphqlProvider } from './graphql.provider';
// import { corsRequestInterceptor } from './cors-request.interceptor';

// TODO Ifm. autorisation så skal der en http interceptor til, brug corsRequestInterceptor
export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes), provideAnimations(), provideHttpClient(), graphqlProvider]
  //providers: [provideRouter(routes), provideAnimations()
  //    , provideHttpClient(withInterceptors([corsRequestInterceptor])), graphqlProvider]
};
