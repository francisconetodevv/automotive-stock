Use a aba "Run and Debug" e selecione sua configuração composta "Launch ALL Services".
# README de Testes — AutomotiveStock

Este README explica como executar localmente os componentes do sistema para testes e verificação dos fluxos principais (consumo, alerta, ordem de compra, reabastecimento).

Previamente: o projeto contém 3 aplicações console principais dentro de `src/`:

- `AutomotiveStock.PlantSimulator` — simula consumo e reabastecimento nas plantas;
- `AutomotiveStock.CentralStock` — consolida e processa eventos, gera alertas e ordens;
- `AutomotiveStock.Purchasing` — recebe alertas/ordens e registra saída.

## Pré-requisitos

- .NET SDK (6.0, 7.0 ou 9.0 conforme configuração do projeto)
- Git
- Docker (recomendado) ou uma instância RabbitMQ acessível

## Iniciar RabbitMQ com Docker (recomendado)

Abra um terminal PowerShell e rode:

```powershell
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

Abra o management UI em: http://localhost:15672 (usuário/senha padrão: `guest`/`guest`).

> Observação: se já tiver RabbitMQ em execução, apenas confirme as configurações de conexão em `appsettings.json` dos projetos.

## Executando todos os serviços (VS Code)

Se você usa VS Code, há uma configuração composta `Launch ALL Services` em `launch.json`. Use a aba "Run and Debug" e selecione essa configuração para iniciar os 3 serviços simultaneamente — cada projeto abre em um terminal separado.

## Executando via terminal (dotnet run)

Abra três terminais (PowerShell) na raiz da solution e execute cada serviço separadamente:

```powershell
cd src/AutomotiveStock.CentralStock
dotnet run

# Em outro terminal
cd src/AutomotiveStock.PlantSimulator
dotnet run

# Em outro terminal
cd src/AutomotiveStock.Purchasing
dotnet run
```

Os serviços irão conectar ao RabbitMQ, criar/executar filas e começar a publicar/consumir eventos.

## O que observar (fluxo de teste)

1. Consumo: o `PlantSimulator` publica eventos `ConsumptionEvent` periodicamente.
2. CentralStock consome esses eventos e atualiza o estoque consolidado.
3. Se um material atingir o ponto de reposição, `CentralStock` publica/loga um alerta `LowStockAlert`.
4. `Purchasing` consome alertas e, para materiais classe A, gera uma ordem de compra (`PurchaseOrderEvent`).
5. Em eventos de reabastecimento (`ReplenishmentEvent`), `CentralStock` atualiza estoque e notifica as plantas.

Logs relevantes aparecerão nos consoles de cada aplicação. Procure por palavras-chave:

- `ALERTA DE ESTOQUE BAIXO` ou `LowStockAlert`
- `ORDEM DE COMPRA GERADA` ou `PurchaseOrderEvent`
- `ReplenishmentEvent` / `estoque atualizado`

## Cenários de teste sugeridos

- Cenário 1 — Consumo normal:
	- Execute o `PlantSimulator` e observe consumos sequenciais; verifique que o estoque é decrementado sem alertas.

- Cenário 2 — Estoque crítico e geração de ordem:
	- Configure o estoque inicial de um material para um valor próximo do ponto de reposição.
	- Simule consumo suficiente para atravessar o ponto de reposição.
	- Verifique emissão de `LowStockAlert` e criação de `PurchaseOrderEvent` pela `Purchasing`.

- Cenário 3 — Reabastecimento:
	- Simule envio de `ReplenishmentEvent` (o `PlantSimulator` pode enviar este evento após X iterações).
	- Verifique atualização de estoque e notificação para as plantas.

Os cenários de teste detalhados também estão descritos no documento de Requisitos (seção 11). Use-os como referência.

## Comandos úteis

- Para rebuildar a solução completa:

```powershell
dotnet build AutomotiveStock.sln
```

- Para limpar bin/obj local (em caso de artefatos antigos):

```powershell
Get-ChildItem -Recurse -Include bin,obj | Remove-Item -Force -Recurse
```

- Para forçar um reprocessamento de filas após alterar código de mensageria, reinicie os serviços.

## Verificação e depuração

- Use o RabbitMQ Management UI para observar exchanges, filas e mensagens em trânsito.
- Verifique logs nos consoles de cada serviço para rastrear o ciclo de um evento (ID do evento, timestamp, planta, material, quantidade).
- Se o sistema central estiver offline, valide que o `PlantSimulator` continua publicando mensagens e que elas ficam enfileiradas.

## Problemas comuns

- Mensagens não chegam: verifique `connection string` para RabbitMQ e se a exchange/queue foram criadas.
- Mensagens duplicadas: verifique implementações de ack/nack no consumer.
- Alertas repetidos: verifique lógica de `AlertSent`/lock e o fluxo de destravamento no reabastecimento.

## Reset de estado (para testes repetidos)

- Pare os serviços, esvazie as filas via Management UI e reinicie os serviços.

## Contato

Se algo não funcionar como esperado, cole os trechos de log relevantes e descreva o cenário que você tentou reproduzir.

----

Arquivo: `src/README.MD` — atualizada para orientar testes locais e reprodução de cenários.
