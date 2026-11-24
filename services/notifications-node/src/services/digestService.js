'use strict';

const _ = require('lodash');

/** Groups pending notifications per recipient and sorts them newest-first. */
function buildDigests(notifications) {
  const byRecipient = _.groupBy(notifications, 'recipient');
  return _.map(byRecipient, (items, recipient) => ({
    recipient,
    count: items.length,
    items: _.orderBy(items, ['createdAt'], ['desc']),
  }));
}

module.exports = { buildDigests };
