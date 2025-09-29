# KrtBank - API de Gerenciamento de Contas Bancárias

Uma API bancária profissional em .NET 8 para gerenciamento de contas de clientes, construída com Clean Architecture, Domain-Driven Design (DDD) e princípios SOLID.

## 🏦 Visão Geral do Projeto

Este projeto demonstra um sistema bancário completo para gerenciamento de contas de clientes com as seguintes funcionalidades:

- **Gerenciamento de Contas**: Operações CRUD para contas de clientes
- **Cache Inteligente**: Sistema de cache unificado com flag de completude
- **Notificações**: Notificações automatizadas para departamentos do banco
- **Validação**: Validação e normalização de CPF
- **Clean Architecture**: Separação de responsabilidades entre camadas
- **Testes Unitários**: 61 testes com cobertura abrangente (xUnit e Moq)

## 🏗️ Arquitetura

### Camadas da Clean Architecture

```
KrtBank/
├── KrtBank.Domain/          # Camada de Domínio
│   ├── Entities/            # Entidades de negócio (Conta)
│   ├── ValueObjects/        # Objetos de valor (Cpf)
│   ├── Enums/              # Enums de domínio (StatusConta)
│   ├── Events/             # Eventos de domínio
│   ├── Utils/              # Utilitários de domínio (CpfValidator)
│   └── Interfaces/         # Interfaces de repositório
├── KrtBank.Application/     # Camada de Aplicação
│   ├── DTOs/               # Objetos de Transferência de Dados
│   ├── Interfaces/         # Interfaces de serviços
│   ├── Services/           # Serviços de lógica de negócio
│   └── Utils/              # Utilitários de aplicação (CpfNormalizer)
├── KrtBank.Infrastructure/  # Camada de Infraestrutura
│   ├── Data/               # Contexto do Entity Framework
│   ├── Repositories/       # Implementações de acesso a dados
│   └── Services/           # Implementações de serviços externos
├── KrtBank.Api/            # Camada de Apresentação
│   └── Controllers/        # Controladores da API REST
└── KrtBank.Tests/          # Camada de Testes
    ├── Services/           # Testes unitários de serviços
    └── Utils/              # Testes de utilitários
```

## 🚀 Funcionalidades

### Funcionalidades Principais
- **Operações CRUD de Contas**
  - Criar novas contas de clientes
  - Recuperar conta por ID ou listar todas as contas
  - Atualizar informações da conta
  - Excluir contas
  - Ativar/Desativar contas

### Funcionalidades Técnicas
- **Sistema de Cache Unificado**
  - Cache único (`contasCache`) para todas as operações
  - Flag de completude (`contasCacheAtualizadas`) com TTL de 2:45h
  - Cache principal com TTL de 3h (15min de buffer)
  - Atualização inteligente: adiciona/atualiza/remove itens da lista
  - Preservação de expiração em atualizações

- **Sistema de Notificações**
  - Notificações automatizadas para Prevenção à Fraude
  - Notificações automatizadas para Departamento de Cartões
  - Notificações automatizadas para Departamento de Crédito

- **Validação e Normalização de CPF**
  - Validação completa de CPF com dígitos verificadores
  - Normalização automática de formatos (remove formatação)
  - Prevenção de contas duplicadas
  - Validação de entrada e tratamento de erros

## 🛠️ Stack Tecnológica

- **.NET 8** - Framework .NET mais recente
- **Entity Framework Core** - ORM para acesso a dados
- **SQLite** - Banco de dados leve
- **xUnit** - Framework de testes unitários
- **Moq** - Framework de mocking
- **Swagger/OpenAPI** - Documentação da API
- **GitHub Actions** - CI/CD Pipeline

## 📋 Pré-requisitos

- .NET 8 SDK
- Visual Studio 2022 ou VS Code
- Git

## 🚀 Como Começar

### 1. Clonar o Repositório
```bash
git clone https://github.com/jinoh0/KrtBank.git
cd KrtBank
```

### 2. Restaurar Dependências
```bash
dotnet restore
```

### 3. Compilar a Solução
```bash
dotnet build
```

### 4. Executar Testes
```bash
dotnet test
```

### 5. Executar a Aplicação
```bash
dotnet run --project KrtBank.Api
```

### 6. Acessar a API
- **Swagger UI**: http://localhost:5226/swagger
- **URL Base da API**: http://localhost:5226/api

## 🧪 Testes

### Executar Testes Unitários
```bash
dotnet test
```

### Cobertura de Testes
- **8 testes** para ContaService
- **Testes de utilitários** (CpfValidator, CpfNormalizer)
- **Cobertura abrangente** de cenários de sucesso e erro

### Testar Endpoints da API
Use os scripts de teste fornecidos:
```bash
# PowerShell
.\test-simple.ps1
.\test-fixes.ps1

# Ou use a interface HTML de teste
open test-api.html
```

## 📊 Endpoints da API

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/Contas` | Obter todas as contas |
| GET | `/api/Contas/{id}` | Obter conta por ID |
| POST | `/api/Contas` | Criar nova conta |
| PUT | `/api/Contas/{id}` | Atualizar conta |
| DELETE | `/api/Contas/{id}` | Excluir conta |
| POST | `/api/Contas/{id}/ativar` | Ativar conta |
| POST | `/api/Contas/{id}/inativar` | Desativar conta |

## 🏗️ Padrões de Design

### Princípios SOLID
- **Responsabilidade Única**: Cada classe tem uma razão para mudar
- **Aberto/Fechado**: Aberto para extensão, fechado para modificação
- **Substituição de Liskov**: Classes derivadas são substituíveis pelas classes base
- **Segregação de Interface**: Clientes dependem apenas das interfaces que usam
- **Inversão de Dependência**: Depender de abstrações, não de concretizações

### Domain-Driven Design
- **Entidades**: Conta com identidade
- **Objetos de Valor**: Cpf com validação
- **Eventos de Domínio**: ContaCriadaEvent, ContaAtualizadaEvent, ContaRemovidaEvent
- **Repositórios**: Abstração de acesso a dados
- **Utilitários de Domínio**: CpfValidator para validação

### Clean Architecture
- **Regra de Dependência**: Dependências apontam para dentro
- **Separação de Responsabilidades**: Cada camada tem responsabilidades específicas
- **Testabilidade**: Fácil de testar unitariamente com injeção de dependência

## 📈 Funcionalidades de Performance

- **Cache Unificado**: Sistema inteligente de cache com flag de completude
- **Async/Await**: Operações não bloqueantes
- **Entity Framework**: Consultas otimizadas
- **Gerenciamento de Memória**: Ciclo de vida eficiente de objetos
- **Preservação de TTL**: Atualizações não resetam expiração do cache

## 🔒 Considerações de Segurança

- **Validação de Entrada**: Todas as entradas são validadas
- **Prevenção de SQL Injection**: Consultas parametrizadas do Entity Framework
- **Tratamento de Erros**: Mensagens de erro seguras
- **Sanitização de CPF**: Normalização e validação automática

## 🚀 CI/CD Pipeline

### GitHub Actions
- **Build**: Compilação automática em .NET 8
- **Testes**: Execução de todos os 61 testes
- **Cobertura**: Relatório de cobertura de código
- **Segurança**: Scan de vulnerabilidades em dependências
- **Triggers**: Push para main/develop e Pull Requests

## 📝 Diretrizes de Desenvolvimento

### Padrões de Código
- **Recursos C# 12** e sintaxe moderna
- **Async/await** para todas as operações de I/O
- **Tipos de referência nullable** habilitados
- **Logging profissional** (sem emojis em produção)
- **Tratamento abrangente de erros**
- **Código limpo** sem comentários desnecessários

### Estratégia de Testes
- **Testes de Integração**: Teste de endpoints da API
- **Mocking**: Dependências externas mockadas
- **Dados de Teste**: Cenários de teste realistas

## 🤝 Contribuindo

1. Faça um fork do repositório
2. Crie uma branch de feature (`git checkout -b feature/funcionalidade-incrivel`)
3. Commit suas mudanças (`git commit -m 'Adiciona funcionalidade incrível'`)
4. Push para a branch (`git push origin feature/funcionalidade-incrivel`)
5. Abra um Pull Request

## 👨‍💻 Autor

**Seu Nome**
- GitHub: [@jinoh0](https://github.com/jinoh0)
- LinkedIn: https://linkedin.com/in/jinohong
