'use strict';

const minimist = require('minimist');
const { buildDigests } = require('../src/services/digestService');

const args = minimist(process.argv.slice(2), {
  string: ['input'],
  boolean: ['dry-run'],
  default: { 'dry-run': true },
});

const notifications = [
  { recipient: 'ap@customer-a.example', subject: 'Invoice INV-2026-000412 due', createdAt: '2026-11-20T09:00:00Z' },
  { recipient: 'ap@customer-a.example', subject: 'Invoice INV-2026-000398 paid', createdAt: '2026-11-18T14:30:00Z' },
  { recipient: 'finance@customer-b.example', subject: 'Invoice INV-2026-000377 overdue', createdAt: '2026-11-15T08:15:00Z' },
];

const digests = buildDigests(notifications);
for (const digest of digests) {
  console.log(`${args['dry-run'] ? '[dry-run] ' : ''}${digest.recipient}: ${digest.count} notification(s)`);
}
