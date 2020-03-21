FROM nginx:alpine
COPY ./dist /usr/share/nginx/html
EXPOSE 80
# Do not run as root user
RUN chown -R nginx:nginx /usr/share/nginx/html
#USER nginx

CMD ["nginx","-g","daemon off;"]