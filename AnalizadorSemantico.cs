using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace Proyecto1Analizador
{
    public class AnalizadorSemantico
    {
        private readonly List<Token> tokens;
        private readonly List<string> errores;
        private readonly List<Simbolo>tablaSimbolos;

        public List<String> Errores => errores;
        public List<Simbolo> TablaSimbolos => tablaSimbolos;

        public AnalizadorSemantico(List<Token> tokensEntrada)
        {
            tokens = tokensEntrada;
            errores = new List<string>();
            tablaSimbolos = new List<Simbolo>();

        }

        public void Analizar()
        {
            for(int i=0; i <tokens.Count; i++)
            {
                Token actual = tokens[i];
                if(actual.Tipo==TipoToken.FP)
                break;

                if(actual.Tipo == TipoToken.PRDEF)
                {
                    ProcesarFuncion(i);
                    while (i < tokens.Count && tokens[i].Tipo != TipoToken.NEWLINE)
                    {
                        i++;
                    }
                    continue;
                }

                if (EsDeclaracion(actual))
                {
                    ProcesarDeclaracion(i);
                }
                if (actual.Tipo == TipoToken.ID)
                {
                    if(i+1 < tokens.Count && tokens[i+1].Tipo == TipoToken.PARENI)
                    {
                        ProcesarLlamadaFuncion(i);
                    }
                    else
                    {
                        ValidarUsoVariable(actual);
                        if(i+1< tokens.Count && tokens[i+1].Tipo == TipoToken.IGUAL)
                        {
                            ProcesarAsignacion(i);
                        }
                    }
                }
                
            }
        }
    

    private bool EsDeclaracion(Token token)
        {
            return  token.Tipo==TipoToken.PRINT ||
                    token.Tipo==TipoToken.PRFLOAT ||
                    token.Tipo==TipoToken.PRCHAR ||
                    token.Tipo==TipoToken.PRBOOL;
        }

    private void ProcesarDeclaracion(int indice)
        {
            if(indice+1 >= tokens.Count)
            return;

            Token tipo = tokens[indice];
            Token identificador = tokens[indice+1];

            if(identificador.Tipo != TipoToken.ID)
            return;

            bool existe = tablaSimbolos.Any(s=> s.Nombre == identificador.Lexema);

            if(existe)
            {
                errores.Add(
                    $"Línea {identificador.Linea}, columna {identificador.ColumnaI}: " + $"Error variable '{identificador.Lexema}' ya declarada");
                    return;
            }

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
            bool existe = tablaSimbolos.Any(s=> s.Nombre == token.Lexema);
            if (!existe)
            {
                errores.Add($"Línea {token.Linea}, columna {token.ColumnaI}: "+ $"Error variable '{token.Lexema}' no declarada");
            }
        }

        private void ProcesarAsignacion(int indice)
        {
            Token variable=tokens[indice];
            Simbolo? simbolo  = tablaSimbolos.FirstOrDefault(s=>s.Nombre==variable.Lexema);
            if (simbolo==null)
            return;

            List<Token> expresion=ObtenerExpresion(indice+2);

            if(expresion.Count==0)
            return;

            string tipoExpresion=EvaluarTipoExpresion(expresion);
            if(tipoExpresion=="error")
            return;

            if(!TiposCompatibles(simbolo.Tipo, tipoExpresion))
            {
                errores.Add($"Línea{variable.Linea}, columna {variable.ColumnaI}: "+ $"Error no se puede asignar '{tipoExpresion}' a '{simbolo.Tipo}'");
                return;
            }
            simbolo.Valor = ObtenerValorSimple(expresion);

        }

        private List<Token> ObtenerExpresion(int inicio)
        {
            List<Token> expresion = new List<Token>();
            for(int i=inicio; i<tokens.Count; i++)
            {
                Token actual = tokens[i];

                if(actual.Tipo==TipoToken.NEWLINE||
                actual.Tipo ==TipoToken.INDENT ||
                actual.Tipo== TipoToken.DEDENT||
                actual.Tipo == TipoToken.FP)
                {
                    break;
                }
                expresion.Add(actual);
            }
            return expresion;
        }

        private string EvaluarTipoExpresion(List<Token> expresion)
        {
            string tipoActual="";

            foreach(Token token in expresion)
            {
                if(EsOperador(token.Tipo)||
                token.Tipo==TipoToken.PARENI ||
                token.Tipo==TipoToken.PAREND ||
                token.Tipo == TipoToken.COMA)
                {
                    continue;
                }

                string tipoToken = ObtenerTipoToken(token);

                if(tipoToken == "desconcodio")
                continue;

                if(tipoToken == "error")
                return "eeror";

                if (tipoActual == "")
                {
                    tipoActual=tipoToken;
                }
                else
                {
                    if(!OperacionValida(tipoActual, tipoToken))
                    {
                        errores.Add($"Línea{token.Linea}, columan{token.ColumnaI}: " + $"Error no se puede operar el tipo '{tipoActual}' y '{tipoToken}'");
                        return "error";
                    }
                    tipoActual=ResultadoOperacion(tipoActual, tipoToken);
                }
            }
            return tipoActual =="" ? "desconocido" : tipoActual;
        }

        private string ObtenerTipoToken(Token token)
        {
            if(token.Tipo==TipoToken.INT)
            return "int";

            if(token.Tipo==TipoToken.FLOAT)
            return "float";

            if(token.Tipo==TipoToken.CHAR)
            return "char";

            if(token.Tipo==TipoToken.BOOL)
            return"bool";

            if (token.Tipo == TipoToken.ID)
            {
                Simbolo? simbolo = tablaSimbolos.FirstOrDefault(s=>s.Nombre == token.Lexema);
                if (simbolo == null)
                {
                    errores.Add($"Línea {token.Linea}, colunma{token.ColumnaI}: "+ $"Error variable '{token.Lexema}' no declarada");
                    return "error";
                }
                return simbolo.Tipo;
            }
            return "desconocido";
        }

        private string ObtenerTipoDeclaracion(string tipoToken)
        {
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
            if (destino==origen)
            return true;

            if(destino=="float"&& origen=="int")
            return true;
            return false;
        }

        private bool OperacionValida(string tipo1, string tipo2)
        {
            if(EsNumerico(tipo1) && EsNumerico(tipo2))
            return true;

            if(tipo1=="bool"&& tipo2=="bool")
            return true;

            return false;
        }

        private string ResultadoOperacion(string tipo1, string tipo2)
        {
            if(tipo1=="float" || tipo2 == "float")
            return "float";

            if(tipo1 == "int" && tipo2 == "int")
            return "int";

            if(tipo1=="bool" && tipo2 == "bool")
            return "bool";

            return "desconocido";
        }

        private bool EsNumerico(string tipo)
        {
            return tipo=="int" || tipo =="float";
        }

        private bool EsOperador(string tipo)
        {
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
            if(expresion.Count ==1)
            return expresion[0].Lexema;

            return string.Join(" ", expresion.Select(t => t.Lexema));
        }
        private void ProcesarFuncion(int indice)
        {
            if(indice+1 >= tokens.Count)
            return;
            Token nombreFuncion = tokens[indice+1];

            if(nombreFuncion.Tipo != TipoToken.ID)
            return;

            if (tablaSimbolos.Any(s => s.Nombre == nombreFuncion.Lexema))
            {
                errores.Add($"Línea {nombreFuncion.Linea}, columna {nombreFuncion.ColumnaI}: Error función '{nombreFuncion.Lexema}' ya declarada");
                return;
            }

             List<string> parametros = new List<string>();

            int i = indice + 2;

            while(i< tokens.Count && tokens[i].Tipo != TipoToken.PAREND)
            {
                if (EsDeclaracion(tokens[i]) && i+1 < tokens.Count && tokens[i+1].Tipo == TipoToken.ID)
                {
                    string tipoParametro = ObtenerTipoDeclaracion(tokens[i].Tipo);
                    Token idParametro = tokens[i+1];

                    parametros.Add(tipoParametro);
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
            Token nombreFuncion = tokens[indice];
            Simbolo? funcion = tablaSimbolos.FirstOrDefault( s=> s.Nombre == nombreFuncion.Lexema && s.Categoria == "funcion");
            if (funcion == null)
            {
                errores.Add($"Línea {nombreFuncion.Linea}, columna {nombreFuncion.ColumnaI}: Error función '{nombreFuncion.Lexema}' no declarada");
                return;
            }
            List<Token> argumentos = ObtenerArgumentos(indice+2);

            if(argumentos.Count != funcion.Parametros.Count)
            {
                errores.Add($"Línea{nombreFuncion.Linea}, columna {nombreFuncion.ColumnaI}: Errore cantidad incorrecta de argumentos para función '{nombreFuncion.Lexema}'");
                return;
            }
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
            List<Token> argumentos = new List<Token>();
            for (int i = inicio; i<tokens.Count; i++)
            {
                Token actual = tokens[i];

                if(actual.Tipo == TipoToken.PAREND)
                break;

                if (actual.Tipo == TipoToken.COMA ||
                    actual.Tipo == TipoToken.NEWLINE ||
                    actual.Tipo == TipoToken.INDENT ||
                    actual.Tipo == TipoToken.DEDENT)
                {
                
                continue;
                    
                }

                 argumentos.Add(actual);
            }
            return argumentos;
        }

        public void MostrarErrores()
        {
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
            Console.ForegroundColor=ConsoleColor.Cyan;
            Console.WriteLine("\n-------------TABLA DE SÍMBOLOS-------------");
            Console.ResetColor();

            Console.WriteLine(
                "Nombre".PadRight(20)+
                "TIPO".PadRight(15) +
                "VALOR".PadRight(25)+
                "LÍNEA".PadRight(10)+
                "COLUMNA"
            );
            Console.WriteLine(new string('-', 80));
            foreach (Simbolo simbolo in tablaSimbolos)
            {
                Console.WriteLine(
                    simbolo.Nombre.PadRight(20)+
                    simbolo.Tipo.PadRight(15)+
                    (simbolo.Valor?.ToString()?? "null").PadRight(25)+
                    simbolo.Linea.ToString().PadRight(10)+simbolo.Columna);
            }
    
        }
    }
}
