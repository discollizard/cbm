## Ideias para melhorias
- Possibilitar o envio de v�rios objetos dentro do JSON para otimizar a utiliza��o de recursos da API
![Atualmente o endpoint s� d� suporte a 1 objeto por vez, o que, dentro de um loop, gera um excesso desnecess�rio de chamadas HTTP](./print_api1.png)
![Atualmente o endpoint s� d� suporte a 1 objeto por vez, o que, dentro de um loop, gera um excesso desnecess�rio de chamadas HTTP](./print_api2.png)
- Documentar sobre o caso onde se coloca um tipo de contrato desconhecido
![Atualmente n�o existe na documenta��o um poss�vel cen�rio onde a resposta seja essa](./print_api3.png)

## Bugs no processo de desenvolvimento (e como eu circumv�-los)
- Por algum motivo a chamada retorna um erro quando o tipo de contrato � "CONDOMINIO". Esse erro � diferente
do erro que eu mencionei acima nas ideias para melhorias, que ocorre quando se usa qualquer valor; esse erro
� aplicado apenas para o caso do CONDOMINIO. Para usar um ambiente de teste livre dos vieses do meu pr�prio, 
fui na fun��o de cliente HTTP da pr�pria documenta��o para provar.
![Chamada com tipo de contrato "MENSALIDADE ESCOLAR"](./print_api4.jpg)
![Chamada id�ntica mas com tipo de contrato "CONDOMINIO"](./print_api5.jpg)
Eu interpretei que a melhor abordagem seria ignorar os registros de condom�nio at� que eu pudesse
falar com alguem da equipe da Cobmais sobre.

## Notas sobre o desenvolvimento
- A estrutura de tabelas foi definida em 3 tabelas: Clientes, Contratos e LogContratos. Isso se deu pois
eu considerei esta uma forma mais s�, concisa e at�mica de separar os dados em seus diferentes dom�nios,
evitando assim a duplicidade de informa��es em cada tabela.
- **Sobre Performance**: O processamento foi feito da forma mais direta poss�vel, considerando algumas 
otimiza��es algor�tmicas (como indexar alguns valores que n�o est�o dispon�veis em 2 tabelas diferentes utlizando
valores que sim s�o acessiveis em ambas), mas apenas levando em conta o escopo limitado de 18 contratos
que o arquivo CSV oferece. Para casos de escopos maiores, onde o processamento � intenso e o volume de
dados � grande, existem maneiras de otimizar ainda mais o procedimento. Os exemplos mais �bvios que eu posso 
citar s�o:

	1. Dividir o arquivo em buffers de tamanhos iguais, onde cada um recebe um n�mero igual de linhas para
processar. O processamento ocorreria de forma paralela atrav�s de threads, assim diminuindo o tempo de resposta
da aplica��o.
	2. Implementar a melhoria sugerida na API na qual seria permitido enviar, no lugar de um objeto JSON por vez, um array
com v�rios contratos de uma vez, tal como retornado v�rios valores atualizados de uma vez. Neste caso, a performance
melhoraria pois fazer chamadas HTTP possui um overhead inerente devido aos dados serem passados por rede.
Portanto, fazer 1 chamada com v�rios dados � invariavalmente mais r�pido que fazer v�rias chamadas com 1 objeto s�.
	3. Manipular a mem�ria diretamente atrav�s da feature `unsafe` do C#; por�m, isso sacrificaria 
parte da seguran�a e legibilidade do c�digo.
