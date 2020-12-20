

[![Build Status](https://travis-ci.com/mihalby/alertproxy.svg?branch=master)](https://travis-ci.com/mihalby/alertproxy)
[![Docker Build](https://img.shields.io/docker/automated/mihalby/alertproxy.svg)](https://hub.docker.com/r/mihalby/alertproxy)
[![Docker Pulls](https://img.shields.io/docker/pulls/mihalby/alertproxy.svg)](https://hub.docker.com/r/mihalby/alertproxy)
# Alert proxy from prometheus alertmanager to other http service(telegram,ansible tower and more).

## How it work
Alertproxy receive webhooks from Prometheus alertmanager.For each alert into webhook do build new web request with url and body based on setting templates mapped to alert object. Alertproxy forward new request.

<p align="center" width="100%">
  <img width="75%" src="https://github.com/mihalby/alertproxy/raw/master/AlertProxy.png">
</p>
Now all forward requests send only POST method with application/json body in UTF-8 encoding.

## How to use. Run.
### Docker

    docker run --privileged=true -e "TZ=Europe/Minsk" -h alertproxy -d -v /opt/alertproxy/logs:/app/logs -v /opt/alertproxy/cfg:/app/cfg -p 8107:8100 --name alertproxy mihalby/alertproxy:latest

*Dont forget make you configs in ./cfg folder.*
### Linux x64
Download .zip from [Releases](https://github.com/mihalby/AlertProxy/releases) unzip, change configs, make file AlertProxy as executable and run
./AlertProxy
### Windows x64
Download archive from [Releases](https://github.com/mihalby/AlertProxy/releases), change configs and run exe file.

### 1. Create configs (settings.json, user.json, serilog.json)
### 1.1. settings.json 
**SSL** - configure ssl. Now service work only with ssl. Place you pfx to ./cfg directory. You will find fake pfx file in repo ./cfg, password 123123.

**Targets** - templates to forwards. Url and Body is mustache templates. [More info about mustache](https://mustache.github.io/mustache.5.html).

example: 
 **target https:/youAlertproxy:8100/alert/kafka-ms** forward request to http://awx.uni.bn/api/v2/job_templates/108/launch/ ... 
**target https:/youAlertproxy:8100/alert/tlg-itretail** forward message to telegram between telegram http api.

     {
      "SSL": {
        "password": "123123",
        "port": 8100,
        "sertificateName": "aspncer.pfx"
      },
      "targets": {
        "kafka-ms": {
          "UrlTemplate": "http://awx.uni.bn/api/v2/job_templates/108/launch/",
          "Bodytemplate": "{\"ask_inventory_on_launch\": false,\"can_start_without_user_input\": true,\"defaults\": {\"extra_vars\": \"\",\"inventory\": {\"id\": 3,\"name\": \"FirstInventory\"}},\"survey_enabled\": false,\"variables_needed_to_start\": [],\"node_templates_missing\": [],\"node_prompts_rejected\": [],\"job_template_data\": {\"id\": 108,\"description\": \"KAFKA-SERVICES-CHECK-RUN\",\"name\": \"KAFKA-SERVICES-CHECK-RUN\"}}",
          "headers": {
            "Accept-Encoding": "gzip,deflate",
            "authorization": "Basic Yneneulewxo"
          }
        },
        "tlg-itretail": {
          "UrlTemplate": "https://api.telegram.org/botBOT-TOKEN/sendMessage",
          "Bodytemplate": "{\"chat_id\": \"-CHAT-ID\", \"text\": \"{{emoji}}<b>ALERT {{status}}</b>\n<code>Instance : {{labels.instance}}\nalert name : {{labels.alertname}}\njob : {{labels.job}}</code>\", \"disable_notification\": false,\"parse_mode\":\"HTML\"}",
          "headers": {
            "Accept-Encoding": "gzip,deflate"
          },
          "firingEmoji": "\\uD83E\\uDD14",
          "resolvingEmoji": "\uD83D\uDE42"
        }
      },
    
      "AllowedHosts": "*"
    }


### 1.2. user.json 
AlertProxy work only with basic authorization.  Example user.json

    [ 
       { 
          "Id":1,
          "FirstName":"Test",
          "LastName":"User",
          "Username":"test",
          "Password":"test"
       }
    ] 
### 1.3. Logging config - serilog.json
example:

    {
      "Serilog": {
        "Using": [
          "Serilog.Sinks.Console",
          "Serilog.Sinks.File"
        ],
        "MinimumLevel": {
          "Default": "Information",
          "Override": {
            "Microsoft": "Information",
            "System": "Information"
          }
        },
        "Enrich": [
          "WithThreadId",
          "WithClientIp",
          "WithClientAgent"
        ],
        "WriteTo": [
          {
            "Name": "Console",
            "Args": {
              "outputTemplate": "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] [{ThreadId}] [{TraceId:l}]  {ClientIp} {ClientAgent} {Message}{NewLine}{Exception}"
            }
          },
          {
            "Name": "File",
            "Args": {
              "outputTemplate": "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] [{ThreadId}] [{TraceId:l} {ClientIp} {ClientAgent}] {Message}{NewLine}{Exception}",
              "path": "./Logs/app.log",
              "rollOnFileSizeLimit": true,
              "fileSizeLimitBytes": "20971520"
    
            }
          }
        ]
      }
    }
    
## Prometheus Alertmanager reciver example config

    - name: 'auto-check-run-kafka-ms'
     webhook_configs:
     - url: 'https://YouAlertProxy:8100/alert/kafka-ms'
       
       http_config:
          basic_auth:
          username: 'test'
          password: 'test'
       tls_config:
          insecure_skip_verify: true

