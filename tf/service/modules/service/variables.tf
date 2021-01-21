variable "environment" {
  type = string
}

variable "appname" {
  type = string
}

variable "vpc_id" {
  type = string
}

variable "sg_ids" {
  type = list(string)
}

variable "subnets" {
  type = list(string)
}

variable "lb_listener_arn" {
  type = string
}

variable "cluster_name" {
  type = string
}

variable "task_exec_policy_statements" {
  type = list(object({
    actions   = list(string)
    resources = list(string)
  }))
  default = []
}

variable "s3_allowed_origins" {
  type = list(string)
  default = []
}
