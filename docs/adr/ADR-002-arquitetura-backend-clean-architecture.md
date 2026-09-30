# ADR 02: Adoção de Clean Architecture em 4 Camadas para o Back-End

## Status
Aceito

## Data
2026-09-30

## Decisores
Grupo do Projeto (Guilherme Guimarães / João Gularte / Guilherme Pujol)

## Contexto
O Back-End da aplicação DotNetshoes Beach deve expor uma Web API em .NET 10 para gerenciar regras de reservas de quadras, autenticação e controle de disponibilidade. 
O projeto exige facilidade de manutenção, baixo acoplamento entre regras de negócio e infraestrutura (PostgreSQL, ASP.NET Core), além de permitir que o código seja testável e atenda aos requisitos acadêmicos da disciplina Frameworks Web.

## Opções Consideradas

1. **Monólito em Projeto Único (.NET Web API Único):**
   - Agrupamento de pastas (`Controllers`, `Models`, `Data`) no mesmo `.csproj`.
   - *Descarte:* Alto risco de acoplamento direto (ex.: controllers chamando o `DbContext`), ferindo a separação estrita de responsabilidades requerida.

2. **Clean Architecture Hiper-Fracionada (6 ou mais projetos):**
   - Criação de bibliotecas independentes para `Domain.Services`, `Application.Contracts`, `Infrastructure.IoC`, etc.
   - *Descarte:* Complexidade acidental e sobrecarga de manutenção ("overengineering") injustificada para o tamanho do time e escopo do sistema.

3. **Clean Architecture em 4 Projetos (`Domain`, `Application`, `Infrastructure`, `Api`):**
   - Separação em quatro projetos com fluxo unidirecional de dependências.

## Decisão
Foi escolhida a **Clean Architecture em 4 Projetos**:
- **Domain:** Entidades puras e regras invariantes de negócio sem dependências externas.
- **Application:** Casos de uso, interfaces de repositórios e serviços, DTOs de entrada e saída.
- **Infrastructure:** Implementação do Entity Framework Core, acesso ao PostgreSQL e serviços externos (geração de tokens).
- **Api:** Camada de entrada HTTP (Controllers/Endpoints), injeção de dependência e configuração de middlewares (CORS, autenticação).

## Consequências Positivas
- Domínio completamente isolado de bibliotecas de terceiros e frameworks de banco de dados.
- Regras de negócio desacopladas do protocolo HTTP.
- Facilidade para mockar dependências e executar testes unitários futuros.
- Organização clara de responsabilidades alinhada com os padrões corporativos modernos.

## Consequências Negativas / Riscos
- Necessidade de mapeamentos explícitos entre Entidades do Domínio e DTOs.
- Maior quantidade de arquivos e referências de projetos do que em um projeto único.