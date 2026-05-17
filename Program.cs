using System;
using System.IO;
using System.Collections.Generic;

namespace Proyecto1Analizador
{
    public class Program
    {
        static void Main(string[] args)
        {
            string rutaEntrada = "";

            Console.Write("Ingrese la ruta del archivo de entrada: ");
            rutaEntrada = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(rutaEntrada))
            {
                Console.WriteLine("No se cargó ningún archivo");
                return;
            }

            if (!File.Exists(rutaEntrada))
            {
                Console.WriteLine("El archivo no existe");
                return;
            }

            if (Path.GetExtension(rutaEntrada).ToLower() != ".mlng")
            {
                Console.WriteLine("El archivo debe ser .mlng");
                return;
            }

            string codigo = "";

            try
            {
                codigo = File.ReadAllText(rutaEntrada);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer el archivo: " + ex.Message);
                return;
            }

            Lexer lexer = new Lexer(codigo);
            List<Token> tokens = lexer.Tokenizar();

            bool hayLexicos = lexer.Errores.Count > 0;

            Interfaz.MostrarTokens(tokens);

            if (hayLexicos)
            {
                Interfaz.MostrarErrores(lexer.Errores);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
                Console.WriteLine("No se encontraron errores léxicos");
                Console.ResetColor();
            }

            ControlSintactico control = new ControlSintactico();
            LectorTokens lector = new LectorTokens(tokens, control);
            Parser parser = new Parser(lector, control);

            bool resultadoParse = false;

            try
            {
                resultadoParse = parser.Parse();
            }
            catch (Exception ex)
            {
                control.AgregarError("Error en el análisis: " + ex.Message);
            }

            if (!resultadoParse && control.errores.Count == 0)
            {
                if (control.tokenActual != null)
                {
                    control.AgregarError("estructura incompleta cerca de '" + control.tokenActual.Lexema + "'");
                }
                else
                {
                    control.AgregarError("Error sintáctico");
                }
            }

            bool haySintacticos = control.errores.Count > 0;

            Interfaz.MostrarErroresSintacticos(control.errores);

            if (!haySintacticos)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
                Console.WriteLine("No se encontraron errores sintácticos");
                Console.ResetColor();
            }

            Interfaz.MostrarTituloSemantico();
            string rutaTabla = "";

            if (!hayLexicos && !haySintacticos)
            {
                AnalizadorSemantico semantico = new AnalizadorSemantico(tokens);
                semantico.Analizar();
                semantico.MostrarErrores();
                semantico.MostrarTabla();

                rutaTabla = semantico.ExportarTabla(rutaEntrada);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nNo se ejecutó el análisis semántico porque hay errores léxicos o sintácticos");
                Console.ResetColor();
            }


            string rutaSalida = Path.ChangeExtension(rutaEntrada, ".out");

            try
            {
                using (StreamWriter writer = new StreamWriter(rutaSalida))
                {
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        writer.WriteLine(tokens[i].ToString());
                    }
                }

                Console.WriteLine("Archivo creado: " + rutaSalida);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear archivo .out: " + ex.Message);
            }

            Console.WriteLine("\nENTER para salir.");
            Console.ReadLine();
        }
    }
}