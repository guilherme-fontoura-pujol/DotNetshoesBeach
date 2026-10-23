# Banco de dados (PostgreSQL) e conexão com a API

## 1. Visão geral

```
clientes (Id, Nome, Email*, Telefone, SenhaHash, Papel, CriadoEm)        * único
quadras  (Id, Nome*, Descricao, ValorHora, Status, MotivoManutencao, CriadoEm)  * único
reservas (Id, ClienteId→clientes, QuadraId→quadras,
          DataHoraInicio, DataHoraFim, Valor, Status, CriadoEm)
```

- **Dois acessos** com uma só tabela de usuários: `clientes.Papel` = `Admin` ou `Cliente`.
- `quadras.Status` = `Disponivel` ou `EmManutencao`.
- `reservas.Status` = `Confirmada` ou `Cancelada` (cancelar não apaga: mantém histórico).
- `quadras.ValorHora` deve ser maior que zero. `reservas.Valor` = duração × `ValorHora` no momento da reserva
  (fica gravado: mudar o preço da quadra depois não altera reservas antigas).
- Datas/horas ficam sempre em **UTC** (`timestamp with time zone`).
- O banco **impede reservas sobrepostas** na mesma quadra (constraint de exclusão
  `ex_reservas_sem_sobreposicao`). Uma reserva 18h–19h e outra 19h–20h não conflitam.
- Esquema completo de referência: [`schema.sql`](schema.sql).

## 2. Instalar o PostgreSQL (escolha um)

**Docker (mais simples)** — na raiz do repositório:
```bash
docker compose up -d
```
Cria o banco `dotnetshoes_beach_db` (usuário `postgres`, senha `postgres`, porta 5432).

**Instalador no Windows** — baixe em <https://www.postgresql.org/download/windows/>, defina a senha do
usuário `postgres` e depois crie o banco (pgAdmin ou psql):
```sql
CREATE DATABASE dotnetshoes_beach_db;
```

> Usando Docker o banco já nasce pronto. Com o instalador, crie o banco vazio: as **tabelas** quem cria é a API.

## 3. Configurar a API

Em `backend/src/DotNetshoesBeach.Api` existem:

| Arquivo | Para quê |
|---|---|
| `appsettings.json` | Configuração padrão (connection string local, sem segredos). |
| `appsettings.Development.json` | **Ignorado pelo git.** Sua chave JWT e o admin inicial. Já criei um para você; altere os valores. |
| `appsettings.Development.example.json` | Modelo versionado do arquivo acima. |

Pontos importantes:

- **Connection string** (`ConnectionStrings:DefaultConnection`):
  `Host=localhost;Port=5432;Database=dotnetshoes_beach_db;Username=postgres;Password=postgres`.
  Se a sua senha for outra, sobrescreva sem editar o arquivo versionado:
  ```bash
  cd backend/src/DotNetshoesBeach.Api
  dotnet user-secrets init
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=dotnetshoes_beach_db;Username=postgres;Password=SUA_SENHA"
  ```
- **`Jwt:Key`**: chave que assina os tokens de login, **mínimo 32 caracteres**. A API não sobe sem ela.
- **`AdminInicial`**: e-mail/senha do primeiro admin. É criado automaticamente na primeira
  execução **somente se ainda não existir nenhum admin**. Troque a senha padrão e, em produção,
  informe via variáveis de ambiente (`AdminInicial__Email`, `AdminInicial__Senha`, `Jwt__Key`).

## 4. Criar as tabelas (migrations do EF Core) — uma vez

O projeto ainda não tem migrations, então gere a inicial (a pasta `Persistence/Migrations` deve ser commitada):

```bash
dotnet tool install --global dotnet-ef        # só na primeira vez
cd backend
dotnet ef migrations add InitialCreate \
  --project src/DotNetshoesBeach.Infrastructure \
  --startup-project src/DotNetshoesBeach.Api \
  --output-dir Persistence/Migrations
```

Sempre que mudar uma entidade/configuração, gere outra migration (`dotnet ef migrations add NomeDaMudanca ...`).

## 5. Rodar

```bash
cd backend/src/DotNetshoesBeach.Api
dotnet run --launch-profile http        # http://localhost:5237
```

Ao iniciar, a API (`DbInitializer`) faz automaticamente:
1. aplica as migrations pendentes (`Database.Migrate`);
2. cria a extensão `btree_gist` e a constraint anti-sobreposição de reservas (idempotente);
3. cria o admin inicial, se não existir nenhum.

Para desligar isso (ex.: produção onde você aplica migrations à parte), use `Database:InicializarAoIniciar = false`.
Nesse caso, rode o SQL da constraint (final do `schema.sql`) uma vez no banco.

Para testar: abra `DotNetshoesBeach.Api.http` (Visual Studio / VS Code com REST Client) e execute as requisições em ordem.

> Se aparecer erro `permission denied to create extension "btree_gist"`, rode uma vez como superusuário:
> `CREATE EXTENSION btree_gist;` no banco `dotnetshoes_beach_db`.

## 6. Como funcionam os dois acessos

1. O usuário faz `POST /api/auth/login` e recebe um **token JWT**.
2. O front envia o token em toda requisição: `Authorization: Bearer <token>`.
3. O token carrega o papel (`Admin` ou `Cliente`) e a API valida a permissão em cada rota.

| Rota | Cliente | Admin |
|---|:-:|:-:|
| `POST /api/auth/login`, `POST /api/auth/registrar` (público) | ✔ | ✔ |
| `GET /api/auth/me` | ✔ | ✔ |
| `GET /api/quadras`, `GET /api/quadras/{id}`, `GET /api/quadras/{id}/ocupados?de=&ate=` | ✔ | ✔ |
| `POST /api/reservas` (reservar) | ✔ (para si) | ✔ (informando `clienteId`, para qualquer cliente) |
| `GET /api/reservas/minhas` | ✔ | ✔ |
| `POST /api/reservas/{id}/cancelar` | ✔ (só as suas, antes de começar) | ✔ (qualquer uma) |
| `GET /api/reservas?quadraId=&clienteId=&de=&ate=&status=&pagina=1&tamanhoPagina=20` (paginado) | ✘ | ✔ |
| `GET /api/relatorios/quadras?de=&ate=` (reservas, valor total e médio por quadra) | ✘ | ✔ |
| `GET/POST/PUT/DELETE /api/clientes` (criar, atualizar, excluir contas) | ✘ | ✔ |
| `POST/PUT/DELETE /api/quadras` (registrar, editar, excluir quadra) | ✘ | ✔ |
| `POST /api/quadras/{id}/manutencao`, `POST /api/quadras/{id}/disponibilizar` | ✘ | ✔ |

> Mudança em relação à versão anterior: o antigo `POST /api/clientes` (cadastro público) agora é o
> `POST /api/auth/registrar`. `POST /api/clientes` passou a ser a criação de conta feita por admin.

## 7. Regras de negócio implementadas

- Cadastro: e-mail único; senha com no mínimo 8 caracteres (armazenada com BCrypt).
- Reserva: começa no futuro; duração de 1 a 4 horas; quadra precisa estar `Disponivel`; sem sobreposição.
- Quadra em manutenção não aceita reservas. Se já houver reservas ativas/futuras, a API responde
  `409` até o admin reenviar com `"cancelarReservas": true`, que as cancela.
- Excluir quadra/cliente é bloqueado enquanto houver reservas ativas ou futuras; o histórico antigo
  e as reservas canceladas são apagados junto.
- Um admin não pode excluir a própria conta nem mudar o próprio papel (evita ficar sem admin).
- Erros voltam como `{ "mensagem": "..." }` com 400, 401, 403, 404 ou 409.

## 8. Conceitos das aulas aplicados no código

| Conceito (aula) | Onde está |
|---|---|
| Mapeamento com **Fluent API** em classes de configuração, mantendo as entidades limpas (aula 09) | `Infrastructure/Persistence/Configurations/*` |
| `AppDbContext` com `DbSet<T>`, `ApplyConfigurationsFromAssembly`, **Migrations** (aula 09) | `ApplicationDbContext`, seção 4 deste guia |
| **Regra de negócio no domínio**: valor por hora > 0 (aula 10) | `Quadra.Atualizar`, `Reserva` |
| **Repository + interface + DI por construtor**, `AddScoped` / `AddSingleton` (aula 10) | `Application/Interfaces/*`, `Infrastructure/DependencyInjection.cs` |
| `Where` antes do `ToListAsync` (filtra no banco), **deferred execution** (aula 11) | `ReservaRepository.ListarPaginadoAsync` |
| **Projeção** `Select` → DTO, sem `Include` (aula 11) | `ReservaRepository.ListarPaginadoAsync`, `ListarDoClienteAsync` |
| `OrderBy`/`ThenBy` e **paginação** `Skip`/`Take` com ordenação única (aula 11) | `ListarPaginadoAsync` (desempata por `Id`) |
| `AnyAsync` e `CountAsync` (aula 11) | `ExisteConflitoAsync`, `ContarAtivasDaQuadraAsync`, total da paginação |
| `Include` quando é preciso a entidade completa e rastreada (aula 11) | `ReservaRepository.ObterPorIdAsync` (usado para cancelar) |
| `GroupBy` + `Count`/`Sum`/`Average`, intervalo `[de, ate)` (aula 11) | `ReservaRepository.ResumirPorQuadraAsync`, `GET /api/relatorios/quadras` |
| `AsNoTracking()` em listagens (aula 11) | todos os repositórios nas consultas somente-leitura |
| **Índices** simples e compostos com `HasIndex` (aula 11) | `ReservaConfiguration` |

Ainda não aplicados (são ferramentas de diagnóstico da aula 11): `ToQueryString()` e `Stopwatch`.
