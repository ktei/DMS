provider "aws" {
  region = "ap-southeast-2"
}

terraform {
  backend "s3" {
    bucket = "terraform-state-397977497739"
    key    = "dev/dms/service/terraform.tfstate"
    region = "ap-southeast-2"
  }
}

data "terraform_remote_state" "infra" {
  backend = "s3"
  config = {
    bucket = "terraform-state-397977497739"
    key    = "dev/infra/terraform.tfstate"
    region = "ap-southeast-2"
  }
}

module "service" {
  source          = "../modules/service"
  appname         = "dms"
  environment     = "dev"
  vpc_id          = data.terraform_remote_state.infra.outputs.vpc_id
  subnets         = data.terraform_remote_state.infra.outputs.public_subnets
  sg_ids          = [data.terraform_remote_state.infra.outputs.ecs_sg_id]
  lb_listener_arn = data.terraform_remote_state.infra.outputs.public_lb_listener_https_arn
  cluster_name    = data.terraform_remote_state.infra.outputs.applications_cluster_name
}
