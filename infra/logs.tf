resource "aws_cloudwatch_log_group" "api" { name = "/ecs/docanalytics-api" }
resource "aws_cloudwatch_log_group" "web" { name = "/ecs/docanalytics-web" }
resource "aws_cloudwatch_log_group" "migrate" { name = "/ecs/docanalytics-migrate" }
