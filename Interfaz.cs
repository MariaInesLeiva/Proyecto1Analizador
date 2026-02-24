using System;
using System.Collections.Generic;

namespace Proyecto1Analizador
{
    public static class Interfaz
    {
        public static void Mostrar(List<Token> tokens)
        {
            Console.WriteLine();
            Console.WriteLine("-----------TOKENS----------");
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

                Console.Write(
                    t.Tipo.PadRight(15)
                );

                Console.ResetColor();

                Console.Write(
                    t.Lexema.PadRight(20) +
                    $"({t.Linea},{t.ColumnaI}-{t.ColumnaF})"
                );

                Console.WriteLine();
            }

            Console.ResetColor();
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

            else if (tipo.StartsWith("PR"))   // palabras reservadas
                Console.ForegroundColor = ConsoleColor.Green;

            else if (tipo == TipoToken.NEWLINE ||
                     tipo == TipoToken.INDENT ||
                     tipo == TipoToken.DEDENT)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            else // operadores y símbolos
                Console.ForegroundColor = ConsoleColor.Magenta;
        }
    }
}