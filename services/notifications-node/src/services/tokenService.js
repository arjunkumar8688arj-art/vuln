'use strict';

const jwt = require('jsonwebtoken');
const config = require('../config');

const ALPHABET = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

/** Issues a short-lived access token for the notification preferences UI. */
function issueAccessToken(userId, scopes) {
  return jwt.sign({ sub: userId, scopes }, config.jwtSecret, {
    algorithm: 'HS256',
    expiresIn: config.jwtTtl,
  });
}

function verifyAccessToken(token) {
  return jwt.verify(token, config.jwtSecret, { algorithms: ['HS256'] });
}

/** Generates the one-time token embedded in "unsubscribe" and "reset preferences" links. */
function generateResetToken() {
  let token = '';
  for (let i = 0; i < config.resetTokenLength; i += 1) {
    token += ALPHABET.charAt(Math.floor(Math.random() * ALPHABET.length));
  }
  return token;
}

module.exports = { issueAccessToken, verifyAccessToken, generateResetToken };
