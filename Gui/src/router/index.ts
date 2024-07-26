import { App } from 'vue';
import { isArray, isFunction } from 'lodash';
import { createRouter, createWebHashHistory, RouteMeta, Router } from 'vue-router';

import { normalizeString } from '@/utils';

let created: boolean = false;

const routes = import.meta.glob('./routes/**', { eager: true });
const middleware = import.meta.glob('./middleware/**', { eager: true });

const createGuiRouter = (app: App): void => {
  if (created) return;

  const router: Router = createRouter({ routes: [], strict: true, history: createWebHashHistory(import.meta.env.BASE_URL) });

  // Register middlewares
  const _middlewares: { [key: string]: any } = {};

  for (const path in middleware) {
    if (Object.prototype.hasOwnProperty.call(middleware, path)) {
      const name = normalizeString('path', path);
      const locale = (middleware[path] as any).default;

      try {
        locale && (
          _middlewares[name] = locale
          // logger.log(LogLevel.Step, `${name} middleware has been loaded in memory`)
        );
      } catch (error) {
        // logger.log(LogLevel.Error, `error while loading ${name} middleware in memory: ${getExceptionMessage(error as Error)}`, (error as Error)!.stack);
        throw error;
      }
    }
  }

  // Register routes
  for (const path in routes) {
    if (Object.prototype.hasOwnProperty.call(routes, path)) {
      const name = normalizeString('path', path);
      const route = (routes[path] as any).default;

      try {
        route && (
          router.addRoute(name, route)
          // logger.log(LogLevel.Step, `'${name}' route has been registered in router`)
        );
      } catch (error) {
        // logger.log(LogLevel.Error, `error while registering '${name}' router module: ${getExceptionMessage(error as Error)}`, (error as Error)!.stack);
        throw error;
      }
    }
  }

  // Apply middleware to routes
  router.beforeEach(async (to, from, next) => {
    const matched = to.matched;
    const length: number = matched.length;

    for (let index = 0; index < length; index++) {
      const meta = matched[index].meta as RouteMeta;

      if (meta) {
        // handling middleware
        if ('middleware' in meta) {
          let middleware = meta.middleware as string[];

          if (typeof middleware === 'string')
            middleware = [middleware];
          else if (!isArray(middleware))
            throw new Error('Invalid route middleware type');

          for (const middlewareName of middleware) {
            if (!(middlewareName in _middlewares)) throw new ReferenceError(`Middleware '${middlewareName}' is not registered`);

            const middlewareFunction = _middlewares[middlewareName];
            if (!isFunction(middlewareFunction)) throw new TypeError(`Middleware '${middlewareName}' is not a function`);

            return middlewareFunction(to, from, next);
          }
        }
      }
    }

    return next();
  });

  app.use(router);
  created = true;
};

export default createGuiRouter;