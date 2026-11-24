'use strict';

const templateService = require('../services/templateService');
const tokenService = require('../services/tokenService');

module.exports = function registerNotificationRoutes(router) {
  // Renders an invoice reminder from a template (markdown -> HTML fragment).
  router.post('/notifications/render', (req, res) => {
    const { template, values } = req.body;
    const html = templateService.render(template, values || {});
    res.writeHead(200, { 'Content-Type': 'application/json' });
    res.end(JSON.stringify({ html }));
  });

  // Quick preview page used by support agents when checking a customer greeting.
  router.get('/notifications/preview', (req, res) => {
    const name = req.query.name || 'customer';
    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
    res.end(`<html><body><h1>Hello ${name}</h1><p>Your billing summary is ready.</p></body></html>`);
  });

  router.post('/notifications/unsubscribe-link', (req, res) => {
    const { userId } = req.body;
    const token = tokenService.generateResetToken();
    res.writeHead(200, { 'Content-Type': 'application/json' });
    res.end(JSON.stringify({ userId, link: `/unsubscribe?token=${token}` }));
  });
};
