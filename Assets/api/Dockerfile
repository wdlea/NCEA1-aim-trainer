FROM golang:alpine
WORKDIR /go/src/github.com/wdlea/aintrainerapi

COPY go.mod go.mod

#import external dependencies
COPY go.sum go.sum
RUN go get .

COPY main.go main.go

COPY api api

EXPOSE 8088

CMD go run main.go