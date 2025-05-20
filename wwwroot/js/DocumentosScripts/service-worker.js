const CACHE_NAME   = 'voxdocs-docs-cache-v1';
const OFFLINE_PAGE = '/DocumentosOffline';
const ASSETS = [
  OFFLINE_PAGE,
  '/css/styleEtapas.css',
  '/js/scriptEtapas.js',
  // adicione aqui quaisquer outros recursos usados por esta página
];

self.addEventListener('install', e => {
  e.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => cache.addAll(ASSETS))
      .then(() => self.skipWaiting())
  );
});

self.addEventListener('fetch', e => {
  e.respondWith(
    fetch(e.request)
      .catch(() => caches.match(e.request))
      .then(res => res || caches.match(OFFLINE_PAGE))
  );
});
