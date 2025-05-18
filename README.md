Steps to run docker

run this command

docker build -t transaction-api .

docker run -d -p 8080:80 --name transaction-api-container transaction-api
