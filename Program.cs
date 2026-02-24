using System;

namespace Proyecto1Analizador
{
public class Program
{
    static void Main(string[] args)
    {
        //Guardamos la ruta del archivo de entrada
        string rutaEntrada = "";

        // Se toma el archivo de entrada como argumento
        if (args != null && args.Length > 0)
        {
            rutaEntrada = args[0];
        }
        else
        {
            // Si no, se lo pedimos en consola
            Console.Write("Ingrese la ruta del archivo de entrada: ");
            rutaEntrada = Console.ReadLine();
        }

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

        // Mostramos los tokens en consola de forma visual
        Interfaz.Mostrar(tokens);

        // Creamos la ruta de salida .out
        string rutaSalida = Path.ChangeExtension(rutaEntrada, ".out");

        try
        {
            // Escribimos los tokens en el archivo de salida
            using (StreamWriter writer = new StreamWriter(rutaSalida))
            {
                // Recorremos la lista de tokens
                for (int i = 0; i < tokens.Count; i++)
                {
                    // Escribimos cada token en una línea del archivo de salida
                    writer.WriteLine(tokens[i].ToString());
                }
            }

            // Informamos que fue creado el archivo de salida
            Console.WriteLine("Archivo creado: " + rutaSalida);
        }
        catch (Exception ex)
        {
            // Si hay algún error lo informamos
            Console.WriteLine("Error al crear archivo .out: " + ex.Message);
        }

        // Imprimimos los errores léxicos 
        if (lexer.Errores.Count > 0)
        {
            Console.WriteLine("---------- ERRORES LÉXICOS ----------");
            for (int i = 0; i < lexer.Errores.Count; i++)
            {
                Console.WriteLine(lexer.Errores[i]);
            }
        }
        else
        {
            Console.WriteLine("Sin errores léxicos");
        }

        Console.WriteLine("ENTER para salir.");
        Console.ReadLine();
    }
}

}