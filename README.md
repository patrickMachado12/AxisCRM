# AxisCRM

## Visão Geral do Projeto

**AxisCRM** é uma API RESTful para gerenciamento de atendimentos de clientes, construída com:
- **Backend**: C# .NET 8 + Entity Framework Core 8 (Code First)
- **Banco de Dados**: PostgreSQL  
- **Frontend**:(as entregues separadamente) Vue 3 + Vuetify 3

O sistema permite CRUD de usuários, clientes, atendimentos e pareceres, com validações, documentação automática e testes automatizados.

---

## Tecnologias

- **.NET 8**  
- **Entity Framework Core 8**  
- **PostgreSQL** (v12+)  
- **AutoMapper** (mapeamento de DTOs)  
- **FluentValidation** (validação desacoplada de requests)  
- **Swagger / Swashbuckle** (UI interativa em `/swagger`)  
- **xUnit**, **Moq**, **AutoFixture** (testes unitários e mocks)  
- **VS Code REST Client** (arquivo `AxisCRM.Api.http` para testar endpoints)

---

## Pré-requisitos

1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
2. PostgreSQL (versão 12 ou superior)  
3. CLI do EF Core v8:
   ```bash
   dotnet tool install --global dotnet-ef --version 8.0.0-*
