using System.Text.RegularExpressions;

namespace Proyecto1Analizador
{
    // Reglas para saber qué token es, qué regex lo reconoce o si se ignora
    public class ReglaLexer
    {
        public string Tipo { 
            get; 
            private set; 
        }

        // Regex para poder hacer Match
        public Regex Patron { 
            get; 
            private set; 
        }

        // Si es true, el lexer lo reconoce pero no lo guarda como token 
        public bool Ignorar { 
            get; 
            private set; 
        }

        // Constructor
        public ReglaLexer(string tipo, string patron, bool ignorar)
        {
            Tipo = tipo;                  
            Patron = new Regex(patron);   
            Ignorar = ignorar;            
        }

        // Constructor cuando no se ignora
        public ReglaLexer(string tipo, string patron)
        {
            Tipo = tipo;               
            Patron = new Regex(patron);
            Ignorar = false;              
        }
    }
}