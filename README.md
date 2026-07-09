# 📡 NetVigia — Monitoramento de Uptime de Sites

API back-end para **monitoramento de disponibilidade (uptime) de sites**, desenvolvida em **C# com ASP.NET Core**. O sistema verifica periodicamente o status de URLs cadastradas através de **workers em background** e armazena o histórico de disponibilidade em um banco de dados **time-series**, permitindo análises de uptime ao longo do tempo.

---

## ✨ Funcionalidades

- **Cadastro de sites/URLs** para monitoramento
- **Verificação periódica automática** de disponibilidade via workers em background
- **Histórico de uptime** armazenado em banco de dados de séries temporais (time-series)
- **Autenticação de usuários** para gerenciamento seguro dos monitoramentos
- API RESTful para consulta de status e histórico de cada site monitorado

---

## 🏗️ Arquitetura em Camadas

```
NetVigia/
├── NetVigia.API               → Endpoints REST (Controllers, middlewares)
├── NetVigia.BLL                → Regras de negócio
├── NetVigia.Data                → Acesso a dados relacionais (cadastro de sites, usuários)
├── NetVigia.Data.TimeSeries     → Acesso ao banco de séries temporais (histórico de uptime)
├── NetVigia.DTO                 → Objetos de transferência de dados
├── NetVigia.Identity             → Autenticação e gerenciamento de usuários
└── NetVigia.Workers              → Serviços em background para checagem periódica de uptime
```

O componente `NetVigia.Workers` é responsável por executar as checagens de disponibilidade em intervalos programados, registrando os resultados (status, tempo de resposta, disponibilidade) na camada `NetVigia.Data.TimeSeries`, otimizada para consultas históricas e séries temporais.

---

## 🛠️ Tecnologias Utilizadas

| Tecnologia               | Uso                                                      |
|---------------------------|-----------------------------------------------------------|
| C# / ASP.NET Core         | Framework principal da API                                |
| Background Workers        | Verificação periódica e assíncrona de disponibilidade     |
| Banco de dados Time-Series| Armazenamento histórico de métricas de uptime              |
| PostgreSQL                | Persistência relacional (sites, usuários, configurações)  |
| ASP.NET Identity / JWT    | Autenticação e autorização                                 |

---

## 📋 Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download)
- Instância de banco de dados time-series configurada (IoTDB)
- [Visual Studio](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

---

## 🚀 Como Executar

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/Dravvos/NetVigia.git
   cd NetVigia
   ```

2. **Configure as strings de conexão:**
   - Crie a variável de ambiente `IoTDBCon` da conexão do banco time-series
   - Crie a variável de ambiente `NetVigiaCon` do PostgreSQL

3. **Aplique as migrations:**
   ```bash
   dotnet ef database update --project NetVigia.Data --startup-project NetVigia.API
   ```

4. **Execute a API:**
   ```bash
   cd NetVigia.API
   dotnet run
   ```

5. **Execute o worker de monitoramento:**
   ```bash
   cd NetVigia.Workers
   dotnet run
   ```

6. **Acesse a documentação (Swagger):**
   - Navegue até `https://localhost:{porta}/swagger`

---

## 📌 Endpoints Principais

| Método | Rota                                                  | Descrição                                         |
|--------|-------------------------------------------------------|---------------------------------------------------|
| POST   | `/api/auth/register`                                  | Cadastro de novo usuário                          |
| POST   | `/api/auth/login`                                     | Login e geração de token JWT                      |
| GET    | `/api/server`                                         | Lista os sites monitorados pelo usuário           |
| POST   | `/api/server`                                         | Cadastra um novo site para monitoramento          |
| PUT    | `/api/server/{id}`                                    | Atualiza as configurações de um site monitorado   |
| DELETE | `/api/server/{id}`                                    | Remove um site do monitoramento                   |
| GET    | `/api/check/{serverId}/{startDate}/{endDate}`         | Consulta o histórico de uptime (série temporal)   |
| GET    | `/api/check/uptime/{serverId}/{startDate}/{endDate}`  | Consulta a porcentagem de uptime                  |

> Todas as rotas de gerenciamento requerem autenticação via `Bearer Token`.

---

## 📁 Estrutura do Projeto

```
NetVigia/
├── NetVigia.API/                 # Camada de apresentação (Controllers, Startup)
├── NetVigia.BLL/                 # Regras de negócio
├── NetVigia.Data/                # Repositórios e contexto do EF Core (dados relacionais)
├── NetVigia.Data.TimeSeries/     # Acesso ao banco de séries temporais (histórico de uptime)
├── NetVigia.DTO/                 # Objetos de transferência de dados
├── NetVigia.Identity/            # Autenticação e gerenciamento de identidade
├── NetVigia.Workers/             # Serviços em background para checagem de disponibilidade
├── NetVigia.sln                  # Solution file
├── LICENSE.txt                   # Licença Apache 2.0
└── README.md
```

---

## 🔐 Autenticação

A API utiliza **JWT (JSON Web Tokens)**. Após o login, inclua o token no header das requisições protegidas:

```
Authorization: Bearer {seu_token_aqui}
```

---

## 🤝 Contribuições

Contribuições são bem-vindas! Para contribuir:

1. Faça um **fork** do repositório
2. Crie uma branch: `git checkout -b feature/minha-feature`
3. Commit: `git commit -m 'feat: descrição da melhoria'`
4. Push: `git push origin feature/minha-feature`
5. Abra um **Pull Request**

---

## 📄 Licença

Este projeto está licenciado sob a [Apache License 2.0](LICENSE.txt).

---

Desenvolvido por [Daniel Oliveira Dias (Dravvos)](https://github.com/Dravvos)
