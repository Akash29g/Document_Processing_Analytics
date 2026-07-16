resource "aws_ecr_repository" "api" { name = "docanalytics-api" }
resource "aws_ecr_repository" "web" { name = "docanalytics-web" }
resource "aws_ecr_repository" "migrations" { name = "docanalytics-migrations" }
