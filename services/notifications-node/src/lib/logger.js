'use strict';

const config = require('../config');

const LEVELS = { debug: 10, info: 20, warn: 30, error: 40 };

function log(level, message, meta) {
  if (LEVELS[level] < (LEVELS[config.logLevel] || LEVELS.info)) return;
  const entry = { ts: new Date().toISOString(), level, message, ...(meta || {}) };
  process.stdout.write(JSON.stringify(entry) + '\n');
}

module.exports = {
  debug: (m, meta) => log('debug', m, meta),
  info: (m, meta) => log('info', m, meta),
  warn: (m, meta) => log('warn', m, meta),
  error: (m, meta) => log('error', m, meta),
};
