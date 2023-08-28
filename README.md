## Como rodar o projeto
A vers�o de release buildada do projeto est� na pasta `bin\Release\net6.0\publish`. Talvez seja necess�rio criar uma pasta
`Arquivos CSV` e colocar o arquivo de importa��o dentro, se j� n�o estiver criada, para o programa ter onde ler o arquivo
de input e onde colocar o arquivo de exporta��o. Os arquivos banco de dados (.mdf e .ldf) est�o na raiz do projeto.
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
Eu interpretei que a melhor abordagem seria salvar o log do registro como uma requisi��o que
deu errado, mostrar na interface qual contrato deu errado e n�o colocar o dado no arquivo de
exporta��o.
## Notas sobre o desenvolvimento
- **Sobre o banco de dados**: A estrutura de tabelas foi definida em 3 tabelas: Clientes, Contratos e LogContratos. Isso se deu pois
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

- **Sobre as p�ginas**: Como eu dividi o banco de dados em 3 tabelas, eu achei justo permitir que as 3 pudessem ser vistas
a partir da interface web.

- **Sobre orgraniza��o do c�digo**: Eu procurei simular o c�digo de uma aplica��o escal�vel nos aspectos que faz�-lo traz
benef�cios para um projeto pequeno como este. Por exemplo: Abstrair estruturas de dados customizadas em suas pr�prias classes;
buscar me ater ao padr�o MVC; manter pastas para cada parte distinta da aplica��o; abstrair a utiliza��o da API em sua pr�pria
classe (apesar de ter deixado algumas propriedades fixas l�, pois todas as chamadas feitas s�o usando POST e o mesmo Content-Type). 
