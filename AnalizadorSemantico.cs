using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.IO;

namespace Proyecto1Analizador
{
    public class AnalizadorSemantico
    {
        // Guardamos la lista de tokens que recibimos del analiizador léxico
        private readonly List<Token> tokens;
        // Creamos una lista para almacenar todos los errores semánticos encontrados
        private readonly List<string> errores;
        // Creamos la tabla de símbolos donde guardamos variables, funciones y parámetros
        private readonly List<Simbolo>tablaSimbolos;

        // Recibimos los tokens generados por el lexer e inicializamos las listas del análisis semántico
        public AnalizadorSemantico(List<Token> tokensEntrada)
        {
            tokens = tokensEntrada;
            errores = new List<string>();
            tablaSimbolos = new List<Simbolo>();

        }

        public void Analizar()
        {
            // Recorremos todos los tokens para validar declaraciones, asignaciones, funciones y condiciones
            for(int i=0; i <tokens.Count; i++)
            {
                Token actual = tokens[i];
                // Si encontramos el token final, detenemos el análisis
                if(actual.Tipo==TipoToken.FP)
                break;

                // Si encontramos una función, la procesamos y luego avanzamos hasta el final de esa línea
                if(actual.Tipo == TipoToken.PRDEF)
                {
                    ProcesarFuncion(i);
                    while (i < tokens.Count && tokens[i].Tipo != TipoToken.NEWLINE)
                    {
                        i++;
                    }
                    continue;
                }

                // Si el token actual es un tipo de dato, entonces procesamos una declaración
                if (EsDeclaracion(actual))
                {
                    ProcesarDeclaracion(i);
                }

                // Si encontramos un if o while, validamos que su condición sea booleana
                if(actual.Tipo == TipoToken.PRIF || actual.Tipo == TipoToken.PRWHILE)
                {
                    ValidarCondicionBool(i);
                }

                // Si encontramos un identificador, revisamos si es llamada a función, uso de variable o asignación
                if (actual.Tipo == TipoToken.ID)
                {
                    // Si después del ID viene un paréntesis, lo tratamos como llamada a función
                    if(i+1 < tokens.Count && tokens[i+1].Tipo == TipoToken.PARENI)
                    {
                        ProcesarLlamadaFuncion(i);
                    }
                    else
                    {
                        // Validamos que la variable haya sido declarada antes de usarse
                        ValidarUsoVariable(actual);
                        // Si después del ID viene un igual, entonces procesamos una asignación
                        if(i+1< tokens.Count && tokens[i+1].Tipo == TipoToken.IGUAL)
                        {
                            ProcesarAsignacion(i);
                        }
                    }
                }
                
            }
        }
    
    private void ValidarCondicionBool(int indice)
    {
        // Iniciamos después del paréntesis izquierdo del if o while
        int inicio = indice + 2;

        // Creamos una lista para guardar los tokens que forman la condición
        List<Token> condicion = new List<Token>();

        // Recorremos la condición hasta encontrar el paréntesis derecho
        while(inicio < tokens.Count && tokens[inicio].Tipo != TipoToken.PAREND)
        {
            condicion.Add(tokens[inicio]);
            inicio++;
        }

        // Evaluamos el tipo de la condición
        string tipoCondicion = EvaluarTipoExpresion(condicion);

        // Si la condición no es bool agregamos un error semántico
        if(tipoCondicion != "bool")
        {
            errores.Add(
                $"Línea {tokens[indice].Linea}, columna {tokens[indice].ColumnaI}: " +
                $"Error la condición debe ser de tipo 'bool'"
            );
        }
    }
    private bool EsDeclaracion(Token token)
    {
        // Verificamos si el token corresponde a un tipo de dato válido para declarar variables
        return  token.Tipo==TipoToken.PRINT ||
                token.Tipo==TipoToken.PRFLOAT ||
                token.Tipo==TipoToken.PRCHAR ||
                token.Tipo==TipoToken.PRBOOL;
    }

    private void ProcesarDeclaracion(int indice)
        {
            // Validamos que exista un token después del tipo de dato
            if(indice+1 >= tokens.Count)
            return;

            // Guardamos el tipo y el identificador de la declaración
            Token tipo = tokens[indice];
            Token identificador = tokens[indice+1];

            // Si después del tipo no viene un ID, no procesamos la declaración
            if(identificador.Tipo != TipoToken.ID)
            return;

            // Revisamos si la variable ya existe en la tabla de símbolos
            bool existe = tablaSimbolos.Any(s=> s.Nombre == identificador.Lexema);

            // Si ya existe, agregamos un error porque no se puede declarar dos veces
            if(existe)
            {
                errores.Add(
                    $"Línea {identificador.Linea}, columna {identificador.ColumnaI}: " + $"Error variable '{identificador.Lexema}' ya declarada");
                    return;
            }

            // Si no existe, agregamos la variable a la tabla de símbolos
            tablaSimbolos.Add(new Simbolo
            {
                Nombre = identificador.Lexema,
                Tipo = ObtenerTipoDeclaracion(tipo.Tipo),
                Valor = null,
                Linea = identificador.Linea,
                Columna = identificador.ColumnaI
            });  
        }

        private void ValidarUsoVariable(Token token)
        {
            // Buscamos si la variable existe dentro de la tabla de símbolos
            bool existe = tablaSimbolos.Any(s=> s.Nombre == token.Lexema);
            // Si no existe, significa que se está usando sin haber sido declarada
            if (!existe)
            {
                errores.Add($"Línea {token.Linea}, columna {token.ColumnaI}: "+ $"Error variable '{token.Lexema}' no declarada");
            }
        }

        private void ProcesarAsignacion(int indice)
        {
            // Guardamos el token de la variable que está recibiendo el valor
            Token variable=tokens[indice];
            // Buscamos la variable en la tabla de símbolos
            Simbolo? simbolo  = tablaSimbolos.FirstOrDefault(s=>s.Nombre==variable.Lexema);
            
            // Si la variable no existe no seguiimos porque ya se reportó como error
            if (simbolo==null)
            return;

            // Obtenemos los tokens que forman la expresión del lado derecho de la asignación
            List<Token> expresion=ObtenerExpresion(indice+2);

            // Si no hay expresión no hay nada que validar
            if(expresion.Count==0)
            return;

            // Evaluamos el tipo de la expresión
            string tipoExpresion=EvaluarTipoExpresion(expresion);
            // Si la expresión yaa generó error, detenemos esta asignación
            if(tipoExpresion=="error")
            return;

            // Validamos si el tipo de la variable es compatible con el tipo de la expresión
            if(!TiposCompatibles(simbolo.Tipo, tipoExpresion))
            {
                errores.Add($"Línea{variable.Linea}, columna {variable.ColumnaI}: "+ $"Error no se puede asignar '{tipoExpresion}' a '{simbolo.Tipo}'");
                return;
            }
            // Si la asignación es válida, guardamos el valor simple o la expresión como valor del símbolp
            simbolo.Valor = ObtenerValorSimple(expresion);

        }

        private List<Token> ObtenerExpresion(int inicio)
        {
            // Creamos una lista para guardar los tokens de una expresión
            List<Token> expresion = new List<Token>();
            // Recorremos desde la posición indicada hasta encontrar el final de línea o bloque
            for(int i=inicio; i<tokens.Count; i++)
            {
                Token actual = tokens[i];
                // La expresión termina cuando aparce un salto de línea, indentación, dedent o fin del archivo
                if(actual.Tipo==TipoToken.NEWLINE||
                actual.Tipo ==TipoToken.INDENT ||
                actual.Tipo== TipoToken.DEDENT||
                actual.Tipo == TipoToken.FP)
                {
                    break;
                }
                // Agregamos cada token que forma parte de la expresión
                expresion.Add(actual);
            }
            return expresion;
        }

        private string EvaluarTipoExpresion(List<Token> expresion)
        {
            // Guardamos el tipo que se va construyendo conforme analizamos la expresión
            string tipoActual="";

            foreach(Token token in expresion)
            {
                // Ignoramos operadores y símbolos porque solo necesitamos comparar los tipos
                if(EsOperador(token.Tipo)||
                token.Tipo==TipoToken.PARENI ||
                token.Tipo==TipoToken.PAREND ||
                token.Tipo == TipoToken.COMA)
                {
                    continue;
                }

                // Obtenemos el tipo del token actual
                string tipoToken = ObtenerTipoToken(token);

                // Si el token no tiene un tipo reconocido, lo ignoramos
                if(tipoToken == "desconcido")
                continue;

                // Si hubo error al obtener el tipo, detenemos la evaluación
                if(tipoToken == "error")
                return "error";

                // Si todavía no hay tipo actual, usamos el tipo del primer operando
                if (tipoActual == "")
                {
                    tipoActual=tipoToken;
                }
                else
                {
                    // Si ya hay un tipo validamos si se puede operar con el tipo actual
                    if(!OperacionValida(tipoActual, tipoToken))
                    {
                        errores.Add($"Línea{token.Linea}, columan{token.ColumnaI}: " + $"Error no se puede operar el tipo '{tipoActual}' y '{tipoToken}'");
                        return "error";
                    }
                    // Calculamos el tipo resultante de la operación
                    tipoActual=ResultadoOperacion(tipoActual, tipoToken);
                }
            }
            // Si no se pudo determinar el tipo, devolvemos desconocido
            return tipoActual =="" ? "desconocido" : tipoActual;
        }

        private string ObtenerTipoToken(Token token)
        {
            // Si el token es un entero devolvemos int
            if(token.Tipo==TipoToken.INT)
            return "int";

            // Si el token es decimal devolvemos float
            if(token.Tipo==TipoToken.FLOAT)
            return "float";

            // Si el token es cadena o carácter devolvemos char
            if(token.Tipo==TipoToken.CHAR)
            return "char";

            // Si el token es booleano devolvemos bool
            if(token.Tipo==TipoToken.BOOL)
            return"bool";

            // Si el token es un identificador buscamos su tipo en la tabla de símbolos
            if (token.Tipo == TipoToken.ID)
            {
                Simbolo? simbolo = tablaSimbolos.FirstOrDefault(s=>s.Nombre == token.Lexema);
                // Si no encontramos el identificador, significa que no fue declarado
                if (simbolo == null)
                {
                    errores.Add($"Línea {token.Linea}, colunma{token.ColumnaI}: "+ $"Error variable '{token.Lexema}' no declarada");
                    return "error";
                }
                return simbolo.Tipo;
            }
            // Si no corresponde a ningún caso anterior devolvemos desconocido
            return "desconocido";
        }

        private string ObtenerTipoDeclaracion(string tipoToken)
        {
            // Convertimos el nombre del token de tipo a su tipo semántico
            return tipoToken switch
            {
                TipoToken.PRINT => "int",
                TipoToken.PRFLOAT =>"float",
                TipoToken.PRCHAR =>"char",
                TipoToken.PRBOOL =>"bool",
                _ =>"desconocido"
            };
        }
        private bool TiposCompatibles(string destino, string origen)
        {
            // Si ambos tipos son iguales la asignación es válida
            if (destino==origen)
            return true;

            // Permitimos asignar un int a un float porque es una conversión aceptada
            if(destino=="float"&& origen=="int")
            return true;
            // En cualquier otro caso, los tipos no son compatibles
            return false;
        }

        private bool OperacionValida(string tipo1, string tipo2)
        {
            // Permitimos operaciones entre tipos numéricos
            if(EsNumerico(tipo1) && EsNumerico(tipo2))
            return true;

            // Permitimos operaciones entre booleanos
            if(tipo1=="bool"&& tipo2=="bool")
            return true;

            // Si no cumple ninguna regla la operación no es válida
            return false;
        }

        private string ResultadoOperacion(string tipo1, string tipo2)
        {
            // Si algino de los tipos es float el resultado será float
            if(tipo1=="float" || tipo2 == "float")
            return "float";

            // Si ambos tipos son int el resultado será in
            if(tipo1 == "int" && tipo2 == "int")
            return "int";

            // Si ambos tipos son bool, el resultado será bool
            if(tipo1=="bool" && tipo2 == "bool")
            return "bool";

            // Si no se reconoce el resultado, devolvemos desconocido
            return "desconocido";
        }

        private bool EsNumerico(string tipo)
        {
            // Revisamos si el tipo recibido pertenece a los tipos numéricos del lenguaje
            return tipo=="int" || tipo =="float";
        }

        private bool EsOperador(string tipo)
        {
            // Verificamos si el token recibido corresponde a algún operador del lenguaje
            return  tipo == TipoToken.SUM ||
                    tipo == TipoToken.RESTA ||
                    tipo == TipoToken.MULTI ||
                    tipo == TipoToken.DIV ||
                    tipo == TipoToken.PORCENTAJE ||
                    tipo == TipoToken.IGUALIGUAL ||
                    tipo == TipoToken.NOIGUAL ||
                    tipo == TipoToken.MAYORQ ||
                    tipo == TipoToken.MENORQ ||
                    tipo == TipoToken.MAYORIGUAL || 
                    tipo == TipoToken.MENORIGUAL ||
                    tipo == TipoToken.AND ||
                    tipo == TipoToken.OR ||
                    tipo == TipoToken.NOT;
        }

        private object? ObtenerValorSimple(List<Token> expresion)
        {
            // Primero revisamos si la expresión solamente tiene un token.
            // En ese caso no necesitamos hacer ninguna operación, solo guardamos el lexema.
            if (expresion.Count == 1)
                return expresion[0].Lexema;

            // Después revisamos si la expresión tiene exactamente tres partes.
            if (expresion.Count == 3)
            {
                // Guardamos cada parte de la expresión para trabajarla más claro.
                Token izquierda = expresion[0];
                Token operador = expresion[1];
                Token derecha = expresion[2];

                // Verificamos que el valor izquierdo sea numérico.
                bool izquierdaNumero =
                    izquierda.Tipo == TipoToken.INT ||
                    izquierda.Tipo == TipoToken.FLOAT;

                // Verificamos que el valor derecho también sea numérico.
                bool derechaNumero =
                    derecha.Tipo == TipoToken.INT ||
                    derecha.Tipo == TipoToken.FLOAT;

                // Si ambos valores son numéricos, intentamos calcular el resultado.
                if (izquierdaNumero && derechaNumero)
                {
                    // Convertimos los lexemas a double para poder operar enteros y decimales.
                    double num1 = Convert.ToDouble(izquierda.Lexema);
                    double num2 = Convert.ToDouble(derecha.Lexema);

                    // Revisamos qué operador se utilizó y devolvemos el resultado correspondiente.
                    switch (operador.Tipo)
                    {
                        case TipoToken.SUM:
                            // Sumamos ambos valores.
                            return num1 + num2;

                        case TipoToken.RESTA:
                            // Restamos el valor derecho al valor izquierdo.
                            return num1 - num2;

                        case TipoToken.MULTI:
                            // Multiplicamos ambos valores.
                            return num1 * num2;

                        case TipoToken.DIV:
                            // Antes de dividir, evitamos una división entre cero.
                            if (num2 != 0)
                                return num1 / num2;
                            break;

                        case TipoToken.PORCENTAJE:
                            // Calculamos el residuo de la división.
                            return num1 % num2;
                    }
                }
            }

            // Si la expresión no se puede calcular directamente, la guardamos como texto.
            return string.Join(" ", expresion.Select(t => t.Lexema));
        }
        private void ProcesarFuncion(int indice)
        {
            // Verificamos que exista un token después de la palabra reservada def
            if(indice+1 >= tokens.Count)
            return;
            // Guardamos el nombre de la función
            Token nombreFuncion = tokens[indice+1];

            // Si después de def no viene un ID no procesamos la función
            if(nombreFuncion.Tipo != TipoToken.ID)
            return;

            // Revisamos si la función ya fue declarada anteriormente
            if (tablaSimbolos.Any(s => s.Nombre == nombreFuncion.Lexema))
            {
                errores.Add($"Línea {nombreFuncion.Linea}, columna {nombreFuncion.ColumnaI}: Error función '{nombreFuncion.Lexema}' ya declarada");
                return;
            }

            // Creamos una lista para guardar los tipos de los parámetros de la función
            List<string> parametros = new List<string>();

            // Empezamos a leer los parametros después del nombre de la función
            int i = indice + 2;

            // Recorremos hasta encontrar el paréntesis de cierre
            while(i< tokens.Count && tokens[i].Tipo != TipoToken.PAREND)
            {
                // Si encontramos un tipo seguido de un ID, entonces es un parámetro
                if (EsDeclaracion(tokens[i]) && i+1 < tokens.Count && tokens[i+1].Tipo == TipoToken.ID)
                {
                    string tipoParametro = ObtenerTipoDeclaracion(tokens[i].Tipo);
                    Token idParametro = tokens[i+1];

                    // Guardamos el tipo del parámetro para validar lllamadas después
                    parametros.Add(tipoParametro);
                    // Agregamos el parámetro a la tabla si todavía no existe
                    if(!tablaSimbolos.Any(s=> s.Nombre == idParametro.Lexema))
                    {
                        tablaSimbolos.Add(new Simbolo
                        {
                            Nombre = idParametro.Lexema,
                            Tipo = tipoParametro,
                            Categoria = "parametro",
                            Valor = null,
                            Linea = idParametro.Linea,
                            Columna = idParametro.ColumnaI
                        });
                    }
                }
                i++;
            }

            //Agregamos la función a la tabla de símbolos
            tablaSimbolos.Add(new Simbolo
            {
                Nombre= nombreFuncion.Lexema,
                Tipo = "funcion",
                Categoria = "funcion",
                Parametros = parametros,
                Valor = null,
                Linea = nombreFuncion.Linea,
                Columna = nombreFuncion.ColumnaI
            });
        }

        private void ProcesarLlamadaFuncion(int indice)
        {
            // Guardamos el nombre de la función llamada
            Token nombreFuncion = tokens[indice];

            // Buscamos la función en la tabla de símbolos
            Simbolo? funcion = tablaSimbolos.FirstOrDefault( s=> s.Nombre == nombreFuncion.Lexema && s.Categoria == "funcion");
            
            // Si no existe, reportamos error
            if (funcion == null)
            {
                errores.Add($"Línea {nombreFuncion.Linea}, columna {nombreFuncion.ColumnaI}: Error función '{nombreFuncion.Lexema}' no declarada");
                return;
            }
            // Obtenemos los argumentos enviados en la llamada
            List<Token> argumentos = ObtenerArgumentos(indice+2);

            // Verificamos que la cantidad de argumentos coincida con la cantidad de parámetros
            if(argumentos.Count != funcion.Parametros.Count)
            {
                errores.Add($"Línea{nombreFuncion.Linea}, columna {nombreFuncion.ColumnaI}: Errore cantidad incorrecta de argumentos para función '{nombreFuncion.Lexema}'");
                return;
            }

            // Validamos que cada argumento sea compatible con el parámetro correspondiente
            for(int i = 0; i<argumentos.Count; i++)
            {
                string tipoArgumento=ObtenerTipoToken(argumentos[i]);
                string tipoParametro = funcion.Parametros[i];

                if(!TiposCompatibles(tipoParametro, tipoArgumento))
                {
                    errores.Add($"Línes {argumentos[i].Linea}, columna {argumentos[i].ColumnaI}: Errore Argumento para la funcion {nombreFuncion.Lexema} es invalido");
                    return;
                }
            }
        }
        private List<Token> ObtenerArgumentos(int inicio)
        {
            // Creamos una lista para guardar los argumentos de una llamada a función
            List<Token> argumentos = new List<Token>();

            // Recorremos los tokens hasta llegar al paréntesis de cierre
            for (int i = inicio; i<tokens.Count; i++)
            {
                Token actual = tokens[i];

                // Si encontramos el paréntesis derecho, terminamos la lectura de argumentos
                if(actual.Tipo == TipoToken.PAREND)
                break;

                // Ignoramos comas y tokens que no forman parte directa del argumento
                if (actual.Tipo == TipoToken.COMA ||
                    actual.Tipo == TipoToken.NEWLINE ||
                    actual.Tipo == TipoToken.INDENT ||
                    actual.Tipo == TipoToken.DEDENT)
                {
                
                continue;
                    
                }
                // Agregamos el argumento encontrado
                argumentos.Add(actual);
            }
            // Agregamos el argumento encontrado
            return argumentos;
        }

        public void MostrarErrores()
        {
            // Si no encontramos errores semánticos mostramos un mensaje de todo bien
            if (errores.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nOK SEMÁNTICO");
                Console.ResetColor();
                return;
            }
            Console.ForegroundColor=ConsoleColor.Red;
            Console.WriteLine("\n-------------ERRORES SEMÁNTICOS-------------");

            foreach(string error in errores)
            {
                Console.WriteLine(error);
            }
            Console.ResetColor();
        }

        public void MostrarTabla()
        {
            // Mostramos el título
            Console.ForegroundColor=ConsoleColor.Cyan;
            Console.WriteLine("\n-------------TABLA DE SÍMBOLOS-------------");
            Console.ResetColor();

            // Encabezados 
            Console.WriteLine(
                "Nombre".PadRight(20)+
                "TIPO".PadRight(15) +
                "VALOR".PadRight(25)+
                "LÍNEA".PadRight(10)+
                "COLUMNA"
            );
            Console.WriteLine(new string('-', 80));
            // Recorremos todos los símbolos guardados y los mostramos en pantall
            foreach (Simbolo simbolo in tablaSimbolos)
            {
                Console.WriteLine(
                    simbolo.Nombre.PadRight(20)+
                    simbolo.Tipo.PadRight(15)+
                    (simbolo.Valor?.ToString()?? "null").PadRight(25)+
                    simbolo.Linea.ToString().PadRight(10)+simbolo.Columna);
            }
    
        }

        public string ExportarTabla(string rutaOriginal)
        {
            // Obtenemos la carpeta donde está guardado el archivo original
            string carpeta = Path.GetDirectoryName(rutaOriginal)!;
            // Obtenemos el nombre del archivo original sin la extensión .mlng
            string nombre = Path.GetFileNameWithoutExtension(rutaOriginal);
            // Creamos la ruta del archivo de salida para la tabla de símbolos
            string rutaTabla = Path.Combine(
                carpeta,
                $"tabla{nombre}.out"
            );

            // Creamos el archivo y escribimos la tabla de símbolos dentro de él
            using(StreamWriter writer = new StreamWriter(rutaTabla))
            {
                writer.WriteLine("-------------TABLA DE SÍMBOLOS-------------");
                writer.WriteLine();

                writer.WriteLine(
                    "Nombre".PadRight(20) +
                    "TIPO".PadRight(15) +
                    "VALOR".PadRight(25) +
                    "LÍNEA".PadRight(10) +
                    "COLUMNA"
                );

                writer.WriteLine(new string('-', 80));

                // Guardamos cada símbolo con el mismo formato usado en consola
                foreach(Simbolo simbolo in tablaSimbolos)
                {
                    writer.WriteLine(
                        simbolo.Nombre.PadRight(20) +
                        simbolo.Tipo.PadRight(15) +
                        (simbolo.Valor?.ToString() ?? "null").PadRight(25) +
                        simbolo.Linea.ToString().PadRight(10) +
                        simbolo.Columna
                    );
                }
            }

            return rutaTabla;
        }
    }
}
