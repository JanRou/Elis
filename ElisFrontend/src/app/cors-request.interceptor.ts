import { HttpInterceptorFn } from '@angular/common/http';

// TODO bruges ikke for nu, dog kan den blive nyttig ifm. autorisation
export const corsRequestInterceptor: HttpInterceptorFn = (req, next) => {
  req = req.clone({
    withCredentials: true,        
  });
  return next(req);
};
