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
    | NEWLINE
    | error NEWLINE { yyerror("Error en línea"); }
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
    | NEWLINE
    | error NEWLINE { yyerror("Error en bloque"); }
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
    | NEWLINE
    | error NEWLINE { yyerror("Error en bloque"); }
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

%%

public partial class Parser
{
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
        if (control.tokenActual != null)
        {
            control.AgregarError(
                $"Línea {control.tokenActual.Linea}, columna {control.tokenActual.ColumnaInicio}, cerca de '{control.tokenActual.Lexema}'"
            );
        }
        else
        {
            control.AgregarError("Error sintáctico");
        }
    }
}