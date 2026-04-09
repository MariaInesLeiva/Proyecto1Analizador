using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Proyecto1Analizador
{
    public class Lexer
    {
        // Texto completo del archivo
        private string texto;

        // Creamos el índice para la posición en el texto
        private int posicion;

        // Indicamos la línea y columna actual
        private int linea;
        private int columna;

        // Mandamos a llamar la lista de reglas del lexer
        private List<ReglaLexer> reglas;

        // Mandamos a llamar la cola para los tokens pendientes
        private Queue<Token> pendientes;

        // Mandamos a llamar la pila de indentación 
        private Stack<int> pilaIndent;

        // Si estamos al inicio de una línea 
        private bool inicioLinea;

        // Mandamos a llamar la lista de errores para imprimirlos al final 
        public List<string> Errores { 
            get; 
            private set; 
        }

        public Lexer(string textoEntrada)
        {
            texto = textoEntrada;              // guardamos texto
            posicion = 0;                      // inicializamos la posición con 0
            linea = 1;                         // inicializamos la primer lína con 1
            columna = 1;                       // inicializamos la primera columna con 1

            reglas = new List<ReglaLexer>();   // creamos el objeto del tipo lista de reglas
            pendientes = new Queue<Token>();   // creamos el objeto de cola de tokens generados extra
            pilaIndent = new Stack<int>();     // creamos el objeto de pila indent
            pilaIndent.Push(0);                // creamos el objeto de indent inicial = 0

            inicioLinea = true;                // al inicio del archivo estamos en el inicio de línea
            Errores = new List<string>();      // creamos el objeto de lista de errores

            InicializarReglas();               // mandamos a llarmar a la función para cargar todas las regex
        }

        private void InicializarReglas()
        {
            // reconocemos el salto de línea como un token 
            reglas.Add(new ReglaLexer(TipoToken.NEWLINE, @"\r?\n"));

            // Operadores dobles
            reglas.Add(new ReglaLexer(TipoToken.MAYORIGUAL, @">="));
            reglas.Add(new ReglaLexer(TipoToken.MENORIGUAL, @"<="));
            reglas.Add(new ReglaLexer(TipoToken.IGUALIGUAL, @"=="));
            reglas.Add(new ReglaLexer(TipoToken.NOIGUAL, @"!="));

            // Operadores 
            reglas.Add(new ReglaLexer(TipoToken.MAYORQ, @">"));
            reglas.Add(new ReglaLexer(TipoToken.MENORQ, @"<"));
            reglas.Add(new ReglaLexer(TipoToken.IGUAL, @"="));
            reglas.Add(new ReglaLexer(TipoToken.SUM, @"\+"));
            reglas.Add(new ReglaLexer(TipoToken.RESTA, @"-"));
            reglas.Add(new ReglaLexer(TipoToken.MULTI, @"\*"));
            reglas.Add(new ReglaLexer(TipoToken.DIV, @"/"));
            reglas.Add(new ReglaLexer(TipoToken.PORCENTAJE, @"%"));

            // Símbolos
            reglas.Add(new ReglaLexer(TipoToken.PARENI, @"\("));
            reglas.Add(new ReglaLexer(TipoToken.PAREND, @"\)"));
            reglas.Add(new ReglaLexer(TipoToken.COMA, @","));

            // Palabras reservadas
            reglas.Add(new ReglaLexer(TipoToken.PRINT, @"\bint\b"));
            reglas.Add(new ReglaLexer(TipoToken.PRFLOAT, @"\bfloat\b"));
            reglas.Add(new ReglaLexer(TipoToken.PRCHAR, @"\bchar\b"));
            reglas.Add(new ReglaLexer(TipoToken.PRBOOL, @"\bbool\b"));

            reglas.Add(new ReglaLexer(TipoToken.PRIF, @"\bif\b"));
            reglas.Add(new ReglaLexer(TipoToken.PRELIF, @"\belif\b"));
            reglas.Add(new ReglaLexer(TipoToken.PRELSE, @"\belse\b"));
            reglas.Add(new ReglaLexer(TipoToken.PRWHILE, @"\bwhile\b"));

            reglas.Add(new ReglaLexer(TipoToken.PRREAD, @"\bRead\b"));
            reglas.Add(new ReglaLexer(TipoToken.PRWRITE, @"\bWrite\b"));

            reglas.Add(new ReglaLexer(TipoToken.PRDEF, @"\bdef\b"));
            reglas.Add(new ReglaLexer(TipoToken.PRRETURN, @"\breturn\b"));

            //Or/and/not
            reglas.Add(new ReglaLexer(TipoToken.OR, @"\bor\b"));
            reglas.Add(new ReglaLexer(TipoToken.AND, @"\band\b"));
            reglas.Add(new ReglaLexer(TipoToken.NOT, @"\bnot\b"));

            // BOOL 
            reglas.Add(new ReglaLexer(TipoToken.BOOL, @"\btrue\b|\bfalse\b"));

            // Números
            reglas.Add(new ReglaLexer(TipoToken.FLOAT, @"[0-9]+\.[0-9]+"));
            reglas.Add(new ReglaLexer(TipoToken.INT, @"[0-9]+"));

            // ID 
            reglas.Add(new ReglaLexer(TipoToken.ID, @"[a-zA-Z][a-zA-Z0-9]*"));
        }

        public List<Token> Tokenizar()  // creamos el método para devolver la isra de tokens
        {
            List<Token> lista = new List<Token>();   // creamos la lista para guardar los tokens

            while (true)
            {
                Token t = SiguienteToken();          // llamamos a la función indicada para analizar el texto
                lista.Add(t);                        // guardamos el token en la lista

                if (t.Tipo == TipoToken.FP)          // creamos una condición para que deje de analizar si llega a FP
                {
                    break;
                }
            }

            return lista;                             // devolvemos la lista completa
        }

        public Token GetToken(List<Token> lista, ref int idx) //creamos una función para devolcer el token actual y avanzar de pindice
        {
            if (idx < lista.Count)
            {
                Token t = lista[idx];
                idx++;
                return t;
            }

            return new Token(TipoToken.FP, "EOF", linea, columna, columna);
        }

        public Token Peek(List<Token> lista, int idx) // creamos una función para ver el token 
        {
            if (idx < lista.Count)
            {
                return lista[idx];
            }

            return new Token(TipoToken.FP, "EOF", linea, columna, columna);
        }

        private Token SiguienteToken()
        {
            // Creamos una condición para devolver los tokens que se encontraban pendientes
            if (pendientes.Count > 0)
            {
                return pendientes.Dequeue();
            }

            // creamos una condición en donde si es el final de texto, se saca DEDENT pendientes
            if (posicion >= texto.Length)
            {
                while (pilaIndent.Count > 1)
                {
                    pilaIndent.Pop(); // bajamos un nivel 
                    pendientes.Enqueue(new Token(TipoToken.DEDENT, "", linea, columna, columna));
                }

                if (pendientes.Count > 0)
                {
                    return pendientes.Dequeue();
                }

                return new Token(TipoToken.FP, "EOF", linea, columna, columna);
            }

            // creamos una condición en donde si es el inicio de línea, indentación primero
            if (inicioLinea)
            {
                ManejarIndentacion();

                if (pendientes.Count > 0)
                {
                    return pendientes.Dequeue();
                }
            }

            // creamos una condición para ignorar espacios/tabs
            if (texto[posicion] == ' ' || texto[posicion] == '\t')
            {
                ConsumirEspacios();
                return SiguienteToken();
            }

            // creamos una condición para ignorar comentarios #
            if (texto[posicion] == '#')
            {
                SaltarComentario();
                return SiguienteToken();
            }

            // creamos una condición para detectar string sin cerrar
            if (texto[posicion] == '"')
            {
                return EscanearString();
            }

            // probamos las reglas en orden 
            for (int i = 0; i < reglas.Count; i++)
            {
                ReglaLexer regla = reglas[i];
                Match m = regla.Patron.Match(texto, posicion);

                // creamos una condición en donde match tiene que iniciar exacto en posicion actual
                if (m.Success && m.Index == posicion)
                {
                    string lexema = m.Value;

                    int colI = columna;
                    int colF = columna + lexema.Length - 1;

                    // creamos una condición en donde newline es token especial
                    if (regla.Tipo == TipoToken.NEWLINE)
                    {
                        Avanzar(lexema);             // llamamos al método para avanzar y actualizar línea y columna
                        inicioLinea = true;          // definimos que la siguiente lectura es inicio de línea
                        return new Token(TipoToken.NEWLINE, "\\n", linea - 1, colI, colI);
                    }

                    // creamos una condición en donde si un id tiene más de 31 carácteres, se produce error
                    if (regla.Tipo == TipoToken.ID)
                    {
                        string original = lexema;

                        if (original.Length > 31) 
                        {
                            string recorte = original.Substring(0, 31);
                            string msg = "ID demasiado largo en linea " + linea +
                                         ", col " + colI + ": '" + original +
                                         "'. Se recortó y quedó como: '" + recorte + "'";
                            Errores.Add(msg);

                            Avanzar(original);       // llamamos al método para consumir todo en el texto
                            inicioLinea = false;     // definimos que ya no se está en el inicio
                            return new Token(TipoToken.ID, recorte, linea, colI, colI + 31 - 1);
                        }

                        Avanzar(original);           // llamamos a la función y se consume ID normal
                        inicioLinea = false;         // definidmos que ya hubo token
                        return new Token(TipoToken.ID, original, linea, colI, colF);
                    }

                    // Token normal
                    Avanzar(lexema);                 // llamamos a la función para avanzar
                    inicioLinea = false;             // indicamos que ya no es inicio de línea
                    return new Token(regla.Tipo, lexema, linea, colI, colF);
                }
            }

            // Indicamos que si el caracter no coincidió con alguna ER, el lexer no lo identifica como simbolo
            string ch = texto[posicion].ToString();
            string err = "Carácter inválido '" + ch + "' en linea " + linea + ", col " + columna;
            Errores.Add(err);

            Token tErr = new Token(TipoToken.ERROR, ch, linea, columna, columna);
            Avanzar(ch);                             // avanza 1 caracter
            inicioLinea = false;                     // indicamos que ya leyó algo
            return tErr;
        }

        private void ManejarIndentacion()
        {
            // si se encuentra en NEWLINE o inicio del archivo
            // Medimos la identación 
            int colInicio = columna;                  // guardamos la columna actual

            int posTemp = posicion;                   // no se consume todavía
            int count = 0;                            // indent en espacios

            // creamos un bucle para contar espacios o tabs
            while (posTemp < texto.Length)
            {
                char c = texto[posTemp];

                if (c == ' ')
                {
                    count++;
                    posTemp++;
                    continue;
                }

                if (c == '\t')
                {
                    // Si encuentra tab, suma 4 espacios 
                    count += 4;
                    posTemp++;
                    continue;
                }

                break;
            }

            // creamos una condición en donde si hay linea vacía no hay cambio indent
            if (posTemp < texto.Length)
            {
                if (texto[posTemp] == '\n' || texto[posTemp] == '\r')
                {
                    inicioLinea = true;
                    return;
                }
            }

            // creamos una condición por si línea solo tiene comentario no haya cambio indent
            if (posTemp < texto.Length && texto[posTemp] == '#')
            {
                // consumimos espacios iniciales para quedar en #
                ConsumirEspaciosAlInicio();
                inicioLinea = false;
                return;
            }

            // Consumimos los espacios/tabs del inicio
            ConsumirEspaciosAlInicio();

            // Comparamos con el nivel anterior
            int actual = count;
            int anterior = pilaIndent.Peek();

            // creamos una condición en donde si sube indentación es igual a INDENT
            if (actual > anterior)
            {
                pilaIndent.Push(actual);
                pendientes.Enqueue(new Token(TipoToken.INDENT, "", linea, colInicio, colInicio));
            }
            else if (actual < anterior)
            {
                // si baja indentación se sacan n DEDENT hasta empatar
                bool ok = false;

                while (pilaIndent.Count > 1)
                {
                    int top = pilaIndent.Peek();

                    if (top == actual)
                    {
                        ok = true;
                        break;
                    }

                    pilaIndent.Pop();
                    pendientes.Enqueue(new Token(TipoToken.DEDENT, "", linea, colInicio, colInicio));
                }

                // condición para indicar que si ningún nivel es válido se debe indicar error de indentación
                if (!ok && pilaIndent.Peek() != actual)
                {
                    string msg = "Indentación inválida en linea " + linea + " (nivel " + actual + ")";
                    Errores.Add(msg);
                }
            }

            // indicamos que la línea ya no es inicio
            inicioLinea = false;
        }

        private void ConsumirEspaciosAlInicio()
        {
            // creamos un bucle para consumir espacios/tabs y actualizar columna/posicion
            while (posicion < texto.Length)
            {
                char c = texto[posicion];

                if (c == ' ')
                {
                    posicion++;
                    columna++;
                    continue;
                }

                if (c == '\t')
                {
                    posicion++;
                    columna += 4; // indicamos que tab es igual a 4
                    continue;
                }

                break;
            }
        }

        private void ConsumirEspacios()
        {
            // creamos un bucle para ignorar los espacios/tabs normales dentro de línea
            while (posicion < texto.Length)
            {
                char c = texto[posicion];

                if (c == ' ')
                {
                    posicion++;
                    columna++;
                }
                else if (c == '\t')
                {
                    posicion++;
                    columna += 4;
                }
                else
                {
                    break;
                }
            }
        }

        private void SaltarComentario()
        {
            // creamos un bucle para saltar desde '#' hasta antes del newline
            while (posicion < texto.Length)
            {
                char c = texto[posicion];

                if (c == '\n' || c == '\r')
                {
                    break;
                }

                posicion++;
                columna++;
            }
        }

        private Token EscanearString()
        {
            // indicamos que el lexer detecta que el carácter actual es uan comilla
            int colI = columna;             // guardamos la columna donde inicia la comilla
            int startLine = linea;          // guardamos la línea donde inicia

            // indicamos que avanza y consume la comilla inicial
            posicion++;
            columna++;

            string contenido = "";          // indicamos que se acumulan los caracteres
            bool cerrada = false;           // indicamos que si encontró comilla final se vuelve false

            // creamos un bucle para leer hasta encontrar otra comilla o error
            while (posicion < texto.Length)
            {
                char c = texto[posicion];

                // creamos una condición que si encuentra la comilla final, cierra
                if (c == '"')
                {
                    cerrada = true;
                    posicion++;
                    columna++;
                    break;
                }

                // creamos una condición que si encuentra un salto de línea, es string sin cerrar
                if (c == '\n' || c == '\r')
                {
                    break;
                }

                contenido += c;             // acumulamos el char
                posicion++;
                columna++;
            }

            // creamos una condición que si no se cerró, reporto error y devuelve error
            if (!cerrada)
            {
                string msg = "String sin cerrar en linea " + startLine + ", col " + colI;
                Errores.Add(msg);

                // devolvemos lo que lleva para que sea visible en el .out
                return new Token(TipoToken.ERROR, "\"" + contenido, startLine, colI, columna);
            }

            if (!Regex.IsMatch(contenido, @"^[a-zA-Z0-9 ]*$"))
            {
                string msg = "CHAR inválido en linea " + startLine + ", col " + colI +
                            ": solo se permiten letras, números y espacios";
                Errores.Add(msg);
                return new Token(TipoToken.ERROR, "\"" + contenido + "\"", startLine, colI, columna - 1);
            }

            // indicamos que si sí se cerró, se devuelve token char con comillas incluidas
            int colF = columna - 1;
            return new Token(TipoToken.CHAR, "\"" + contenido + "\"", startLine, colI, colF);
        }

        private void Avanzar(string lexema)
        {
            // creamos un bucle para avanzar carácter por carácter y actualizar línea/columna bien
            for (int i = 0; i < lexema.Length; i++)
            {
                char c = texto[posicion];

                if (c == '\n')
                {
                    linea++;       // pasa a la siguiente línea 
                    columna = 1;   // se reinicia la columna
                }
                else if (c == '\r')
                {
                    // consideramos como parte del salto a \r\n
                    columna = 1;
                }
                else
                {
                    columna++;     // definimos que si no es salto de línea solo incrementa una columna
                }

                posicion++;        // definimos que siempre avanza al siguiente carácter
            }
        }
    }
}