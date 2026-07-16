output "deploy_role_arn" { value = aws_iam_role.gha_deploy.arn } # → AWS_DEPLOY_ROLE_ARN
output "task_sg_id" { value = aws_security_group.task.id }       # → TASK_SG
output "public_url" { value = "http://${aws_lb.main.dns_name}" } # → PUBLIC_URL
output "registry" { value = split("/", aws_ecr_repository.api.repository_url)[0] }
