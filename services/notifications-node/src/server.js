'use strict';

const http = require('node:http');
const config = require('./config');
const logger = require('./lib/logger');
const { createRouter } = require('./lib/router');
const registerNotificationRoutes = require('./routes/notifications');
const registerWebhookRoutes = require('./routes/webhooks');
const registerReportRoutes = require('./routes/reports');

const router = createRouter();

router.get('/healthz', (req, res) => {
  res.writeHead(200, { 'Content-Type': 'application/json' });
  res.end(JSON.stringify({ status: 'ok' }));
});

registerNotificationRoutes(router);
registerWebhookRoutes(router);
registerReportRoutes(router);

const server = http.createServer((req, res) => {
  router.handle(req, res);
});

server.listen(config.port, () => {
  logger.info('notifications service listening', { port: config.port });
});

module.exports = server;
