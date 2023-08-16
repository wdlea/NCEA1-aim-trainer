FROM golang:alpine
WORKDIR /go/src/github.com/wdlea/aintrainerapi

COPY go.mod go.mod

# COPY go.sum go.sum # i dont need these becuase i dont use external depenencies
# RUN go get .

COPY main.go main.go

COPY api api

EXPOSE 8088

CMD go run main.go