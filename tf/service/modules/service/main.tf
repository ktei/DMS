locals {
  appname             = var.appname
  environment         = var.environment
  prefixed_appname    = "${var.environment}-${var.appname}"
  bucket_name         = "${local.prefixed_appname}-${data.aws_caller_identity.current.account_id}"
}

data "aws_caller_identity" "current" {}

resource "aws_s3_bucket" "bucket" {
  bucket = local.bucket_name
  acl    = "private"
  versioning {
    enabled = true
  }

  tags = {
    Name        = local.bucket_name
    Environment = local.environment
  }
}

module "service" {
  source                      = "git::https://github.com/pingai-github/terraform-aws-fargate.git?ref=heads/develop"
  appname                     = local.appname
  environment                 = local.environment
  vpc_id                      = var.vpc_id
  sg_ids                      = var.sg_ids
  subnets                     = var.subnets
  lb_listener_arn             = var.lb_listener_arn
  cluster_name                = var.cluster_name
  lb_path                     = "/dms"
  task_file                   = "task.json"
  task_exec_policy_statements = var.task_exec_policy_statements
  policy_statements = [
    {
      actions = [
        "s3:ListBucket"
      ],
      resources = [
        aws_s3_bucket.bucket.arn
      ]
    },
    {
      actions = [
        "s3:GetObject",
        "s3:GetObjectVersion",
        "s3:GetBucketVersioning",
        "s3:PutObject"
      ],
      resources = [
        aws_s3_bucket.bucket.arn,
        "${aws_s3_bucket.bucket.arn}/*"
      ]
    }
  ]
}

