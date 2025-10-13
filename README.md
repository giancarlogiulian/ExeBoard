# ExeBoard
### Programa desenvolvido para automatizar o download, instalação, inicialização de aplicações com arquitetura cliente-servidor e atualização de bancos de dados

#### Passo a Passo (Para Usuários e Desenvolvedores)
1. Instalar o NET Framework ( https://dotnet.microsoft.com/pt-br/download )
2. Instalar o Google Drive ( https://ipv4.google.com/intl/pt-BR/drive/download/ )
3. Conectar a conta do Google Drive
4. Criar o arquivo Inicializar.ini na pasta do CopiarExes.exe

#### Passo a Passo (Para Desenvolvedores)
5. Instalar o Visual Studio Community no computador ( https://visualstudio.microsoft.com/pt-br/vs/community/ )
6. Instalar o SDK do NET 8.0.414 x64  ( https://dotnet.microsoft.com/pt-br/download )
7. Fazer um fork/clone do projeto e importar na IDE

#### Inicializar.ini

```
[CONFIG_GERAIS]
RODAR_NA_BANDEJA=Sim        -- Quando configurado igual a Sim deixa o ícone do programa na bandeja ao fechar/minimizar o sistema

[CAMINHOS]
DE=G:\Meu Drive             -- Diretório de origem dos arquivos
DE_PASTA_CLIENT=Clientes    -- Pasta de origem dos executáveis de aplicações clientes
DE_PASTA_SERVER=Servidores  -- Pasta de origem dos executáveis de aplicações servidoras
DE_PASTA_DADOS=SQL          -- Pasta de origem dos scripts de atualização
PARA=C:\CopiarExes          -- Diretório de destino dos arquivos
PASTA_CLIENT=Client         -- Pasta de destino dos executáveis de aplicações clientes
PASTA_SERVER=Server         -- Pasta de destino dos executáveis de aplicações servidoras
PASTA_DADOS=Dados           -- Pasta de destino dos scripts de atualização

[APLICACOES_CLIENTE]
Count=3                     -- Quantidade de aplicações cliente
Cliente0=Cliente1.exe       -- Nome da aplicação cliente
SubDiretorios0=             -- Utilizar caso houver outros diretórios depois do Client para apontar ao EXE.
Categoria0=                 -- A categoria do programa. Ex: Financeiro, Gerencial. Será utilizado na bandeija ao lado do relógio.
Cliente1=Cliente2.exe
SubDiretorios1=
Categoria1=
Cliente2=Cliente3.exe
SubDiretorios3=
Categoria3=

[APLICACOES_SERVIDORAS]
Count=2                     -- Quantidade de aplicações servidoras
Servidor0=Servidor1.exe     -- Nome da aplicação servidora
SubDiretorios0=             -- Utilizar caso houver outros diretórios depois da Server para apontar ao EXE
Tipo1=Servico               -- Servico (caso seja executado como um serviço do Windows) ou Aplicacao
Servidor1=Servidor2.exe
SubDiretorios1=
Tipo1=Aplicação

[SERVIDORES_AVULSOS]
Count=1                      -- Quantidade de servidores avulsos (Obs: Serão utilizados somente na tela de Servidores)
Servidor0=ServidorA.exe      -- Nome do servidor
Caminho0=                    -- Caminho completo do servidor avulso
Tipo0=                       -- Servico (caso seja executado como um serviço do Windows) ou Aplicacao
Categoria0=                  -- A categoria do programa. Ex: Financeiro, Gerencial. Será utilizado na bandeija ao lado do relógio.

[BANCO_DE_DADOS]
Count=2                      -- Quantidade de bancos de dados utilizados
BancoDados0=Firebird         -- Caminho dos arquivos atualizadores de banco de dados
BancoDados1=Oracle

[ATUALIZADORES]
Count=2                      -- Quantidade de executáveis atualizadores
NomeAtualizador0=            -- Nome do executável atualizador
CaminhoAtualizador0=         -- Caminho do executável atualizador
NomeAtualizador1=
CaminhoAtualizador1=
```
