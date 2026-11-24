'use strict';

const test = require('node:test');
const assert = require('node:assert');
const { buildDigests } = require('../src/services/digestService');

test('groups notifications per recipient, newest first', () => {
  const digests = buildDigests([
    { recipient: 'a@x.example', createdAt: '2026-11-01T00:00:00Z' },
    { recipient: 'a@x.example', createdAt: '2026-11-03T00:00:00Z' },
    { recipient: 'b@x.example', createdAt: '2026-11-02T00:00:00Z' },
  ]);

  assert.strictEqual(digests.length, 2);
  const a = digests.find((d) => d.recipient === 'a@x.example');
  assert.strictEqual(a.count, 2);
  assert.strictEqual(a.items[0].createdAt, '2026-11-03T00:00:00Z');
});
