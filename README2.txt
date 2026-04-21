Proyecto Fase 1 – Analizador Léxico

María Inés Leiva Casiano - 1089524
Jennifer Fernanda Turcios Estrada - 1088724

1.	DESCRPCIÓN
Este proyecto implementa un Analizador Sintáctitco que trabaja sobre el analizador léxico que fue realizado en la fase 1. El proyecto solicita una ruta de archivo .mlng, sigue realizando un análisis léxico para generar tokens y porteriormente realiza un análisis sintáctico con ayuda de Gardens Point Parser Generator. El programa verifica que la estructura del contenido del archivo sea válido según la gramática definida. 
Con esto, indica con un OK si es correcto, muestra los errores sintáctico y detecta los errores léxicos.

2. ESTRUCTURA DEL PROYECTO
-	Program 
-	Lexer 
-	Token 
-	TipoToken 
-	ReglaLexer 
-	Interfaz 
-	ControlSintactico 
-	LectorTokens 
-	Parser 

3. Gramática definida (BNF)
S -> LINEAS
LINEAS -> LINEA LINEAS
 | ε
LINEA -> SENTENCIA newline
 | newline
SENTENCIA -> DECLARACION
 | ASIGNACION
 | EXPRESION
 | IF
 | WHILE
 | ENTRADASALIDA
 | FUNCION
TIPO -> print
 | prfloat
 | prchar
 | prbool
DATO -> int
 | float
 | char
 | bool
DECLARACION -> TIPO id DECLARACIONP
DECLARACIONP -> igual EXPRESION
 | ε
ASIGNACION -> id igual EXPRESION
EXPRESION -> TERM EXPRESIONP
EXPRESIONP -> sum TERM EXPRESIONP
 | resta TERM EXPRESIONP
 | ε
TERM -> VALOR TERMP
TERMP -> multi VALOR TERMP
 | div VALOR TERMP
 | porcentaje VALOR TERMP
 | ε
VALOR -> id
 | DATO
 | LLAMADA
 | pareni EXPRESION parend
LLAMADA -> id pareni ARGUMENTOS parend
ARGUMENTOS -> LISTAARGUMENTOS
 | ε
LISTAARGUMENTOS -> EXPRESION LISTAARGUMENTOSP
LISTAARGUMENTOSP -> coma EXPRESION LISTAARGUMENTOSP
 | ε
CONDICION -> EXPROR
EXPROR -> EXPRAND EXPRORP
EXPRORP -> or EXPRAND EXPRORP
 | ε
EXPRAND -> EXPRNOT EXPRANDP
EXPRANDP -> and EXPRNOT EXPRANDP
 | ε
EXPRNOT -> not EXPRNOT
 | COMPARACION
COMPARACION -> EXPRESION SIMBOLO EXPRESION
 | pareni CONDICION parend
 | EXPRESION
SIMBOLO -> mayorq
 | menorq
 | mayorigual
 | menorigual
 | igualigual
 | noigual
BLOQUE -> indent LINEASBLOQUE dedent
LINEASBLOQUE -> LINEABLOQUE LINEASBLOQUE
 | ε
LINEABLOQUE -> SENTENCIABLOQUE newline
 | newline
SENTENCIABLOQUE -> DECLARACION
 | ASIGNACION
 | EXPRESION
 | IF
 | WHILE
 | ENTRADASALIDA
 | RETURN
IF -> prif pareni CONDICION parend newline BLOQUE ELIFSP
ELIFSP -> prelif pareni CONDICION parend newline BLOQUE ELIFSP
        | ELSEP
ELSEP -> prelse newline BLOQUE
       | εWHILE -> prwhile pareni CONDICION parend newline BLOQUE
ENTRADASALIDA -> prread pareni id parend
 | prwrite pareni EXPRESION parend
FUNCION -> prdef id pareni PARAMETROS parend newline BLOQUEFUNCION
BLOQUEFUNCION -> indent LINEASFUNCION dedent
LINEASFUNCION -> LINEAFUNCION LINEASFUNCION
 | ε
LINEAFUNCION -> SENTENCIAFUNCION newline
 | newline
SENTENCIAFUNCION -> DECLARACION
 | ASIGNACION
 | EXPRESION
 | IF
 | WHILE
 | ENTRADASALIDA
 | RETURN
PARAMETROS -> PARAMS
 | ε
PARAMS -> PARAM PARAMSP
PARAMSP -> coma PARAM PARAMSP
 | ε
PARAM -> TIPO id
RETURN -> prreturn EXPRESION

4. Gramática minilang
%namespace Proyecto1Analizador
%partial
%tokentype Tokens
%start S

%token PRINT PRFLOAT PRCHAR PRBOOL
%token PRIF PRELIF PRELSE PRWHILE PRDEF PRRETURN
%token PRREAD PRWRITE
%token INT FLOAT CHAR BOOL ID
%token OR AND NOT
%token SUM RESTA MULTI DIV PORCENTAJE
%token IGUAL IGUALIGUAL NOIGUAL MAYORQ MENORQ MAYORIGUAL MENORIGUAL
%token PARENI PAREND COMA
%token NEWLINE INDENT DEDENT
%token ERROR

%left OR
%left AND
%right NOT
%nonassoc IGUALIGUAL NOIGUAL MAYORQ MENORQ MAYORIGUAL MENORIGUAL
%left SUM RESTA
%left MULTI DIV PORCENTAJE

%%

S
    : LINEAS
    ;

LINEAS
    : LINEAS LINEA
    |
    ;

LINEA
    : SENTENCIA NEWLINE
    | SENTENCIA
    | NEWLINE
    | error NEWLINE
    ;

SENTENCIA
    : DECLARACION
    | ASIGNACION
    | EXPRESION
    | IF
    | WHILE
    | ENTRADASALIDA
    | FUNCION
    ;

TIPO
    : PRINT
    | PRFLOAT
    | PRCHAR
    | PRBOOL
    ;

DATO
    : INT
    | FLOAT
    | CHAR
    | BOOL
    ;

DECLARACION
    : TIPO ID
    | TIPO ID IGUAL EXPRESION
    ;

ASIGNACION
    : ID IGUAL EXPRESION
    ;

IF
    : PRIF PARENI CONDICION PAREND NEWLINE BLOQUE ELIFS ELSEP
    ;

ELIFS
    : ELIFS ELIF
    |
    ;

ELIF
    : PRELIF PARENI CONDICION PAREND NEWLINE BLOQUE
    ;

ELSEP
    : PRELSE NEWLINE BLOQUE
    |
    ;

WHILE
    : PRWHILE PARENI CONDICION PAREND NEWLINE BLOQUE
    ;

ENTRADASALIDA
    : PRREAD PARENI ID PAREND
    | PRWRITE PARENI EXPRESION PAREND
    ;

FUNCION
    : PRDEF ID PARENI PARAMETROS PAREND NEWLINE BLOQUEFUNCION
    ;

PARAMETROS
    : PARAMS
    |
    ;

PARAMS
    : PARAM
    | PARAMS COMA PARAM
    ;

PARAM
    : TIPO ID
    ;

RETURN
    : PRRETURN EXPRESION
    ;

BLOQUE
    : INDENT LINEASBLOQUE DEDENT
    ;

LINEASBLOQUE
    : LINEASBLOQUE LINEABLOQUE
    |
    ;

LINEABLOQUE
    : SENTENCIABLOQUE NEWLINE
    | SENTENCIABLOQUE
    | NEWLINE
    | error NEWLINE
    ;

SENTENCIABLOQUE
    : DECLARACION
    | ASIGNACION
    | EXPRESION
    | IF
    | WHILE
    | ENTRADASALIDA
    | RETURN
    ;

BLOQUEFUNCION
    : INDENT LINEASFUNCION DEDENT
    ;

LINEASFUNCION
    : LINEASFUNCION LINEAFUNCION
    |
    ;

LINEAFUNCION
    : SENTENCIAFUNCION NEWLINE
    |SENTENCIAFUNCION
    | NEWLINE
    | error NEWLINE
    ;

SENTENCIAFUNCION
    : DECLARACION
    | ASIGNACION
    | EXPRESION
    | IF
    | WHILE
    | ENTRADASALIDA
    | RETURN
    ;

LLAMADA
    : ID PARENI ARGUMENTOS PAREND
    ;

ARGUMENTOS
    : LISTAARGUMENTOS
    |
    ;

LISTAARGUMENTOS
    : EXPRESION
    | LISTAARGUMENTOS COMA EXPRESION
    ;

CONDICION
    : EXPRESION
    ;

EXPRESION
    : ID
    | DATO
    | LLAMADA
    | PARENI EXPRESION PAREND
    | NOT EXPRESION
    | EXPRESION OR EXPRESION
    | EXPRESION AND EXPRESION
    | EXPRESION IGUALIGUAL EXPRESION
    | EXPRESION NOIGUAL EXPRESION
    | EXPRESION MAYORQ EXPRESION
    | EXPRESION MENORQ EXPRESION
    | EXPRESION MAYORIGUAL EXPRESION
    | EXPRESION MENORIGUAL EXPRESION
    | EXPRESION SUM EXPRESION
    | EXPRESION RESTA EXPRESION
    | EXPRESION MULTI EXPRESION
    | EXPRESION DIV EXPRESION
    | EXPRESION PORCENTAJE EXPRESION
    ;

5. Cambios realizados en la gramática (BNF)
- Se utilizó recursión por la derecha para procesar varias líneas en orden sin ambigüedad
- Se separó LINEA y SENTENCIA para que cada instrucción termine de manera adecuada, además permite líneas vacías
- Se agruparon las instrucciones en un único no terminal para simplificar la gramática
- El manejo de precedencia en las expresiones se separó por niveles para que se respete la precedencia de operadores
- Se incluyeron llamadas dentro de las expresiones 
- La condiciones lógicas se separan por niveles lógicos para respetar la precedencia lógica
- Se utiliza simbolo para los operadores para que sea más fácil agregar nuevos operadores
- Los bloques se manejan con INDENT y DEDENT para agrupar las isntrucciones manera adecuada y evitar la ambigüedad en las estructuras
- Se definieron SENTENCIA, SENTENCIABLOQUE y SENTENCIAFUNCION para evitar al ambigüedad en el parser y controlar las instrucciones que van en cada contexto
- Soporte de elig y else para que se puedan implementar distintas condiciones
- La entrada y salida se unificaron para que sea más fácil de mantener
- Se agregaron PARAMETROS para que se permitan funciones simples y complejas

6. Manejo de errores
- Estructuras incompletas
- Tokens inesperados
- Errores en los bloques
El analizador no se detiene con los errores

7. Uso de GPPG
Se utilizó GPPG para generar de manera automática el parser LR 

8. Salidas del programa
- Genera el archivo .out con los tokens
- Se imprime en consola el OK o los errores que encuentre

