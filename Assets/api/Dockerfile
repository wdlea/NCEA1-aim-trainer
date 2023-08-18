FROM golang:alpine
WORKDIR /go/src/github.com/wdlea/aintrainerapi

COPY go.mod go.mod



COPY main.go main.go

COPY objects objects
COPY boilerplate boilerplate
COPY logic logic



#import external dependencies
COPY go.sum go.sum
RUN go get ./...

RUN go install github.com/wdlea/aimtrainerAPI

EXPOSE 8088

CMD aimtrainerAPI