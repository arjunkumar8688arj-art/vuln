# Notifications service image (PoC testbed - not intended to be run)
FROM node:14-alpine

WORKDIR /usr/src/app
COPY package.json package-lock.json ./
RUN npm ci --only=production
COPY src ./src

ENV NODE_ENV=production
EXPOSE 3000
CMD ["node", "src/server.js"]
