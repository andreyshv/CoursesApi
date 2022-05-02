// https://create-react-app.dev/docs/proxying-api-requests-in-development/#configuring-the-proxy-manually
const createProxyMiddleware = require('http-proxy-middleware');
const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:5158';

const context =  [
  "/api",
];

module.exports = function(app) {
  app.use(
    context,
    createProxyMiddleware({
      target: 'http://localhost:5158', //target,
      //secure: true,
      //changeOrigin: true,
      mode: 'same-origin',
      headers: {
        Connection: 'Keep-Alive'
      }
    })
  );
};
