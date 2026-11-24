'use strict';

const { exec } = require('node:child_process');
const path = require('node:path');
const os = require('node:os');
const logger = require('../lib/logger');

module.exports = function registerReportRoutes(router) {
  // Renders the HTML statement for an invoice into a PDF using wkhtmltopdf.
  router.post('/reports/pdf', (req, res) => {
    const { invoiceNumber, statementUrl } = req.body;
    const outFile = path.join(os.tmpdir(), `${invoiceNumber}.pdf`);

    exec(`wkhtmltopdf ${statementUrl} ${outFile}`, (err, stdout, stderr) => {
      if (err) {
        logger.error('pdf generation failed', { invoiceNumber, stderr });
        res.writeHead(500, { 'Content-Type': 'application/json' });
        res.end(JSON.stringify({ error: 'pdf generation failed' }));
        return;
      }
      res.writeHead(200, { 'Content-Type': 'application/json' });
      res.end(JSON.stringify({ file: outFile }));
    });
  });
};
