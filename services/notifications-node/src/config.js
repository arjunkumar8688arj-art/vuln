'use strict';

const config = {
  port: parseInt(process.env.PORT || '3000', 10),
  logLevel: process.env.LOG_LEVEL || 'info',
  billingApiUrl: process.env.BILLING_API_URL || 'http://localhost:8080',
  jwtSecret: process.env.JWT_SECRET || 'acme-notif-hs256-signing-key-9f8e7d6c5b4a39281706f5e4d3c2b1a0',
  jwtTtl: '1h',
  resetTokenLength: 32,
};

module.exports = config;
