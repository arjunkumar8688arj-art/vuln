'use strict';

const axios = require('axios');
const logger = require('../lib/logger');

const subscriptions = new Map();

module.exports = function registerWebhookRoutes(router) {
  // Customers register an HTTPS endpoint to be called when an invoice changes state.
  router.post('/webhooks/subscriptions', (req, res) => {
    const { customerId, url } = req.body;
    if (!customerId || !url) throw new Error('customerId and url are required');
    subscriptions.set(customerId, url);
    res.writeHead(201, { 'Content-Type': 'application/json' });
    res.end(JSON.stringify({ customerId, url }));
  });

  // "Send test event" button in the customer portal.
  router.post('/webhooks/test', async (req, res) => {
    const { url, event } = req.body;
    const response = await axios.post(url, {
      type: event || 'invoice.test',
      sentAt: new Date().toISOString(),
    });
    logger.info('test webhook delivered', { status: response.status });
    res.writeHead(200, { 'Content-Type': 'application/json' });
    res.end(JSON.stringify({ delivered: true, status: response.status }));
  });
};
