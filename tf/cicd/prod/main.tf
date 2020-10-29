provider "aws" {
  region = "ap-southeast-2"
}

terraform {
  backend "s3" {
    bucket = "terraform-state-397977497739"
    key    = "prod/dms/cicd/terraform.tfstate"
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

module "dms" {
  source       = "git::https://github.com/pingai-github/terraform-aws-cicd.git?ref=heads/develop"
  cluster_name = data.terraform_remote_state.infra.outputs.applications_cluster_name
  appname      = "dms"
  environment  = "prod"
  repo         = "DialogManagementService"
  branch       = "master"
}
