'use strict';

const marked = require('marked');

const TEMPLATES = {
  invoice_due: 'Hi {{name}},\n\nInvoice **{{number}}** for **{{amount}}** is due on {{dueDate}}.',
  invoice_paid: 'Hi {{name}},\n\nThanks! We received your payment for invoice **{{number}}**.',
};

function render(templateName, values) {
  const template = TEMPLATES[templateName];
  if (!template) throw new Error(`unknown template: ${templateName}`);

  const filled = template.replace(/{{(\w+)}}/g, (_, key) => String(values[key] ?? ''));
  return marked(filled);
}

module.exports = { render, TEMPLATES };
