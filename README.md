# 🚀 Simple Billing – Demo Project

## Table of Contents

- [Built With](#built-with)
- [Overview](#overview)
- [Summary](#summary)
- [Architectural Project](#architectural-project)
- [Main Tables](#main-tables)
- [Key Features](#key-features)
- [Objetive](#objetive)
- [How To Run](#how-to-run)
- [Functional Modules of the System](#functional-modules-of-the-system)
- [Development URL](#development-url)
- [Response Format](#response-format)
- [Error Handling](#error-handling)
- [Rate Limiting](#rate-limiting)
- [Pagination](#pagination)
- [Deployment](#deployment) 
- [Installation](#installation)
- [Environment configurationlation](#environment-configuration)
- [Usage of deploy script](#usage-of-deploy-script)
- [Vault integration](#vault-integration)
- [Consul integration](#consul-integration)
- [General Flow](#general-flow)
- [Routing Table](#routing-table) 
- [Endpoints](#endpoints)
  - [Billing API](#general)
    - [Health](#health)
    - [Post](#post)    
    - [Gets](#gets)   
    - [Put](#put)       
    - [Get](#get)     
- [Data Models](#data-models)
- [Usage Examples](#usage-examples)
- [SDKs and Libraries](#sdks-and-libraries)
- [v1.0.0 — Initial Production-Ready Runtime](#initial-production-ready-runtime)

# 🛠 Built With <a id="built-with"></a>

------------------------------------------------------------------------

## ⚙️ Tools & Technologies

![C#](https://img.shields.io/badge/Code-C%23-512bd4?style=for-the-badge&logo=csharp&logoColor=white)
![Python](https://img.shields.io/badge/Code-Python-3776ab?style=for-the-badge&logo=python&logoColor=white)
![Docker](https://img.shields.io/badge/Container-Docker-2496ed?style=for-the-badge&logo=docker&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-336791?style=for-the-badge&logo=postgresql&logoColor=white)

------------------------------------------------------------------------

## 📊 Monitoring & Logging  

![Grafana](https://img.shields.io/badge/Dashboard-Grafana-f28e1c?style=for-the-badge&logo=grafana&logoColor=white)
![Prometheus](https://img.shields.io/badge/Metrics-Prometheus-e6522c?style=for-the-badge&logo=prometheus&logoColor=white)
![Alloy](https://img.shields.io/badge/Collector-Alloy-4c6ef5?style=for-the-badge&logo=grafana&logoColor=white)
![Loki](https://img.shields.io/badge/Logs-Loki-8e44ad?style=for-the-badge&logo=grafana&logoColor=white)
![Tempo](https://img.shields.io/badge/Tracing-Tempo-1d70b8?style=for-the-badge&logo=grafana&logoColor=white)

------------------------------------------------------------------------

## 🛠️ Observability & Infra  

![RabbitMQ](https://img.shields.io/badge/Messaging-RabbitMQ-ff6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![Consul](https://img.shields.io/badge/Service%20Discovery-Consul-e03875?style=for-the-badge&logo=consul&logoColor=white)
![Vault](https://img.shields.io/badge/Security-Vault-000000?style=for-the-badge&logo=vault&logoColor=white)

------------------------------------------------------------------------

## 🌐 Overview <a id="overview"></a>

This project is a simple implementation of a billing system, developed as part of a training course. Although it consists of only four main tables, it was designed applying modern architectural principles and software patterns, with the aim of serving as a technical portfolio and demonstrating best practices in backend and microservices projects.

## ✨ Summary <a id="summary"></a>
Minimalist billing system with a modern architecture: microservices in .NET 10, messaging with RabbitMQ, and full observability with Prometheus/Grafana. Designed as a training demo and technical portfolio to showcase best practices in CQRS, DDD, and vertical slicing.

## ✨ Architectural Project <a id="architectural-project"></a>
![Architectural Project](images/architectural_project.jpg)

## 📊 Main Tables <a id="main-tables"></a>

- Invoices → Full CRUD operations, aggregation pattern
- InvoiceItems → Details of items associated with invoices
- Auditlog → Audit log for traceability
- Eventrelay → Integration and messaging events

### ✨ Key Features <a id="key-features"></a>

- ✅ API Gateway: ASP.NET Core + YARP
- ✅ Backend microservices: .NET 10, organized by business context
- ✅ Messaging: RabbitMQ
- ✅ Database: PostgreSQL
- ✅ Observability: Prometheus + Grafana + Alloy + Tempo
- ✅ Patterns applied: CQRS, DDD, Vertical Slices

### 🎯 Objetive <a id="objetive"></a>

The purpose of this project is to demonstrate how even a small domain can benefit from a modular and scalable architecture. It serves as a practical example for:

- Training in microservices and design patterns.
- A technical portfolio showcasing expertise in .NET, clean architecture, and DevOps.
- A foundation for extending to more complex billing or ERP scenarios.

### 📦 How to run <a id="how-to-run"></a>

1. Clone the repository.
2. Configure environment variables in docker-compose.override.yml.
3. Start services with Docker Compose.
4. Access the API Gateway at http://localhost:6004/billing-service.

------------------------------------------------------------------------

#### 📦 Functional Modules of the System <a id="functional-modules-of-the-system"></a>

1. Billing
   - Sales Invoices
2. Event Audit
    - Logs, Critical Data Changes

## 🔗 Development URL <a id="development-url"></a>

```abc
Billing.Api: http://localhost:5000
```
------------------------------------------------------------------------

## Response Format <a id="response-format"></a>

All API responses follow a consistent JSON structure:

### Success Response

```json
{
  "data": { /* Response data */ },
  "meta": {
    "timestamp": "2024-01-15T10:30:00Z",
    "version": "1.0"
  }
}
```

### Paginated Response

```json
{
  "pageIndex": 0,
  "pageSize": 10,
  "count": 150,
  "data": [/* Array of items */]
}
```
------------------------------------------------------------------------

## ❌ Error Handling <a id="error-handling"></a>

The API uses standard HTTP status codes and returns detailed error information:

---
### Error Response Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "The request contains invalid parameters",
  "instance": "/billings",
  "errors": {
    "PageSize": ["PageSize must be between 1 and 100"]
  }
}
```
---

### Common HTTP Status Codes


| Code  | Description                              |
| ----- | ---------------------------------------- |
| `200` | OK - Request successful                  |
| `201` | Created - Resource created successfully  |
| `400` | Bad Request - Invalid request parameters |
| `401` | Unauthorized - Authentication required   |
| `403` | Forbidden - Insufficient permissions     |
| `404` | Not Found - Resource not found           |
| `409` | Conflict - Resource already exists       |
| `422` | Unprocessable Entity - Validation failed |
| `500` | Internal Server Error - Server error     |

------------------------------------------------------------------------

## ⏱️ Rate Limiting <a id="rate-limiting"></a>

> **Note:** Add your rate limiting information here
- **Rate Limit:** 1000 requests per hour per API key
- **Headers:** Check `X-RateLimit-Remaining` and `X-RateLimit-Reset` headers

------------------------------------------------------------------------

## 📄 Pagination <a id="pagination"></a>

All endpoints that return lists support pagination:

**Parameters:**

- `Pageindex`: page number (base 0, default: 0)
- `Pagesize`: Elements per page (default: 10, maximum: 100)

**Response format:**

```json
{
  "pageIndex": 0,
  "pageSize": 10,
  "count": 45,
  "data": [...]
}
```
------------------------------------------------------------------------

## 🚀 Deployment <a id="deployment"></a>

This project implements a deployment system with **Docker Compose + Python scripts** 

### 📂 Folder structure

```bash
    deploy/
    │
    ├── framework/
    │   ├── config/
    │   │   ├── loader.py
    │   │   └── models.py	
    │   │
    │   ├── consul/
    │   │   └── ConsulManager.py		
    │   │	
    │   ├── core/
    │   │   ├── context.py         # Estado compartido (env, clientes, etc.)
    │   │   └── registry.py        # Registro de providers
    │   │
    │   ├── docker/
    │   │   └── compose.py	
    │   │
    │   ├── providers/
    │   │   ├── postgres.py
    │   │   ├── rabbitmq.py
    │   │   ├── consul.py
    │   │   ├── vault.py
    │   │   └── http.py
    │   │
    │   ├── health/
    │   │   ├── postgres.py
    │   │   ├── rabbitmq.py
    │   │   └── http.py
    │   │
    │   └── utils/
    │       ├── config_loader.py
    │       └── resolver.py
    │
    ├── services.yml
    └── main.py
```

------------------------------------------------------------------------

## ⚙️ Installation <a id="installation"></a>

1.  Create Python virtual environment:

    ``` bash
    python -m venv .venv
    source .venv/bin/activate   # Linux/Mac
    .venv\Scripts\activate      # Windowscls
    ```

2.  Install dependencies:

    ``` bash
    pip install -r deploy/requirements.txt
    ```

------------------------------------------------------------------------

## 🌍 Environment configuration <a id="environment-configuration"></a>

The `.env.{environment}` files contain all required variables.

Example **.env.dev**:

``` env
VAULT_ADDR=http://localhost:8200
VAULT_CONTAINER=vault_dev
VAULT_DEV_TOKEN=root

CONSUL_URL=http://localhost:8500
POSTGRES_HOST=accountingdb_dev
POSTGRES_PORT=5432
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=postgres
```

In **Stage/Prod**, `VAULT_DEV_TOKEN` does not apply because Vault is
initialized dynamically.

------------------------------------------------------------------------

## ▶️ Usage of deploy script <a id="usage-of-deploy-script"></a>

### Bring up environment

``` bash
  python deploy/main.py --env dev
```

### Bring down environment

``` bash
  python deploy/main.py --env dev --down
```

### Check status

``` bash  
  python deploy/main.py --env dev ps
```

------------------------------------------------------------------------

## 🔐 Vault integration <a id="vault-integration"></a>

-   **Dev**
    -   Uses `VAULT_DEV_TOKEN` defined in `.env.dev`.
    -   Automatically configures engines and dynamic roles (no
        init/unseal).
-   **Stage / Prod**
    -   Initializes and unseals Vault (`vault operator init`).
    -   Saves unseal keys and root token in
        `deploy/secrets/vault_init_{env}_{timestamp}.json`.
    -   Configures engines and roles same as Dev.

Example: dynamic Postgres credentials (`db-role`).

------------------------------------------------------------------------

## 📌 Consul integration <a id="consul-integration"></a>

-   `ConsulManager.py` registers key services (e.g., PostgreSQL).
-   APIs self-register in Consul when they start.

Example Postgres registration:

``` json
{
  "id": "billingdb",
  "name": "postgres",
  "address": "billingdb",
  "port": 5432,
  "check": {
    "tcp": "billingdb:5432",
    "interval": "10s"
  }
}
```

------------------------------------------------------------------------

## ✅ General Flow (v1.0.0) <a id="general-flow"></a>

1. **Load configuration**

   * `services.yml` is loaded and validated via Pydantic.
   * Environment variables are resolved (`.env` or runtime environment).

2. **Start infrastructure**

   * Services are started using `docker compose` (external step or wrapper).
   * Includes databases, message brokers, APIs, and workers.

3. **Wait for infrastructure readiness**

   * The orchestrator waits for each service based on its type:

     * `postgres` → TCP connection check
     * `rabbitmq` → management/API or port check
     * `http` → HTTP endpoint validation
     * `worker` → skipped (no health check required)
   * Ensures dependencies are available before continuing.

4. **Vault bootstrap (idempotent)**

   * Vault is initialized and unsealed (if required).
   * Engines are enabled dynamically:

     * Database engine (Postgres)
     * RabbitMQ engine
   * Roles and connections are configured per service.
   * Dynamic credentials become available for consumers.

5. **Consul service registration**

   * Services marked with `consul.enabled=true` are registered.
   * Service names and IDs are resolved automatically.
   * Health checks are attached when applicable.

6. **System health evaluation**

   * A unified health check pass is executed across all services.
   * Each service is evaluated using its corresponding checker.
   * Results are aggregated into a system health summary.

7. **Deployment result**

   * A consolidated health report is printed.
   * Deployment is marked successful if no critical failures are detected.

---

## 🧠 Notes

* The flow is **idempotent** and safe to re-run.
* Vault and Consul integrations are fully automated (no manual UI steps required).
* The system is designed for **On-Premise environments** with minimal external dependencies.
* Workers are treated as **fire-and-forget processes**, not requiring active health validation.

---

## 🎯 Outcome

A fully initialized environment with:

* Running infrastructure (DB, MQ, APIs, Workers)
* Dynamic secrets managed by Vault
* Service discovery via Consul
* Verified service health and readiness

---

------------------------------------------------------------------------

## ⚠️ Recommendations

-   In **Stage/Prod**, upload `vault_init_{env}_{timestamp}.json` to a
    secure manager (e.g., AWS Secrets Manager).
-   Do not version `.env.*` or files inside `deploy/secrets/`.
-   Use `requirements.txt` with pinned versions to guarantee
    reproducibility.

------------------------------------------------------------------------

## 📑 Routing Table <a id="routing-table"></a>

| Path in Gateway      | Microservice      | Local Env | Docker Env | Docker Inside |
|----------------------|-------------------|-----------|------------|---------------|
| `/billing`           | Billing           | 5000–5050 | 6000–6060  | 8080-8081     |
| `Yarp.ApiGw   `      | Yarp Api Gateway  | 5001–6001 | 6004–6064  | 8080-8081     |

------------------------------------------------------------------------

# 🔗 Endpoints <a id="endpoints"></a>

## 📊 Billing API <a id="general"></a>

------------------------------------------------------------------------

### Health <a id="health"></a>

```http
GET /health
```

**Example of application:**

```bash
  curl -sS -H "Accept: application/json" https://localhost:5050/health
```

------------------------------------------------------------------------

#### Post <a id="post"></a>

```http
POST /billings
```

**Parameters:**


| Parameter          | Type    | Required | Description               |
| ------------------ | ------- | -------- | ------------------------- |
| `number`           | string  | Yes      | Internal invoice number   |
| `issueDate`        | Date    | Yes      | Date of issue             |
| `customerId`       | UUID    | No       | Client ID                 |
| `description`      | string  | Yes      | Item description          |
| `quantity`         | int     | Yes      | Quantity of item          |
| `price`            | string  | Yes      | Price unit                |
| `lineNumber`       | string  | No       | line Number               |
-----------------------------------------------------------------------

**Body of the Request:**

```json
{
  "Invoice": {
    "number": "INV-20260519-001",
    "issueDate": "2026-05-19T00:29:43.018Z",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "lines": [
      {
        "description": "Servicio de consultoría",
        "quantity": 10,
        "price": 150.00,
        "lineNumber": 1
      },
      {
        "description": "Licencia de software",
        "quantity": 2,
        "price": 500.00,
        "lineNumber": 2
      }
    ]
  }
}
```

**Validation Rules:**

- ✅ At least one lines are required

**Sample Application:**

```bash
 curl -X 'POST' 
  'http://localhost:5000/billings' 
  -H 'accept: application/json' 
  -H 'Content-Type: application/json' 
  -d '{
  "apInvoice": {
    "number": "INV-20260519-001",
    "issueDate": "2026-05-19T00:29:43.018Z",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "lines": [
      {
        "description": "Servicio de consultoría",
        "quantity": 10,
        "price": 150.00,
        "lineNumber": 1
      },
      {
        "description": "Licencia de software",
        "quantity": 2,
        "price": 500.00,
        "lineNumber": 2á
      }
    ]
  }
}
'
```

**Sample Answer:**

```json
{
   "id": "123e4567-e89b-12d3-a456-426614174004"
}
```

------------------------------------------------------------------------

#### Gets <a id="gets"></a>

```http
GET /billings?PageIndex=0&PageSize=10
```

**Parameters:**

| Parameter   | Type    | Required | Description       |
| ----------- | ------- | -------- | ----------------- |
| `PageIndex` | integer | 0        | Page number       |
| `PageSize`  | integer | 10       | Elements per page |


**Sample Application:**

```bash
 curl -X 'GET' 
  'http://localhost:5000/billings?PageIndex=10&PageSize=1' 
  -H 'accept: application/json'
```

**Sample Answer:**

```json
{
  "invoices": {
    "pageIndex": 0,
    "pageSize": 10,
    "count": 1,
    "data": [
      {
        "id": "b45e75af-fe29-4c2e-9653-aad9bf5b42ef",
        "number": "INV-639162751332422969",
        "issueDate": "2026-06-05T16:52:13.241709Z",
        "customerId": "e44ed594-272c-4978-a3b5-11fb47e9ca12",
        "total": 97500.00,
        "lines": [
          {
            "id": "71f1eeb1-198b-406d-8fb7-d08e04a595e6",
            "invoiceId": "b45e75af-fe29-4c2e-9653-aad9bf5b42ef",
            "description": "Cubo cargador 10 Watts",
            "quantity": 1,
            "price": 4500.00,
            "lineNumber": 3,
            "total": 4500.00
          },
          {
            "id": "8fa3d4dc-fe22-401d-bd85-eeb2d972dd2f",
            "invoiceId": "b45e75af-fe29-4c2e-9653-aad9bf5b42ef",
            "description": "Funda protectora IPhone XR 10",
            "quantity": 1,
            "price": 8000.00,
            "lineNumber": 2,
            "total": 8000.00
          },
          {
            "id": "fa6485c1-d362-4e47-92f9-2fe6c5bddf96",
            "invoiceId": "b45e75af-fe29-4c2e-9653-aad9bf5b42ef",
            "description": "IPhone XR 10",
            "quantity": 1,
            "price": 85000.00,
            "lineNumber": 1,
            "total": 85000.00
          }
        ]
      }
    ]
  }
}
```

------------------------------------------------------------------------

#### PUT <a id="put"></a>

```http
PUT /posting-templates
```

**Parameters:**

| Parameter          | Type    | Required | Description               |
| ------------------ | ------- | -------- | ------------------------- |
| `Id        `       | UUID    | No       | Invoice Id                |                 
| `number`           | string  | Yes      | Internal invoice number   |
| `issueDate`        | Date    | Yes      | Date of issue             |
| `customerId`       | UUID    | No       | Client ID                 |
| `description`      | string  | Yes      | Item description          |
| `quantity`         | int     | Yes      | Quantity of item          |
| `price`            | string  | Yes      | Price unit                |
| `lineNumber`       | string  | No       | line Number               |
------------------------------------------------------------------------

**Validation Rules:**

- ✅ At least two lines are required

**Sample Application:**

```bash
 curl -X 'PUT' 
  'http://localhost:5000/billings' 
  -H 'accept: application/json' 
  -H 'Content-Type: application/json' 
  -d '{
   "invoice":{
      "id":"b45e75af-fe29-4c2e-9653-aad9bf5b42ef",
      "number":"INV-639147467845222848-UPDATED",
      "issueDate":"2026-05-20T09:30:00.000Z",
      "customerId":"e44ed594-272c-4978-a3b5-11fb47e9ca12",
      "lines":[
         {
            "id":"c7b8ea78-9994-4b13-b5a6-cd4d1f55dc01",
            "description":"IPhone XR 10 (ajustado)",
            "quantity":1,
            "price":87000.00,
            "lineNumber":1
         },
         {
            "id":"7fa2d198-a132-4126-b2fb-d39fd1d1ef9b",
            "description":"Funda protectora IPhone XR 10",
            "quantity":2,
            "price":7500.00,
            "lineNumber":2
         },
         {
            "description":"Cable USB-C original",
            "quantity":1,
            "price":2500.00,
            "lineNumber":3
         },
         {
            "description":"Protector de pantalla vidrio templado",
            "quantity":1,
            "price":3000.00,
            "lineNumber":4
         }
      ]
   }
}'
```

**Sample Answer:**

```json
{
  "isSuccess": true
}
```

------------------------------------------------------------------------

#### GET <a id="get"></a>

```http
Get /billings/{id}
```

**Parameters:**

| Parameter          | Type    | Required | Description   |
| ------------------ | ------- | -------- | --------------|
| `id`               | UUID    | Yes      | Invoice ID    |
-----------------------------------------------------------

**Sample Application:**

```bash
 curl -X 'GET' 
  'http://localhost:5000/billings?PageIndex=10&PageSize=1' 
  -H 'accept: application/json'
```

**Sample Answer:**

```json
{
  "invoice": {
    "id": "b45e75af-fe29-4c2e-9653-aad9bf5b42ef",
    "number": "INV-639162751332422969",
    "issueDate": "2026-06-05T16:52:13.241709Z",
    "customerId": "e44ed594-272c-4978-a3b5-11fb47e9ca12",
    "total": 97500.00,
    "lines": [
      {
        "id": "71f1eeb1-198b-406d-8fb7-d08e04a595e6",
        "invoiceId": "b45e75af-fe29-4c2e-9653-aad9bf5b42ef",
        "description": "Cubo cargador 10 Watts",
        "quantity": 1,
        "price": 4500.00,
        "lineNumber": 3,
        "total": 4500.00
      },
      {
        "id": "8fa3d4dc-fe22-401d-bd85-eeb2d972dd2f",
        "invoiceId": "b45e75af-fe29-4c2e-9653-aad9bf5b42ef",
        "description": "Funda protectora IPhone XR 10",
        "quantity": 1,
        "price": 8000.00,
        "lineNumber": 2,
        "total": 8000.00
      },
      {
        "id": "fa6485c1-d362-4e47-92f9-2fe6c5bddf96",
        "invoiceId": "b45e75af-fe29-4c2e-9653-aad9bf5b42ef",
        "description": "IPhone XR 10",
        "quantity": 1,
        "price": 85000.00,
        "lineNumber": 1,
        "total": 85000.00
      }
    ]
  }
}
```

------------------------------------------------------------------------

## 📊 Data Models <a id="data-models"></a>

### CreateInvoiceRequest

```C#
public class CreateInvoiceRequest
{
    public InvoiceDto Invoice { get; set; }
}

```
------------------------------------------------------------------------

### CreateInvoiceResponse

```C#
public class CreateInvoiceResponse
{
    public Guid Id { get; set; }
}
```
------------------------------------------------------------------------

### GetInvoiceByIdResponse

```C#
public class GetInvoiceByIdResponse
{
    public InvoiceDto Invoice { get; set; }
}
```
------------------------------------------------------------------------

### GetInvoicesResponse

```C#
public class GetInvoicesResponse
{
    public InvoiceDtoPaginatedResult Invoices { get; set; }
}
```
------------------------------------------------------------------------

### UpdateInvoiceRequest

```C#
public class UpdateInvoiceRequest
{
    public InvoiceDto Invoice { get; set; }
}
```
------------------------------------------------------------------------

### UpdateInvoiceResponse

```C#
public class UpdateInvoiceResponse
{
    public bool IsSuccess { get; set; }
}
```
------------------------------------------------------------------------

### InvoiceDto

```C#
public class InvoiceDto
{
    public Guid Id { get; set; }
    public string? Number { get; set; }
    public DateTime IssueDate { get; set; }
    public Guid? CustomerId { get; set; }
    public double Total { get; set; }
    public List<InvoiceLineDto>? Lines { get; set; }
}

```
------------------------------------------------------------------------

### InvoiceDtoPaginatedResult

```C#
public class InvoiceDtoPaginatedResult
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public long Count { get; set; }
    public List<InvoiceDto>? Data { get; set; }
}
```
------------------------------------------------------------------------

### InvoiceLineDto

```C#
public class InvoiceLineDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public int LineNumber { get; set; }
    public double Total { get; set; }
}
```
------------------------------------------------------------------------

### ProblemDetails

```C#
public class ProblemDetails
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
}
```
------------------------------------------------------------------------

## SDKs and Libraries

### Official SDKs

- **Python**: `pip install billing-api-client`
- **C#/.NET**: `dotnet add package billing.Client`
------------------------------------------------------------------------

### Example with JavaScript SDK

```javascript
const apiUrl = "http://localhost:5000/billings";

const invoicePayload = {
  Invoice: {
    number: "INV-20260519-001",
    issueDate: "2026-05-19T00:29:43.018Z",
    customerId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    lines: [
      {
        description: "Servicio de consultoría",
        quantity: 1,
        price: 15000,
        lineNumber: 1
      },
      {
        description: "Licencia de software",
        quantity: 2,
        price: 500.00,
        lineNumber: 2
      }
    ]
  }
};

async function createInvoice() {
  try {
    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Accept": "application/json",
        "Content-Type": "application/json"
      },
      body: JSON.stringify(invoicePayload)
    });

    if (!response.ok) {
      throw new Error(`Error: ${response.status} - ${response.statusText}`);
    }

    const result = await response.json();
    console.log("Invoice created successfully:", result);
  } catch (error) {
    console.error("Failed to create invoice:", error);
  }
}

createInvoice();
```
------------------------------------------------------------------------

### Error Validation Example

```bash
# Attempt to create unbalanced settlement (must ≠ have)
curl -X 'POST' \
  'http://localhost:5000/billings' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "Invoice": {
    "Number": "INV-001",
    "issueDate": "2026-05-19T00:29:43.018Z",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "lines": [
      {
        "description": "Servicio de consultoría",
        "quantity": 0,
        "price": 0,
        "lineNumber": 1
      },
      {
        "description": "Licencia de software",
        "quantity": 2,
        "price": 500.00,
        "lineNumber": 2
      }
    ]
  }
}'

# Error response:
{
  "title": "ValidationException",
  "status": 500,
  "detail": "Validation failed: \r\n -- Invoice.Number: Invoice number must be longer than 10 characters Severity: Error\r\n -- Invoice.Lines[0]: El precio de la línea debe ser mayor a cero. Severity: Error\r\n -- Invoice.Lines[0]: Cantidad del Item debe de ser al menos 1. Severity: Error",
  "instance": "/billings",
  "traceId": "0HNM35J7FD0N2:00000005"
}
```
------------------------------------------------------------------------

# 🚀 v1.0.0 — Initial Production-Ready Runtime <a id="initial-production-ready-runtime"></a>

This release marks the first stable version of the declarative deployment runtime.

## ✨ Highlights

- ✅ Declarative service model via `services.yml`
- ✅ Full infrastructure orchestration (Postgres, RabbitMQ, HTTP, Workers)
- ✅ Integrated Vault bootstrap (database + RabbitMQ engines)
- ✅ Dynamic secrets with real-time credential injection
- ✅ Consul service registration with health checks
- ✅ Multi-type health checking system (DB, MQ, HTTP, Worker-aware)
- ✅ Clean orchestrator flow (wait → bootstrap → register → verify)
- ✅ Cross-environment support (Windows / Linux-ready)
- ✅ Designed for On-Premise deployments

## 🔐 Security

- Dynamic database credentials via Vault
- Lease-based secret lifecycle
- Ready for TTL + renewal strategies

## 🧠 Architecture

- Strong separation of concerns (config / runtime / infra)
- Extensible service model (Pydantic-based)
- Pluggable health check system
- Idempotent Vault provisioning

## ⚙️ Supported Service Types

- postgres
- rabbitmq
- http
- worker

## 🚧 Known Constraints (Accepted for v1)

- Docker host resolution differs between Windows and Linux (`host.docker.internal`)
- No centralized logging/metrics yet
- No automated lease renewal (handled at app layer)

## 🎯 Purpose

This runtime is intended as a reusable foundation for:

- Microservices platforms
- On-premise deployments
- Internal developer platforms (IDP)
- Future projects like Axenta

---

## 🏁 Final Notes

This is not a demo — it is a production-grade foundation.

The system is stable, extensible, and ready to be reused across projects.

Further improvements (observability, scaling, advanced orchestration) can be layered on top without breaking the core design.

---

🔥 Ready for real-world usage.

------------------------------------------------------------------------

## Support
- **GitHub**: [https://github.com/sgoni/Billing.git](https://github.com/sgoni/Billing.git)

------------------------------------------------------------------------

*Last updated: June 04, 2026*
*Document version: 1.0.0*