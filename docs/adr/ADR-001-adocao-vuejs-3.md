ADR 01: Adoção do Vue.js 3 para a Interface da Aplicação

Status: Aceito
Data: 2026-09-29
Decisores: Grupo do Projeto (Guimarães / Gularte / Pujol)

1. Contexto

A aplicação requer uma interface Web desacoplada para consumir uma Web API desenvolvida em .NET 10.

O projeto possui prazo definido para desenvolvimento e a equipe precisa utilizar um framework Web que permita a construção de uma interface organizada, componentizada e de fácil manutenção, mantendo a separação de responsabilidades entre Front-End e Back-End.

O Front-End será desenvolvido utilizando Visual Studio Code, com a extensão oficial do Vue.

2. Opções Consideradas
Opção 1 (Angular): Oferece uma estrutura robusta e padronizada para aplicações Web, porém apresenta uma curva de aprendizado maior e possui uma estrutura mais abrangente para o escopo atual do projeto.
Opção 2 (React): É uma alternativa amplamente utilizada e flexível, porém exige que a equipe tome decisões adicionais sobre bibliotecas e ferramentas para estruturar diferentes aspectos da aplicação.
Opção 3 (Vue.js 3 - Composition API): Oferece uma abordagem baseada em componentes, possui documentação oficial abrangente e permite organizar a aplicação de forma modular, sendo adequado para o desenvolvimento da interface proposta.
3. Decisão

Escolhemos o Vue.js 3, utilizando a Composition API, para o desenvolvimento do Front-End da aplicação.

A escolha considera o escopo do projeto, o prazo disponível e a necessidade de desenvolver uma interface componentizada e desacoplada do Back-End.

O Vue.js será responsável pela camada de apresentação e pela interação com o usuário, enquanto as regras de negócio permanecerão no Back-End desenvolvido em .NET 10.

4. Consequências
Positivas:
Desenvolvimento da interface baseado em componentes reutilizáveis.
Separação clara entre Front-End e Back-End.
Estrutura adequada para desenvolvimento das telas de login, cadastro, disponibilidade, reservas e dashboard.
Possibilidade de utilizar a Composition API para organizar lógica reutilizável entre componentes.
Boa integração com APIs HTTP desenvolvidas em .NET.
Possibilidade de utilização do TypeScript caso o grupo identifique benefícios durante o desenvolvimento.
Utilização das ferramentas oficiais do ecossistema Vue.
Negativas / Riscos (Trade-offs):
A equipe precisará aprender e aplicar corretamente os conceitos do Vue.js 3 e da Composition API.
Será necessário definir uma estratégia adequada para comunicação com a API .NET.
O projeto precisará estabelecer posteriormente como serão tratados autenticação, autorização e armazenamento do token no Front-End.
A utilização de bibliotecas adicionais deverá ser avaliada para evitar aumento desnecessário da complexidade do projeto.
O grupo deverá manter as regras de negócio no Back-End para evitar acoplamento entre a lógica da aplicação e o Vue.js.
5. Impacto Arquitetural

A adoção do Vue.js 3 estabelece que o framework será utilizado na camada de apresentação da aplicação.

A comunicação com o Back-End ocorrerá por meio da Web API desenvolvida em .NET 10:

Vue.js 3
   │
   │ HTTP / JSON
   ▼
.NET 10 Web API
   │
   ▼
Clean Architecture
   │
   ▼
PostgreSQL

Dessa forma, o Vue.js não será responsável pelas regras de negócio relacionadas a clientes, quadras, horários ou reservas.

Essa separação permitirá que o Front-End e o Back-End sejam desenvolvidos e evoluídos de maneira desacoplada.

6. Critérios para Revisão

A decisão poderá ser revisada caso sejam identificados problemas relevantes durante o desenvolvimento relacionados à:

Performance;
Complexidade do código;
Manutenção dos componentes;
Integração com a Web API;
Gerenciamento de estado;
Escalabilidade;
Produtividade da equipe;
Necessidade de recursos não atendidos adequadamente pelo framework.

Qualquer mudança significativa nessa decisão deverá ser registrada por meio de um novo ADR, mantendo o histórico das decisões arquiteturais do projeto