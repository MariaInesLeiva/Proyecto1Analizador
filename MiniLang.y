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
    | NEWLINE
    | error NEWLINE { yyerror("sentencia no válida o incompleta"); }
    ;

SENTENCIA
    : DECLARACION
    | ASIGNACION
    | IF
    | WHILE
    | ENTRADASALIDA
    | FUNCION
    | LLAMADA
    ;

SENTENCIA_SIMPLE
    : DECLARACION
    | ASIGNACION
    | IF
    | WHILE
    | ENTRADASALIDA
    | LLAMADA
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
    | TIPO error { yyerror("después del tipo se esperaba un identificador"); }
    | TIPO ID IGUAL error { yyerror("después de '=' se esperaba una expresión válida"); }
    ;

ASIGNACION
    : ID IGUAL EXPRESION
    | ID IGUAL error { yyerror("después de '=' se esperaba una expresión válida"); }
    ;

IF
    : PRIF PARENI CONDICION PAREND NEWLINE BLOQUE ELIFS ELSEP
    | PRIF CONDICION PAREND NEWLINE BLOQUE ELIFS ELSEP { yyerror("en if falta el paréntesis de apertura '('"); }
    | PRIF PARENI CONDICION NEWLINE BLOQUE ELIFS ELSEP { yyerror("en if falta el paréntesis de cierre ')'"); }
    | PRIF PARENI error PAREND NEWLINE BLOQUE ELIFS ELSEP { yyerror("la condición del if no es válida"); }
    | PRIF PARENI CONDICION PAREND NEWLINE error { yyerror("después del if se esperaba un bloque indentado"); }
    ;

ELIFS
    : ELIFS ELIF
    |
    ;

ELIF
    : PRELIF PARENI CONDICION PAREND NEWLINE BLOQUE
    | PRELIF CONDICION PAREND NEWLINE BLOQUE { yyerror("en elif falta el paréntesis de apertura '('"); }
    | PRELIF PARENI CONDICION NEWLINE BLOQUE { yyerror("en elif falta el paréntesis de cierre ')'"); }
    | PRELIF PARENI error PAREND NEWLINE BLOQUE { yyerror("la condición del elif no es válida"); }
    ;

ELSEP
    : PRELSE NEWLINE BLOQUE
    |
    ;

WHILE
    : PRWHILE PARENI CONDICION PAREND NEWLINE BLOQUE
    | PRWHILE CONDICION PAREND NEWLINE BLOQUE { yyerror("en while falta el paréntesis de apertura '('"); }
    | PRWHILE PARENI CONDICION NEWLINE BLOQUE { yyerror("en while falta el paréntesis de cierre ')'"); }
    | PRWHILE PARENI error PAREND NEWLINE BLOQUE { yyerror("la condición del while no es válida"); }
    | PRWHILE PARENI CONDICION PAREND NEWLINE error { yyerror("después del while se esperaba un bloque indentado"); }
    ;

ENTRADASALIDA
    : PRREAD PARENI ID PAREND
    | PRWRITE PARENI EXPRESION PAREND
    | PRREAD PARENI error PAREND { yyerror("Read solo puede recibir un identificador"); }
    | PRREAD ID PAREND { yyerror("en Read falta el paréntesis de apertura '('"); }
    | PRREAD PARENI ID error { yyerror("en Read falta el paréntesis de cierre ')'"); }
    | PRWRITE PARENI error PAREND { yyerror("Write necesita una expresión válida"); }
    | PRWRITE EXPRESION PAREND { yyerror("en Write falta el paréntesis de apertura '('"); }
    | PRWRITE PARENI EXPRESION error { yyerror("en Write falta el paréntesis de cierre ')'"); }
    ;

FUNCION
    : PRDEF ID PARENI PARAMETROS PAREND NEWLINE BLOQUEFUNCION
    | PRDEF PARENI PARAMETROS PAREND NEWLINE BLOQUEFUNCION { yyerror("después de def se esperaba el nombre de la función"); }
    | PRDEF ID PARAMETROS PAREND NEWLINE BLOQUEFUNCION { yyerror("en la función falta el paréntesis de apertura '('"); }
    | PRDEF ID PARENI PARAMETROS NEWLINE BLOQUEFUNCION { yyerror("en la función falta el paréntesis de cierre ')'"); }
    | PRDEF ID PARENI PARAMETROS PAREND NEWLINE error { yyerror("después de la función se esperaba un bloque indentado"); }
    ;

PARAMETROS
    : PARAMS
    |
    ;

PARAMS
    : PARAM
    | PARAMS COMA PARAM
    | PARAMS COMA error { yyerror("después de la coma se esperaba otro parámetro tipo id"); }
    ;

PARAM
    : TIPO ID
    | TIPO error { yyerror("en el parámetro falta el identificador"); }
    ;

RETURN
    : PRRETURN EXPRESION
    | PRRETURN error { yyerror("return necesita una expresión válida"); }
    ;

BLOQUE
    : INDENT LINEASBLOQUE DEDENT
    | INDENT DEDENT { yyerror("el bloque no puede estar vacío"); }
    ;

LINEASBLOQUE
    : LINEASBLOQUE LINEABLOQUE
    | LINEABLOQUE
    ;

LINEABLOQUE
    : SENTENCIABLOQUE NEWLINE
    | NEWLINE
    | error NEWLINE { yyerror("sentencia no válida dentro del bloque"); }
    ;

SENTENCIABLOQUE
    : SENTENCIA_SIMPLE
    | RETURN
    ;

BLOQUEFUNCION
    : INDENT LINEASFUNCION DEDENT
    | INDENT DEDENT { yyerror("el bloque de la función no puede estar vacío"); }
    ;

LINEASFUNCION
    : LINEASFUNCION LINEAFUNCION
    | LINEAFUNCION
    ;

LINEAFUNCION
    : SENTENCIAFUNCION NEWLINE
    | NEWLINE
    | error NEWLINE { yyerror("sentencia no válida dentro de la función"); }
    ;

SENTENCIAFUNCION
    : SENTENCIA_SIMPLE
    | RETURN
    ;

LLAMADA
    : ID PARENI ARGUMENTOS PAREND
    | ID PARENI ARGUMENTOS error { yyerror("en la llamada falta el paréntesis de cierre ')'"); }
    ;

ARGUMENTOS
    : LISTAARGUMENTOS
    |
    ;

LISTAARGUMENTOS
    : EXPRESION
    | LISTAARGUMENTOS COMA EXPRESION
    | LISTAARGUMENTOS COMA error { yyerror("después de la coma se esperaba otro argumento"); }
    ;

CONDICION
    : EXPRESION
    ;

EXPRESION
    : EXP_OR
    ;

EXP_OR
    : EXP_OR OR EXP_AND
    | EXP_AND
    ;

EXP_AND
    : EXP_AND AND EXP_IGUALDAD
    | EXP_IGUALDAD
    ;

EXP_IGUALDAD
    : EXP_RELACIONAL
    | EXP_RELACIONAL IGUALIGUAL EXP_RELACIONAL
    | EXP_RELACIONAL NOIGUAL EXP_RELACIONAL
    ;

EXP_RELACIONAL
    : EXP_ADITIVA
    | EXP_ADITIVA MAYORQ EXP_ADITIVA
    | EXP_ADITIVA MENORQ EXP_ADITIVA
    | EXP_ADITIVA MAYORIGUAL EXP_ADITIVA
    | EXP_ADITIVA MENORIGUAL EXP_ADITIVA
    ;

EXP_ADITIVA
    : EXP_ADITIVA SUM EXP_MULTIPLICATIVA
    | EXP_ADITIVA RESTA EXP_MULTIPLICATIVA
    | EXP_MULTIPLICATIVA
    ;

EXP_MULTIPLICATIVA
    : EXP_MULTIPLICATIVA MULTI EXP_UNARIA
    | EXP_MULTIPLICATIVA DIV EXP_UNARIA
    | EXP_MULTIPLICATIVA PORCENTAJE EXP_UNARIA
    | EXP_UNARIA
    ;

EXP_UNARIA
    : NOT EXP_UNARIA
    | RESTA EXP_UNARIA
    | PRIMARIA
    ;

PRIMARIA
    : ID
    | DATO
    | LLAMADA
    | PARENI EXPRESION PAREND
    | PARENI EXPRESION error { yyerror("en la expresión falta el paréntesis de cierre ')'"); }
    | PARENI error PAREND { yyerror("expresión inválida dentro de paréntesis"); }
    ;

%%

private LectorTokens lector;
private ControlSintactico control;

public Parser(LectorTokens lectorEntrada, ControlSintactico controlEntrada) : base(lectorEntrada)
{
    lector = lectorEntrada;
    control = controlEntrada;
}

public int yylex()
{
    return lector.yylex();
}

public void yyerror(string mensaje)
{
    if (string.IsNullOrWhiteSpace(mensaje))
    {
        mensaje = "error sintáctico";
    }

    control.AgregarError(mensaje);
}