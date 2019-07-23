FROM docker:latest
RUN apk add --update --no-cache \
  curl \
  python \
  which \
  bash
RUN curl -sSL https://sdk.cloud.google.com | bash
ENV PATH $PATH:/root/google-cloud-sdk/bin
RUN gcloud components install docker-credential-gcr
RUN gcloud components install beta
RUN gcloud auth configure-docker
