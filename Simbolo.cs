namespace Proyecto1Analizador
{
    public class Simbolo
    {
        public string Nombre { get; set;} = "";
        public string Tipo { get; set;} = "";
        public object? Valor { get; set;} 
        public int Linea { get; set;}
        public int Columna { get; set;}
    }
}