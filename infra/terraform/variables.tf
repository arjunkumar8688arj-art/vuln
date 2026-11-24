variable "aws_region" {
  type    = string
  default = "us-east-1"
}

variable "environment" {
  type    = string
  default = "dev"
}

variable "vpc_id" {
  type        = string
  description = "VPC that hosts the billing and notifications services"
}

variable "vpc_cidr" {
  type    = string
  default = "10.20.0.0/16"
}
