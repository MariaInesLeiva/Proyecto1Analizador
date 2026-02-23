namespace Proyecto1Analizador
{
    // Token = lo que sale del lexer: tipo + lexema + posición
    public class Token
    {
        // Tipo del token
        public string Tipo { 
            get; 
            private set; 
        }

        // Texto exacto que se leyó del archivo 
        public string Lexema { get; private set; }

        // Indicamos posición, líneas y columnas 
        public int Linea { 
            get; 
            private set; 
        }
        public int ColumnaI { 
            get; 
            private set; 
        }
        public int ColumnaF { 
            get; 
            private set; 
        }

        // Creamos un constructor para guardar la info del token
        public Token(string tipo, string lexema, int linea, int columnaI, int columnaF)
        {
            Tipo = tipo;             
            Lexema = lexema;        
            Linea = linea;         
            ColumnaI = columnaI;   
            ColumnaF = columnaF;    
        }

        // Creamos una función para crear el archivo .out
        public override string ToString()
        {
            // Se creó un formato para la pila que indica: TIPO 'lexema' (Linea,ColumnaI-ColumnaF)
            return Tipo + " '" + Lexema + "' (" + Linea + "," + ColumnaI + "-" + ColumnaF + ")";
        }
    }
}