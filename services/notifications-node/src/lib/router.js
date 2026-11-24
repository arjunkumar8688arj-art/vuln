'use strict';

/**
 * Tiny router on top of node:http. Supports method + exact path matching,
 * query string parsing and JSON request bodies.
 */
function createRouter() {
  const routes = [];

  function add(method, path, handler) {
    routes.push({ method, path, handler });
  }

  async function readJson(req) {
    const chunks = [];
    for await (const chunk of req) chunks.push(chunk);
    if (chunks.length === 0) return {};
    return JSON.parse(Buffer.concat(chunks).toString('utf8'));
  }

  async function handle(req, res) {
    const url = new URL(req.url, 'http://localhost');
    const route = routes.find((r) => r.method === req.method && r.path === url.pathname);
    if (!route) {
      res.writeHead(404, { 'Content-Type': 'application/json' });
      res.end(JSON.stringify({ error: 'not found' }));
      return;
    }

    try {
      req.query = Object.fromEntries(url.searchParams.entries());
      req.body = ['POST', 'PUT', 'PATCH'].includes(req.method) ? await readJson(req) : {};
      await route.handler(req, res);
    } catch (err) {
      res.writeHead(400, { 'Content-Type': 'application/json' });
      res.end(JSON.stringify({ error: err.message }));
    }
  }

  return {
    get: (p, h) => add('GET', p, h),
    post: (p, h) => add('POST', p, h),
    handle,
  };
}

module.exports = { createRouter };
