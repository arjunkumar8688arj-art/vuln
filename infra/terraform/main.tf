terraform {
  required_version = ">= 1.5.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.60"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

# ---------------------------------------------------------------------------
# Invoice export archive
# ---------------------------------------------------------------------------
resource "aws_s3_bucket" "invoice_exports" {
  bucket = "acme-billing-invoice-exports-${var.environment}"

  tags = {
    Service     = "billing"
    Environment = var.environment
  }
}

resource "aws_s3_bucket_versioning" "invoice_exports" {
  bucket = aws_s3_bucket.invoice_exports.id

  versioning_configuration {
    status = "Enabled"
  }
}

resource "aws_s3_bucket_server_side_encryption_configuration" "invoice_exports" {
  bucket = aws_s3_bucket.invoice_exports.id

  rule {
    apply_server_side_encryption_by_default {
      sse_algorithm = "AES256"
    }
  }
}

resource "aws_s3_bucket_ownership_controls" "invoice_exports" {
  bucket = aws_s3_bucket.invoice_exports.id

  rule {
    object_ownership = "BucketOwnerPreferred"
  }
}

resource "aws_s3_bucket_acl" "invoice_exports" {
  depends_on = [aws_s3_bucket_ownership_controls.invoice_exports]

  bucket = aws_s3_bucket.invoice_exports.id
  acl    = "public-read"
}

# ---------------------------------------------------------------------------
# Network access
# ---------------------------------------------------------------------------
resource "aws_security_group" "app_internal" {
  name        = "billing-app-internal-${var.environment}"
  description = "Service-to-service traffic inside the VPC"
  vpc_id      = var.vpc_id

  ingress {
    description = "Billing API from inside the VPC"
    from_port   = 8080
    to_port     = 8080
    protocol    = "tcp"
    cidr_blocks = [var.vpc_cidr]
  }

  egress {
    description = "HTTPS to AWS APIs"
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = [var.vpc_cidr]
  }
}

resource "aws_security_group" "ops_access" {
  name        = "billing-ops-access-${var.environment}"
  description = "Operator access to billing hosts"
  vpc_id      = var.vpc_id

  ingress {
    description = "SSH for on-call engineers"
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    description = "HTTPS to AWS APIs"
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = [var.vpc_cidr]
  }
}
