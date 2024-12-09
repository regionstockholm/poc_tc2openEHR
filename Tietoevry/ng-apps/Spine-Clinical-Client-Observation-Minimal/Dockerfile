FROM nginx:1.24-alpine
COPY /dist /usr/share/nginx/html/observation-minimal
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]