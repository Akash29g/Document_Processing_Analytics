# ── GitHub OIDC provider ──
resource "aws_iam_openid_connect_provider" "github" {
  url             = "https://token.actions.githubusercontent.com"
  client_id_list  = ["sts.amazonaws.com"]
  thumbprint_list = ["6938fd4d98bab03faadb97b34396831e3780aea1"]
}

# ── Deploy role GitHub Actions assumes ──
data "aws_iam_policy_document" "gha_trust" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    principals {
      type        = "Federated"
      identifiers = [aws_iam_openid_connect_provider.github.arn]
    }

    condition {
      test     = "StringEquals"
      variable = "token.actions.githubusercontent.com:aud"
      values   = ["sts.amazonaws.com"]
    }
    condition {
      test     = "StringLike"
      variable = "token.actions.githubusercontent.com:sub"
      values   = ["repo:${var.github_repo}:ref:refs/heads/main"]
    }
  }
}

resource "aws_iam_role" "gha_deploy" {
  name               = "docanalytics-gha-deploy"
  assume_role_policy = data.aws_iam_policy_document.gha_trust.json
}

data "aws_iam_policy_document" "deploy_perms" {
  statement {
    actions   = ["ecr:GetAuthorizationToken"]
    resources = ["*"]
  }
  statement {
    actions = [
      "ecr:BatchCheckLayerAvailability", "ecr:CompleteLayerUpload", "ecr:InitiateLayerUpload",
      "ecr:PutImage", "ecr:UploadLayerPart", "ecr:BatchGetImage", "ecr:GetDownloadUrlForLayer"
    ]
    resources = ["arn:aws:ecr:${var.region}:${var.account_id}:repository/docanalytics-*"]
  }
  statement {
    actions = [
      "ecs:RegisterTaskDefinition", "ecs:DescribeTaskDefinition", "ecs:UpdateService",
      "ecs:DescribeServices", "ecs:RunTask", "ecs:DescribeTasks"
    ]
    resources = ["*"]
  }
  statement {
    actions   = ["iam:PassRole"]
    resources = [aws_iam_role.task_exec.arn, aws_iam_role.task_role.arn]
  }
}

resource "aws_iam_role_policy" "deploy_perms" {
  name   = "deploy-perms"
  role   = aws_iam_role.gha_deploy.id
  policy = data.aws_iam_policy_document.deploy_perms.json
}

# ── ECS execution role (pull image, read secret, logs) ──
data "aws_iam_policy_document" "ecs_assume" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }

}

resource "aws_iam_role" "task_exec" {
  name               = "docanalytics-task-exec"
  assume_role_policy = data.aws_iam_policy_document.ecs_assume.json
}
resource "aws_iam_role_policy_attachment" "task_exec_managed" {
  role       = aws_iam_role.task_exec.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}
resource "aws_iam_role_policy" "read_secret" {
  name = "read-rds-secret"
  role = aws_iam_role.task_exec.id
  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [{
      Effect = "Allow", Action = ["secretsmanager:GetSecretValue"],

      Resource = "arn:aws:secretsmanager:ap-south-1:323155024771:secret:docanalytics/jwt-key-6WojHu"

    }]
  })
}

# ── ECS task role (runtime: S3/Bedrock later) ──
resource "aws_iam_role" "task_role" {
  name               = "docanalytics-task-role"
  assume_role_policy = data.aws_iam_policy_document.ecs_assume.json
}
