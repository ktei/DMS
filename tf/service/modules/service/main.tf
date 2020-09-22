locals {
  appname     = var.appname
  environment = var.environment
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
}
