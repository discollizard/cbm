## Ideias para melhorias
- Possibilitar o envio de v�rios objetos dentro do JSON para otimizar a utiliza��o de recursos da API
![Atualmente o endpoint s� d� suporte a 1 objeto por vez, o que, dentro de um loop, gera um excesso desnecess�rio de chamadas HTTP](./print_api1.png)
![Atualmente o endpoint s� d� suporte a 1 objeto por vez, o que, dentro de um loop, gera um excesso desnecess�rio de chamadas HTTP](./print_api2.png)

## Notas sobre o desenvolvimento
- Inicialmente eu coloquei uma chave prim�ria gen�rica (Id) em cada tabela. Esse fluxo
� v�lido, por�m eu optei por usar os campos do documento que eu interpretei como �nicos pois
us�-los como chave prim�ria facilita o relacionamento entre as tabelas, por permitir o
trabalho com informa��es que j� existem explicitamente, assim otimizando as queries e os algoritmos
