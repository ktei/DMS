provider "aws" {
  region = "ap-southeast-2"
}

terraform {
  required_version = "= 0.13.5"
  backend "s3" {
    bucket = "terraform-state-397977497739"
    key    = "prod/dms/service/terraform.tfstate"
    region = "ap-southeast-2"
  }
}

data "terraform_remote_state" "infra" {
  backend = "s3"
  config = {
    bucket = "terraform-state-397977497739"
    key    = "prod/infra/terraform.tfstate"
    region = "ap-southeast-2"
  }
}

data "aws_region" "current" {}
data "aws_caller_identity" "current" {}

module "service" {
  source          = "../modules/service"
  appname         = "dms"
  environment     = "prod"
  vpc_id          = data.terraform_remote_state.infra.outputs.vpc_id
  subnets         = data.terraform_remote_state.infra.outputs.public_subnets
  sg_ids          = [data.terraform_remote_state.infra.outputs.ecs_sg_id]
  lb_listener_arn = data.terraform_remote_state.infra.outputs.public_lb_listener_https_arn
  cluster_name    = data.terraform_remote_state.infra.outputs.applications_cluster_name
  task_exec_policy_statements = [
    {
      actions = [
        "secretsmanager:GetSecretValue"
      ],
      resources = [
        "arn:aws:secretsmanager:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:secret:prod_application_rds-6XxZJF"
      ]
    }
  ]
}
