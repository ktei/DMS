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
