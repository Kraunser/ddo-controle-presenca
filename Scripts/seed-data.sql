-- =============================================
-- Script para popular o banco DDO com dados de teste
-- Execute este script após aplicar as migrations
-- =============================================

USE [DDO_ControlePonto]
GO

-- Limpar dados existentes (cuidado em produção!)
DELETE FROM [Presencas];
DELETE FROM [ArquivosPDF];
DELETE FROM [Colaboradores];
DELETE FROM [Areas];
GO

-- Inserir Áreas
INSERT INTO [Areas] ([Nome], [Descricao], [Ativa], [DataCriacao]) VALUES
('Tecnologia da Informação', 'Área responsável por desenvolvimento de sistemas e infraestrutura de TI', 1, DATEADD(month, -6, GETUTCDATE())),
('Recursos Humanos', 'Área responsável pela gestão de pessoas e processos de RH', 1, DATEADD(month, -5, GETUTCDATE())),
('Engenharia', 'Área de desenvolvimento de produtos e soluções de engenharia', 1, DATEADD(month, -4, GETUTCDATE())),
('Vendas', 'Área comercial responsável pelas vendas e relacionamento com clientes', 1, DATEADD(month, -3, GETUTCDATE())),
('Marketing', 'Área responsável por marketing digital e comunicação', 1, DATEADD(month, -2, GETUTCDATE())),
('Financeiro', 'Área responsável pela gestão financeira e contábil', 1, DATEADD(month, -1, GETUTCDATE()));
GO

-- Inserir Colaboradores
INSERT INTO [Colaboradores] ([Nome], [Matricula], [CodigoRFID], [Email], [Telefone], [Ativo], [DataCadastro], [AreaId]) VALUES
-- TI
('Ana Silva Santos', 'RDC0001', 'RFID123456', 'ana.silva@randoncorp.com', '(51) 99876-5432', 1, DATEADD(day, -120, GETUTCDATE()), 1),
('Bruno Costa Lima', 'RDC0002', 'RFID234567', 'bruno.costa@randoncorp.com', '(51) 99765-4321', 1, DATEADD(day, -110, GETUTCDATE()), 1),
('Carla Oliveira Souza', 'RDC0003', 'RFID345678', 'carla.oliveira@randoncorp.com', '(51) 99654-3210', 1, DATEADD(day, -100, GETUTCDATE()), 1),
('Daniel Ferreira Alves', 'RDC0004', 'RFID456789', 'daniel.ferreira@randoncorp.com', '(51) 99543-2109', 1, DATEADD(day, -90, GETUTCDATE()), 1),
('Eduarda Martins Rocha', 'RDC0005', 'RFID567890', 'eduarda.martins@randoncorp.com', '(51) 99432-1098', 1, DATEADD(day, -80, GETUTCDATE()), 1),

-- RH
('Felipe Rodrigues Nunes', 'RDC0006', 'RFID678901', 'felipe.rodrigues@randoncorp.com', '(51) 99321-0987', 1, DATEADD(day, -70, GETUTCDATE()), 2),
('Gabriela Pereira Castro', 'RDC0007', 'RFID789012', 'gabriela.pereira@randoncorp.com', '(51) 99210-9876', 1, DATEADD(day, -60, GETUTCDATE()), 2),
('Henrique Santos Dias', 'RDC0008', 'RFID890123', 'henrique.santos@randoncorp.com', '(51) 99109-8765', 1, DATEADD(day, -50, GETUTCDATE()), 2),
('Isabela Lima Cardoso', 'RDC0009', 'RFID901234', 'isabela.lima@randoncorp.com', '(51) 99098-7654', 1, DATEADD(day, -40, GETUTCDATE()), 2),

-- Engenharia
('João Pedro Almeida', 'RDC0010', 'RFID012345', 'joao.pedro@randoncorp.com', '(51) 99987-6543', 1, DATEADD(day, -35, GETUTCDATE()), 3),
('Karina Fernandes Silva', 'RDC0011', 'RFID123450', 'karina.fernandes@randoncorp.com', '(51) 99876-5430', 1, DATEADD(day, -30, GETUTCDATE()), 3),
('Lucas Barbosa Mendes', 'RDC0012', 'RFID234501', 'lucas.barbosa@randoncorp.com', '(51) 99765-4320', 1, DATEADD(day, -25, GETUTCDATE()), 3),
('Mariana Gomes Ribeiro', 'RDC0013', 'RFID345012', 'mariana.gomes@randoncorp.com', '(51) 99654-3201', 1, DATEADD(day, -20, GETUTCDATE()), 3),
('Nicolas Araújo Costa', 'RDC0014', 'RFID450123', 'nicolas.araujo@randoncorp.com', '(51) 99543-2100', 1, DATEADD(day, -15, GETUTCDATE()), 3),

-- Vendas
('Patrícia Moreira Santos', 'RDC0015', 'RFID501234', 'patricia.moreira@randoncorp.com', '(51) 99432-1090', 1, DATEADD(day, -10, GETUTCDATE()), 4),
('Rafael Carvalho Lopes', 'RDC0016', 'RFID612345', 'rafael.carvalho@randoncorp.com', '(51) 99321-0980', 1, DATEADD(day, -8, GETUTCDATE()), 4),
('Sophia Nascimento Cruz', 'RDC0017', 'RFID723456', 'sophia.nascimento@randoncorp.com', '(51) 99210-9870', 1, DATEADD(day, -6, GETUTCDATE()), 4),
('Thiago Ramos Oliveira', 'RDC0018', 'RFID834567', 'thiago.ramos@randoncorp.com', '(51) 99109-8760', 1, DATEADD(day, -4, GETUTCDATE()), 4),

-- Marketing
('Vitória Azevedo Pinto', 'RDC0019', 'RFID945678', 'vitoria.azevedo@randoncorp.com', '(51) 99098-7650', 1, DATEADD(day, -3, GETUTCDATE()), 5),
('William Teixeira Sousa', 'RDC0020', 'RFID056789', 'william.teixeira@randoncorp.com', '(51) 99987-6540', 1, DATEADD(day, -2, GETUTCDATE()), 5),
('Amanda Correia Freitas', 'RDC0021', 'RFID167890', 'amanda.correia@randoncorp.com', '(51) 99876-5403', 1, DATEADD(day, -1, GETUTCDATE()), 5),

-- Financeiro
('Carlos Eduardo Melo', 'RDC0022', 'RFID278901', 'carlos.eduardo@randoncorp.com', '(51) 99765-4302', 1, GETUTCDATE(), 6),
('Débora Campos Vieira', 'RDC0023', 'RFID389012', 'debora.campos@randoncorp.com', '(51) 99654-3201', 1, GETUTCDATE(), 6),
('Evandro Silva Monteiro', 'RDC0024', 'RFID490123', 'evandro.silva@randoncorp.com', '(51) 99543-2101', 1, GETUTCDATE(), 6);
GO

-- Inserir Arquivos PDF de exemplo
INSERT INTO [ArquivosPDF] ([NomeArquivo], [CaminhoArquivo], [TamanhoBytes], [TipoConteudo], [DataUpload], [UploadedBy], [AreaId], [Descricao], [Ativo]) VALUES
('DDO_Janeiro_2024.pdf', '/uploads/pdfs/DDO_Janeiro_2024.pdf', 1234567, 'application/pdf', DATEADD(day, -30, GETUTCDATE()), 'admin@randoncorp.com', 1, 'Arquivo DDO de Janeiro 2024', 1),
('DDO_Fevereiro_2024.pdf', '/uploads/pdfs/DDO_Fevereiro_2024.pdf', 1345678, 'application/pdf', DATEADD(day, -25, GETUTCDATE()), 'admin@randoncorp.com', 2, 'Arquivo DDO de Fevereiro 2024', 1),
('DDO_Marco_2024.pdf', '/uploads/pdfs/DDO_Marco_2024.pdf', 1456789, 'application/pdf', DATEADD(day, -20, GETUTCDATE()), 'admin@randoncorp.com', 3, 'Arquivo DDO de Março 2024', 1),
('Apresentacao_DDO_TI.pdf', '/uploads/pdfs/Apresentacao_DDO_TI.pdf', 2345678, 'application/pdf', DATEADD(day, -15, GETUTCDATE()), 'admin@randoncorp.com', 1, 'Apresentação do DDO para área de TI', 1),
('Relatorio_Presenca_RH.pdf', '/uploads/pdfs/Relatorio_Presenca_RH.pdf', 987654, 'application/pdf', DATEADD(day, -10, GETUTCDATE()), 'admin@randoncorp.com', 2, 'Relatório de presença do RH', 1),
('Manual_Sistema_DDO.pdf', '/uploads/pdfs/Manual_Sistema_DDO.pdf', 3456789, 'application/pdf', DATEADD(day, -5, GETUTCDATE()), 'admin@randoncorp.com', 1, 'Manual do sistema DDO', 1);
GO

-- Inserir registros de presença de exemplo (últimos 7 dias úteis)
DECLARE @DataInicio DATE = DATEADD(day, -7, CAST(GETDATE() AS DATE));
DECLARE @DataAtual DATE = @DataInicio;
DECLARE @ColaboradorId INT;
DECLARE @Hora TIME;

WHILE @DataAtual <= CAST(GETDATE() AS DATE)
BEGIN
    -- Pular fins de semana
    IF DATEPART(weekday, @DataAtual) NOT IN (1, 7) -- Não é domingo (1) nem sábado (7)
    BEGIN
        -- Cursor para percorrer colaboradores ativos
        DECLARE colaborador_cursor CURSOR FOR
        SELECT Id FROM Colaboradores WHERE Ativo = 1;
        
        OPEN colaborador_cursor;
        FETCH NEXT FROM colaborador_cursor INTO @ColaboradorId;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Simular que 80% dos colaboradores comparecem
            IF (ABS(CHECKSUM(NEWID())) % 10) < 8
            BEGIN
                -- Entrada (entre 7:30 e 9:00)
                SET @Hora = DATEADD(minute, ABS(CHECKSUM(NEWID())) % 90 + 450, '00:00:00'); -- 450 min = 7:30
                INSERT INTO [Presencas] ([ColaboradorId], [DataPresenca], [TipoRegistro], [MetodoRegistro], [LocalRegistro], [Observacoes])
                VALUES (@ColaboradorId, CAST(@DataAtual AS DATETIME) + CAST(@Hora AS DATETIME), 0, 0, 'Leitor-01', 'Entrada principal');
                
                -- Saída para almoço (entre 11:30 e 12:30)
                IF (ABS(CHECKSUM(NEWID())) % 10) < 7 -- 70% saem para almoço
                BEGIN
                    SET @Hora = DATEADD(minute, ABS(CHECKSUM(NEWID())) % 60 + 690, '00:00:00'); -- 690 min = 11:30
                    INSERT INTO [Presencas] ([ColaboradorId], [DataPresenca], [TipoRegistro], [MetodoRegistro], [LocalRegistro], [Observacoes])
                    VALUES (@ColaboradorId, CAST(@DataAtual AS DATETIME) + CAST(@Hora AS DATETIME), 1, 0, 'Leitor-01', 'Saída para almoço');
                    
                    -- Retorno do almoço (entre 13:00 e 14:00)
                    SET @Hora = DATEADD(minute, ABS(CHECKSUM(NEWID())) % 60 + 780, '00:00:00'); -- 780 min = 13:00
                    INSERT INTO [Presencas] ([ColaboradorId], [DataPresenca], [TipoRegistro], [MetodoRegistro], [LocalRegistro], [Observacoes])
                    VALUES (@ColaboradorId, CAST(@DataAtual AS DATETIME) + CAST(@Hora AS DATETIME), 0, 0, 'Leitor-01', 'Retorno do almoço');
                END
                
                -- Saída final (entre 17:00 e 19:00)
                IF (ABS(CHECKSUM(NEWID())) % 10) < 9 -- 90% registram saída
                BEGIN
                    SET @Hora = DATEADD(minute, ABS(CHECKSUM(NEWID())) % 120 + 1020, '00:00:00'); -- 1020 min = 17:00
                    INSERT INTO [Presencas] ([ColaboradorId], [DataPresenca], [TipoRegistro], [MetodoRegistro], [LocalRegistro], [Observacoes])
                    VALUES (@ColaboradorId, CAST(@DataAtual AS DATETIME) + CAST(@Hora AS DATETIME), 1, 0, 'Leitor-01', 'Saída final');
                END
            END
            
            FETCH NEXT FROM colaborador_cursor INTO @ColaboradorId;
        END
        
        CLOSE colaborador_cursor;
        DEALLOCATE colaborador_cursor;
    END
    
    SET @DataAtual = DATEADD(day, 1, @DataAtual);
END
GO

-- Verificar dados inseridos
SELECT 'Areas' as Tabela, COUNT(*) as Total FROM Areas
UNION ALL
SELECT 'Colaboradores', COUNT(*) FROM Colaboradores
UNION ALL
SELECT 'Presencas', COUNT(*) FROM Presencas
UNION ALL
SELECT 'ArquivosPDF', COUNT(*) FROM ArquivosPDF;
GO

PRINT 'Dados de teste inseridos com sucesso!';
GO
