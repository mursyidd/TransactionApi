Logs are saved in this path

\bin\Debug\net6.0\Logs

Steps to run docker

run this command

docker build -t transaction-api .

docker run -d -p 8080:80 --name transaction-api-container transaction-api
