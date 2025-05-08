# AxisCRM

## Visão Geral do Projeto

**AxisCRM** é uma API RESTful para gerenciamento de atendimentos por clientes, construída com C# .NET 8, Entity Framework Core 8 e PostgreSQL. Fornece endpoints para CRUD de usuários, clientes, atendimentos e outros recursos do sistema.

## Tecnologias

- **.NET 8**  
- **Entity Framework Core 8**  
- **PostgreSQL**  
- **AutoMapper** (mapeamento de DTOs)  
- **FluentValidation** (validação de requests)  
- **Swagger** (documentação interativa)  
- **xUnit**, **Moq**, **AutoFixture** (testes unitários e de integração)  
- **VS Code REST Client** (arquivo `AxisCRM.Api.http`)

## Pré-requisitos

1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
2. PostgreSQL (versão suportada: 12+)  
3. CLI do Entity Framework Core v8:
   ```bash
   dotnet tool install --global dotnet-ef --version 8.0.0-*
   ```

