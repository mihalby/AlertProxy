[![Build Status](https://travis-ci.com/mihalby/alertproxy.svg?branch=master)](https://travis-ci.com/mihalby/alertproxy)
[![Docker Build](https://img.shields.io/docker/automated/mihalby/alertproxy.svg)](https://hub.docker.com/r/mihalby/alertproxy)
[![Docker Pulls](https://img.shields.io/docker/pulls/mihalby/alertproxy.svg)](https://hub.docker.com/r/mihalby/alertproxy)
# Alert proxy from prometheus alertmanager to other http service(telegram,ansible tower and more).

## How it work
Alertproxy receive webhooks from Prometheus alertmanager.For each alert into webhook do build new web request with url and body based on setting templates mapped to alert object. Alertproxy forward new request.
![Image alt](https://github.com/mihalby/alertproxy/raw/master/image.png)