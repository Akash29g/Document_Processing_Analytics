variable "region" { default = "ap-south-1" }
variable "account_id" { type = string }
variable "vpc_id" { type = string }
variable "private_subnets" { type = list(string) }
variable "public_subnets" { type = list(string) }
variable "github_repo" { default = "Akash29g/Document_Processing_Analytics" }
variable "rds_sg_id" { type = string } # docanalytics-db-sg
variable "secret_name" { default = "docanalytics/rds-conn" }
variable "secret_arn" { type = string }

