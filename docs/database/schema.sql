-- DotNetshoes Beach — esquema de referência do PostgreSQL

CREATE EXTENSION IF NOT EXISTS btree_gist;

CREATE TABLE clientes (
    "Id"        uuid                     NOT NULL,
    "Nome"      varchar(150)             NOT NULL,
    "Email"     varchar(150)             NOT NULL,
    "Telefone"  varchar(20),
    "SenhaHash" text                     NOT NULL,
    "Papel"     varchar(20)              NOT NULL,   -- 'Cliente' | 'Admin'
    "CriadoEm"  timestamp with time zone NOT NULL,
    CONSTRAINT "PK_clientes" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "IX_clientes_Email" ON clientes ("Email");

CREATE TABLE quadras (
    "Id"               uuid                     NOT NULL,
    "Nome"             varchar(100)             NOT NULL,
    "Descricao"        varchar(500),
    "ValorHora"        numeric(10,2)            NOT NULL,   -- regra: maior que zero (validada no domínio)
    "Status"           varchar(20)              NOT NULL,   -- 'Disponivel' | 'EmManutencao'
    "MotivoManutencao" varchar(500),
    "CriadoEm"         timestamp with time zone NOT NULL,
    CONSTRAINT "PK_quadras" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "IX_quadras_Nome" ON quadras ("Nome");

CREATE TABLE reservas (
    "Id"             uuid                     NOT NULL,
    "ClienteId"      uuid                     NOT NULL,
    "QuadraId"       uuid                     NOT NULL,
    "DataHoraInicio" timestamp with time zone NOT NULL,   -- sempre UTC
    "DataHoraFim"    timestamp with time zone NOT NULL,   -- sempre UTC
    "Valor"          numeric(10,2)            NOT NULL,   -- duração × ValorHora da quadra na criação
    "Status"         varchar(20)              NOT NULL,   -- 'Confirmada' | 'Cancelada'
    "CriadoEm"       timestamp with time zone NOT NULL,
    CONSTRAINT "PK_reservas" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_reservas_clientes_ClienteId" FOREIGN KEY ("ClienteId") REFERENCES clientes ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_reservas_quadras_QuadraId"   FOREIGN KEY ("QuadraId")  REFERENCES quadras ("Id")  ON DELETE CASCADE
);
CREATE INDEX "IX_reservas_ClienteId_DataHoraInicio" ON reservas ("ClienteId", "DataHoraInicio");
CREATE INDEX "IX_reservas_QuadraId_DataHoraInicio"  ON reservas ("QuadraId", "DataHoraInicio");

-- Regra de ouro do sistema: a mesma quadra nunca pode ter duas reservas CONFIRMADAS
-- com horários sobrepostos. Garantido pelo banco, mesmo com dois pedidos simultâneos.
-- Intervalo [início, fim): uma reserva 18h–19h e outra 19h–20h NÃO conflitam.
ALTER TABLE reservas
    ADD CONSTRAINT ex_reservas_sem_sobreposicao
    EXCLUDE USING gist (
        "QuadraId" WITH =,
        tstzrange("DataHoraInicio", "DataHoraFim", '[)') WITH &&
    )
    WHERE ("Status" = 'Confirmada');
