Proyecto Fase 3 – Analizador Semántico

María Inés Leiva Casiano - 1089524
Jennifer Fernanda Turcios Estrada - 1088724

1. DESCRIPCIÓN
Este proyecto implementa un Analizador Semántico que trabaja sobre las fases anteriores del compilador. Primero se realiza el análisis léxico para generar los tokens, luego se ejecuta el análisis sintáctico con ayuda de Gardens Point Parser Generator y, si no existen errores léxicos ni sintácticos, se realiza el análisis semántico.

El analizador semántico verifica el uso correcto de variables, tipos de datos, asignaciones, operaciones y llamadas a funciones. Además, genera una tabla de símbolos donde se almacenan las variables, funciones y parámetros encontrados durante el análisis.


2. ESTRUCTURA DEL PROYECTO
El proyecto está compuesto por las siguientes clases:

- Program
- Lexer
- Token
- TipoToken
- ReglaLexer
- Interfaz
- ControlSintactico
- LectorTokens
- Parser
- AnalizadorSemantico
- Simbolo

3. GRAMÁTICA DEFINIDA (BNF)
S -> LINEAS

LINEAS -> LINEAS LINEA
        | ε

LINEA -> SENTENCIA NEWLINE
       | SENTENCIA
       | NEWLINE

SENTENCIA -> DECLARACION
           | ASIGNACION
           | EXPRESION
           | IF
           | WHILE
           | ENTRADASALIDA
           | FUNCION

TIPO -> PRINT
      | PRFLOAT
      | PRCHAR
      | PRBOOL

DATO -> INT
      | FLOAT
      | CHAR
      | BOOL

DECLARACION -> TIPO ID
             | TIPO ID IGUAL EXPRESION

ASIGNACION -> ID IGUAL EXPRESION

IF -> PRIF PARENI CONDICION PAREND NEWLINE BLOQUE ELIFS ELSEP

ELIFS -> ELIFS ELIF
       | ε

ELIF -> PRELIF PARENI CONDICION PAREND NEWLINE BLOQUE

ELSEP -> PRELSE NEWLINE BLOQUE
       | ε

WHILE -> PRWHILE PARENI CONDICION PAREND NEWLINE BLOQUE

ENTRADASALIDA -> PRREAD PARENI ID PAREND
               | PRWRITE PARENI EXPRESION PAREND

FUNCION -> PRDEF ID PARENI PARAMETROS PAREND NEWLINE BLOQUEFUNCION

PARAMETROS -> PARAMS
            | ε

PARAMS -> PARAM
        | PARAMS COMA PARAM

PARAM -> TIPO ID

RETURN -> PRRETURN EXPRESION

BLOQUE -> INDENT LINEASBLOQUE DEDENT

LINEASBLOQUE -> LINEASBLOQUE LINEABLOQUE
              | ε

LINEABLOQUE -> SENTENCIABLOQUE NEWLINE
             | SENTENCIABLOQUE
             | NEWLINE

SENTENCIABLOQUE -> DECLARACION
                 | ASIGNACION
                 | EXPRESION
                 | IF
                 | WHILE
                 | ENTRADASALIDA
                 | RETURN

BLOQUEFUNCION -> INDENT LINEASFUNCION DEDENT

LINEASFUNCION -> LINEASFUNCION LINEAFUNCION
               | ε

LINEAFUNCION -> SENTENCIAFUNCION NEWLINE
              | SENTENCIAFUNCION
              | NEWLINE

SENTENCIAFUNCION -> DECLARACION
                  | ASIGNACION
                  | EXPRESION
                  | IF
                  | WHILE
                  | ENTRADASALIDA
                  | RETURN

LLAMADA -> ID PARENI ARGUMENTOS PAREND

ARGUMENTOS -> LISTAARGUMENTOS
            | ε

LISTAARGUMENTOS -> EXPRESION
                 | LISTAARGUMENTOS COMA EXPRESION

CONDICION -> EXPRESION

EXPRESION -> ID
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

4. FUNCIONAMIENTO DEL PROGRAMA
El programa solicita la ruta de un archivo con extensión .mlng. Primero valida que el archivo exista 
y que tenga la extensión correcta. Luego lee el contenido del archivo y ejecuta el analizador léxico 
para generar la lista de tokens.

Después se ejecuta el analizador sintáctico utilizando el parser generado con GPPG. Si el programa no 
encuentra errores léxicos ni sintácticos, se ejecuta el analizador semántico.
Importante: Todos los archivos deben tener un enter al final para no provocar error sintáctico EOF.

El análisis semántico revisa que las variables estén declaradas antes de usarse, que no se declaren dos veces, que las asignaciones respeten los tipos de datos y que las funciones sean declaradas antes de llamarse.

5. FUNCIONALIDAD CLASES AGREGADAS DURANTE ESTA FASE

AnalizadorSemantico.cs
- Recorre la lista de tokens
- Detecta declaraciones de variables
- Detecta asignaciones
- Valida el uso de variables
- Valida tipos de datos en expresiones
- Procesa funciones
- Valida llamadas a funciones
- Genera la tabla de símbolos
- Muestra los errores semánticos encontrados

Simbolo.cs
Representa un elemento de la tabla de símbolos con:
- Nombre
- Tipo
- Valor
- Categoría
- Parámetros
- Línea
- Columna

6. REGLAS SEMÁNTICAS IMPLEMENTADAS

Declaración de variables:
- Una variable debe declararse antes de utilizarse.
- No se permite declarar dos veces una variable con el mismo nombre.
- Cada variable declarada se guarda en la tabla de símbolos.

Uso de variables:
- Si se utiliza un identificador que no existe en la tabla de símbolos, se genera un error semántico.
- El análisis no se detiene al encontrar este error.

Asignaciones:
- Se valida que la variable exista antes de asignarle un valor.
- Se obtiene el tipo de la expresión del lado derecho.
- Se verifica que el tipo de la expresión sea compatible con el tipo de la variable.
- Se permite asignar un int a una variable float.
- No se permite asignar float, char o bool a una variable de tipo int si no corresponde.

Operaciones:
- Se permiten operaciones entre tipos numéricos, como int y float.
- Se permiten operaciones entre valores bool cuando corresponde.
- No se permiten operaciones entre tipos incompatibles, por ejemplo int con char o bool con int.

Funciones:
- Una función no puede declararse dos veces con el mismo nombre.
- Las funciones se guardan en la tabla de símbolos con categoría "funcion".
- Los parámetros se guardan con su tipo correspondiente.
- Cuando se llama una función, se valida que exista.
- Se valida que la cantidad de argumentos coincida con la cantidad de parámetros.
- Se valida que los tipos de los argumentos sean compatibles con los tipos de los parámetros.

7. TABLA DE SÍMBOLOS
La tabla de símbolos almacena la información importante de las variables, funciones y parámetros encontrados durante el análisis.

La tabla muestra:
- Nombre
- Tipo
- Valor
- Línea
- Columna

Esto permite verificar qué identificadores fueron reconocidos por el analizador semántico y qué información se guardó de cada uno.

8. MANEJO DE ERRORES SEMÁNTICOS
El sistema detecta:

- Variables no declaradas
- Variables declaradas más de una vez
- Asignaciones con tipos incompatibles
- Operaciones entre tipos incompatibles
- Funciones no declaradas
- Funciones declaradas más de una vez
- Cantidad incorrecta de argumentos en una función
- Argumentos incompatibles con los parámetros de una función

El analizador semántico no se ejecuta si existen errores léxicos o sintácticos, ya que primero 
se necesita que el código tenga tokens válidos y una estructura correcta.

9. SALIDAS DEL PROGRAMA
El programa posee las siguientes salidas:

- Muestra en consola los tokens encontrados.
- Muestra errores léxicos si existen.
- Muestra errores sintácticos si existen.
- Muestra el resultado del análisis semántico.
- Muestra la tabla de símbolos.
- Genera un archivo .out con el listado de tokens.

10. RESULTADO FINAL
El proyecto integra las tres fases principales del análisis de un compilador:

- Análisis léxico
- Análisis sintáctico
- Análisis semántico
