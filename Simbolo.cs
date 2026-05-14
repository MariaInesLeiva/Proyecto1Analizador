namespace Proyecto1Analizador
{
    public class Simbolo
    {
        public string Nombre { get; set;} = "";
        public string Tipo { get; set;} = "";
        public object? Valor { get; set;} 
        public string Categoria { get; set; } = "variable";
        public List<string> Parametros { get; set; } = new List<string>();
        public int Linea { get; set;}
        public int Columna { get; set;}
    }
}