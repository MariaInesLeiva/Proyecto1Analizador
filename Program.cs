using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;

namespace Proyecto1Analizador
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Guardamos la ruta del archivo de entrada
            string rutaEntrada = "";

            // Lo pedimos en consola
            Console.Write("Ingrese la ruta del archivo de entrada: ");
            rutaEntrada = Console.ReadLine();

            // Validaciones
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

            // Solo aceptamos archivos .mlng
            if (Path.GetExtension(rutaEntrada).ToLower() != ".mlng")
            {
                Console.WriteLine("El archivo debe ser .mlng");
                return;
            }

            // Leemos lo que está en el archivo
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

            // Creamos el lexer y tokenizamos el código
            Lexer lexer = new Lexer(codigo);
            List<Token> tokens = lexer.Tokenizar();

            ControlSintactico control = new ControlSintactico();
            LectorTokens lector = new LectorTokens(tokens, control);
            Parser parser = new Parser(lector, control);

            try
            {
                parser.Parse(); //se ejecuta el análisis sintáctico
            }

            catch(Exception ex) //Se manejan los errores
            {
                control.AgregarError("Error durante el análisis: "+ex.Message);
            }

            bool hayLexicos=lexer.Errores.Count > 0; //valida los errores léxicos
            bool haySintacticos = control.errores.Count >0; //valida los errores sintácticos

            if(!hayLexicos && !haySintacticos) //condición para imprimir el resultado
            {
                Console.WriteLine ("OK");
            
            }
            else
            {
                if (hayLexicos)
                {
                    Interfaz.MostrarErrores(lexer.Errores);
                }
                if (haySintacticos)
                {
                    Interfaz.MostrarErroresSintacticos(control.errores);
                    
                }
            }

            // Mostramos en la consola las animaciones 
            Interfaz.MostrarTokens(tokens);

            // Creamos la ruta de salida .out
            string rutaSalida = Path.ChangeExtension(rutaEntrada, ".out");

            try
            {
                // Escribimos los tokens en el archivo de salida
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