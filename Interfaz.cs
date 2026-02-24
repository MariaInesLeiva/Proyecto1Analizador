using System;
using System.Collections.Generic;
using System.Threading;

namespace Proyecto1Analizador
{
    public static class Interfaz
    {
        public static void MostrarTokens(List<Token> tokens)
        {
            MostrarAnimacion("-----------------------------ANALIZADOR LÉXICO-----------------------------", ConsoleColor.Blue);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-----------------------TOKENS-----------------------");
            Console.WriteLine();

            Console.WriteLine(
                "TIPO".PadRight(15) +
                "LEXEMA".PadRight(20) +
                "POSICIÓN"
            );

            Console.WriteLine(new string('-', 55));

            foreach (var t in tokens)
            {
                PonerColor(t.Tipo);

                Console.Write(t.Tipo.PadRight(15));
                Console.ResetColor();

                string lex = (t.Lexema ?? "").Replace("\t", "\\t");
                if (lex.Length > 18) lex = lex.Substring(0, 15) + "...";
                lex = lex.PadRight(20);

                Console.WriteLine(
                    lex +
                    $"({t.Linea},{t.ColumnaI}-{t.ColumnaF})"
                );
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        public static void MostrarErrores(List<string> errores)
        {
            if (errores == null || errores.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nSin errores léxicos.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("--------------------ERRORES LÉXICOS--------------------");
            Console.WriteLine("N°".PadRight(5) + "DESCRIPCIÓN");
            Console.WriteLine(new string('-', 55));

            for (int i = 0; i < errores.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write((i + 1).ToString().PadRight(5));
                Console.ResetColor();

                Console.WriteLine(errores[i]);
            }

            Console.WriteLine();
        }

        private static void PonerColor(string tipo)
        {
            if (tipo == TipoToken.ERROR)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (tipo == TipoToken.ID)
                Console.ForegroundColor = ConsoleColor.Cyan;
            else if (tipo == TipoToken.INT || tipo == TipoToken.FLOAT)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (tipo != null && tipo.StartsWith("PR")) // reservadas
                Console.ForegroundColor = ConsoleColor.Green;
            else if (tipo == TipoToken.NEWLINE || tipo == TipoToken.INDENT || tipo == TipoToken.DEDENT)
                Console.ForegroundColor = ConsoleColor.DarkGray;
            else
                Console.ForegroundColor = ConsoleColor.White; // ops/símbolos
        }

        private static void MostrarAnimacion(string texto, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            foreach (char c in texto)
            {
                Console.Write(c);
                Thread.Sleep(120); // pausa entre cada letra
            }

            Console.ResetColor();
            Console.WriteLine(); // salto de línea al final
        }
    }
}
