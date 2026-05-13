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
    : SENTENCIA_SIMPLE NEWLINE
    | IF
    | WHILE
    | FUNCION
    | NEWLINE
    | error NEWLINE { yyerror("sentencia no valida o incompleta"); }
    ;

SENTENCIA_SIMPLE
    : DECLARACION
    | ASIGNACION
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
    | TIPO error { yyerror("despues del tipo se esperaba un identificador"); }
    | TIPO ID IGUAL error { yyerror("despues de '=' se esperaba una expresion valida"); }
    ;

ASIGNACION
    : ID IGUAL EXPRESION
    | ID IGUAL error { yyerror("despues de '=' se esperaba una expresion valida"); }
    ;

IF
    : PRIF PARENI CONDICION PAREND NEWLINE BLOQUE ELIFS ELSEP
    | PRIF CONDICION PAREND NEWLINE BLOQUE ELIFS ELSEP { yyerror("en if falta el parentesis de apertura '('"); }
    | PRIF PARENI CONDICION NEWLINE BLOQUE ELIFS ELSEP { yyerror("en if falta el parentesis de cierre ')'"); }
    | PRIF PARENI error PAREND NEWLINE BLOQUE ELIFS ELSEP { yyerror("la condicion del if no es valida"); }
    | PRIF PARENI CONDICION PAREND NEWLINE error { yyerror("despues del if se esperaba un bloque indentado"); }
    ;

ELIFS
    : ELIFS ELIF
    |
    ;

ELIF
    : PRELIF PARENI CONDICION PAREND NEWLINE BLOQUE
    | PRELIF CONDICION PAREND NEWLINE BLOQUE { yyerror("en elif falta el parentesis de apertura '('"); }
    | PRELIF PARENI CONDICION NEWLINE BLOQUE { yyerror("en elif falta el parentesis de cierre ')'"); }
    | PRELIF PARENI error PAREND NEWLINE BLOQUE { yyerror("la condicion del elif no es valida"); }
    ;

ELSEP
    : PRELSE NEWLINE BLOQUE
    |
    ;

WHILE
    : PRWHILE PARENI CONDICION PAREND NEWLINE BLOQUE
    | PRWHILE CONDICION PAREND NEWLINE BLOQUE { yyerror("en while falta el parentesis de apertura '('"); }
    | PRWHILE PARENI CONDICION NEWLINE BLOQUE { yyerror("en while falta el parentesis de cierre ')'"); }
    | PRWHILE PARENI error PAREND NEWLINE BLOQUE { yyerror("la condicion del while no es valida"); }
    | PRWHILE PARENI CONDICION PAREND NEWLINE error { yyerror("despues del while se esperaba un bloque indentado"); }
    ;

ENTRADASALIDA
    : PRREAD PARENI ID PAREND
    | PRWRITE PARENI EXPRESION PAREND
    | PRREAD PARENI error PAREND { yyerror("Read solo puede recibir un identificador"); }
    | PRREAD ID PAREND { yyerror("en Read falta el parentesis de apertura '('"); }
    | PRREAD PARENI ID error { yyerror("en Read falta el parentesis de cierre ')'"); }
    | PRWRITE PARENI error PAREND { yyerror("Write necesita una expresion valida"); }
    | PRWRITE EXPRESION PAREND { yyerror("en Write falta el parentesis de apertura '('"); }
    | PRWRITE PARENI EXPRESION error { yyerror("en Write falta el parentesis de cierre ')'"); }
    ;

FUNCION
    : PRDEF ID PARENI PARAMETROS PAREND NEWLINE BLOQUEFUNCION
    | PRDEF PARENI PARAMETROS PAREND NEWLINE BLOQUEFUNCION { yyerror("despues de def se esperaba el nombre de la funcion"); }
    | PRDEF ID PARAMETROS PAREND NEWLINE BLOQUEFUNCION { yyerror("en la funcion falta el parentesis de apertura '('"); }
    | PRDEF ID PARENI PARAMETROS NEWLINE BLOQUEFUNCION { yyerror("en la funcion falta el parentesis de cierre ')'"); }
    | PRDEF ID PARENI PARAMETROS PAREND NEWLINE error { yyerror("despues de la funcion se esperaba un bloque indentado"); }
    ;

PARAMETROS
    : PARAMS
    |
    ;

PARAMS
    : PARAM
    | PARAMS COMA PARAM
    | PARAMS COMA error { yyerror("despues de la coma se esperaba otro parametro"); }
    ;

PARAM
    : TIPO ID
    | TIPO error { yyerror("en el parametro falta el identificador"); }
    ;

RETURN
    : PRRETURN EXPRESION
    | PRRETURN error { yyerror("return necesita una expresion valida"); }
    ;

BLOQUE
    : INDENT LINEASBLOQUE DEDENT
    | INDENT DEDENT { yyerror("el bloque no puede estar vacio"); }
    ;

LINEASBLOQUE
    : LINEASBLOQUE LINEABLOQUE
    | LINEABLOQUE
    ;

LINEABLOQUE
    : SENTENCIA_SIMPLE NEWLINE
    | IF
    | WHILE
    | RETURN NEWLINE
    | NEWLINE
    | error NEWLINE { yyerror("sentencia no valida dentro del bloque"); }
    ;

BLOQUEFUNCION
    : INDENT LINEASFUNCION DEDENT
    | INDENT DEDENT { yyerror("el bloque de la funcion no puede estar vacio"); }
    ;

LINEASFUNCION
    : LINEASFUNCION LINEAFUNCION
    | LINEAFUNCION
    ;

LINEAFUNCION
    : SENTENCIA_SIMPLE NEWLINE
    | IF
    | WHILE
    | RETURN NEWLINE
    | NEWLINE
    | error NEWLINE { yyerror("sentencia no valida dentro de la funcion"); }
    ;

LLAMADA
    : ID PARENI ARGUMENTOS PAREND
    | ID PARENI ARGUMENTOS error { yyerror("en la llamada falta el parentesis de cierre ')'"); }
    ;

ARGUMENTOS
    : LISTAARGUMENTOS
    |
    ;

LISTAARGUMENTOS
    : EXPRESION
    | LISTAARGUMENTOS COMA EXPRESION
    | LISTAARGUMENTOS COMA error { yyerror("despues de la coma se esperaba otro argumento"); }
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
    | PARENI EXPRESION error { yyerror("en la expresion falta el parentesis de cierre ')'"); }
    | PARENI error PAREND { yyerror("expresion invalida dentro de parentesis"); }
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
        mensaje = "error sintactico";
    }

    control.AgregarError(mensaje);
}