Proyecto Fase 1 – Analizador Léxico

María Inés Leiva Casiano - 1089524
Jennifer Fernanda Turcios Estrada - 1088724

1.	DESCRPCIÓN
Este proyecto implementa un Analizador Léxico el cual solicita la ruta de un archivo .mlng, analiza su contenido carácter por carácter, clasifica los tokens encontrados y genera un archivo de salida .out con el listado de tokens. Además, muestra en pantalla los errores léxicos encontrados o indica que no hay errores. 

2.	ESTRUCTURA DEL PROYECTO
El proyecto está compuesto por las siguientes clases:
•	Program
•	Lexer
•	Token
•	TipoToken
•	ReglaLexer
•	Interfaz 

3.	GRAMÁTICA DEFINIDA (BNF)
s -> lineas
lineas -> linea lineas
        | ε
linea -> sentencia NEWLINE
       | NEWLINE
sentencia -> declaracion
           | asignacion
           | expresion
           | if
           | while
           | entradasalida
           | funcion
           | return
tipo -> PRINT
      | PRFLOAT
      | PRCHAR
      | PRBOOL
dato -> INT
      | FLOAT
      | CHAR
      | BOOL
declaracion -> tipo ID declaracionp
declaracionp -> IGUAL expresion
              | ε
asignacion -> ID IGUAL expresion
expresion -> term expresionp
expresionp -> SUM term expresionp
            | RESTA term expresionp
            | ε
term -> valor termp
termp -> MULTI valor termp
       | DIV valor termp
       | ε
valor -> ID
       | dato
       | PARENI expresion PAREND
condicion -> expresion simbolo expresion
simbolo -> MAYORQ
         | MENORQ
         | MAYORIGUAL
         | MENORIGUAL
         | IGUALIGUAL
         | NOIGUAL
bloque -> INDENT lineas DEDENT
if -> PRIF PARENI condicion PAREND NEWLINE bloque elsep
elsep -> PRELSE NEWLINE bloque
       | ε


while -> PRWHILE PARENI condicion PAREND NEWLINE bloque
entradasalida -> PRREAD PARENI ID PAREND
               | PRWRITE PARENI argwrite PAREND
argwrite -> expresion
          | dato
funcion -> PRDEF ID PARENI parametros PAREND NEWLINE bloque
parametros -> params
            | ε
params -> param paramsp
paramsp -> COMA param paramsp
         | ε
param -> tipo ID
return -> PRRETURN expression

4.	EXPRESIONES REGULARES UTILIZADAS
PRINT ::=  "int"
PRFLOAT::= "float"
PRCHAR ::=  "char"
PRBOOL ::= "bool"
PRIF ::= "if"
PRELSE ::= "else"
PRWHILE ::= "while"
PRREAD ::= "Read"
PRWRITE ::= "Write"
PRDEF ::= "def"
PRRETURN ::= "return"
INT ::= [0-9]+
FLOAT ::= [0-9]+\.[0-9]+
CHAR ::= \"[a-zA-Z0-9 ]*\"
BOOL ::= "true" | "false"
ID ::= [a-zA-Z][a-zA-Z0-9]{0,30}
SUM ::=  \+
RESTA ::= "-"
MULTI ::= \*
DIV ::= "/"
PARENI ::= "("
PAREND ::= ")"
IGUAL ::=  "="
IGUALIGUAL ::= "=="
NOIGUAL ::= "!="
MAYORQ ::= ">"
MENORQ ::= "<"
MAYORIGUAL ::= ">="
MENORIGUAL ::= "<="
PORCENTAJE ::= "%"
COMA ::= ","
WS ::= [ \t]+
COMENT ::= #[^\n\r]*
NEWLINE ::= \r?\n

5. FUNCIONAMIENTO DEL PROGRAMA
El programa solicita la ruta del archivo en la consola. Al ingresar la ruta hay que borrar las comillas que aparecerán por defecto. Luego de esto, se valida que el archivo exista y que tenga extensión .mlng. Finalmente, después de analizar el contenido, se genera un archivo .out con los tokens y en pantalla se imprimen los errores. 
Funcionalidad de cada clase:
Program.cs
•	Solicita la ruta
•	Realiza validaciones
•	Invoca el Lexer
•	Genera el archivo .out
•	Muestra errores léxicos o mensaje de éxito

Lexer.cs
•	Recorre el archivo carácter por carácter
•	Aplica las reglas regex en orden
•	Genera tokens
•	Detecta errores léxicos
•	Maneja indentación con una pila 
•	Emite tokens INDENT y DEDENT
Manejo de indentación:
•	Un TAB equivale a 4 espacios
•	Cuando aumenta la indentación se genera INDENT
•	Cuando disminuye se generan DEDENT
•	Si el nivel no coincide con uno anterior es un error

Token.cs
Representa un token con:
•	Tipo
•	Lexema
•	Línea
•	Columna inicial
•	Columna final

TipoToken.cs
Contiene constantes con los nombres de todos los tokens.

ReglaLexer.cs
Define:
•	Tipo de token
•	Patrón regex
•	Si el token debe ignorarse

6. MANEJO DE ERRORES LÉXICOS
El sistema detecta:
• Caracteres inválidos
• Strings sin cerrar
• Identificadores demasiado largos (pasan de 31 caracteres)
• Errores de indentación
Sin embargo, el analizador sigue tokenizando cuando hay un error, no se detiene. 

7. SALIDAS DEL PROGRAMA
Posee dos salidas:
•	Salida con archivo .out
•	Salida de impresión en consola, en esta salida se muestran mensajes si existen errores o si no los hay.
