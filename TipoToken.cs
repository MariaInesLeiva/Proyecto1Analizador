namespace Proyecto1Analizador
{
    // Guardamos los "nombres" de los tokens como strings 
    public static class TipoToken
    {
        // Tipos para declarar variables
        public const string PRINT = "PRINT";    
        public const string PRFLOAT = "PRFLOAT";    
        public const string PRCHAR = "PRCHAR";    
        public const string PRBOOL = "PRBOOL";      

        // Funciones
        public const string PRIF = "PRIF";  
        public const string PRELIF = "PRELIF";
        public const string PRELSE = "PRELSE";     
        public const string PRWHILE = "PRWHILE";    
        public const string PRDEF = "PRDEF";        
        public const string PRRETURN = "PRRETURN";  

        // Entrada y salida
        public const string PRREAD = "PRREAD";     
        public const string PRWRITE = "PRWRITE";   

        // Varoles y variables
        public const string INT = "INT";            
        public const string FLOAT = "FLOAT";      
        public const string CHAR = "CHAR";          
        public const string BOOL = "BOOL";          
        public const string ID = "ID";             

        // Operadores or/and/not 
        public const string OR = "OR";
        public const string AND = "AND";
        public const string NOT = "NOT";
        
        
        // Operadores
        public const string SUM = "SUM";           
        public const string RESTA = "RESTA";       
        public const string MULTI = "MULTI";     
        public const string DIV = "DIV";          
        public const string IGUAL = "IGUAL";       
        public const string IGUALIGUAL = "IGUALIGUAL"; 
        public const string NOIGUAL = "NOIGUAL";    
        public const string MAYORQ = "MAYORQ";     
        public const string MENORQ = "MENORQ";   
        public const string MAYORIGUAL = "MAYORIGUAL";
        public const string MENORIGUAL = "MENORIGUAL"; 
        public const string PORCENTAJE = "PORCENTAJE";

        // Símbolos
        public const string PARENI = "PARENI";  
        public const string PAREND = "PAREND";  
        public const string COMA = "COMA";      

        public const string NEWLINE = "NEWLINE";    // salto de línea
        public const string INDENT = "INDENT";      // subir indent
        public const string DEDENT = "DEDENT";      // bajar indent

        // Final y error
        public const string FP = "FP";              // fin 
        public const string ERROR = "ERROR";        // errores léxicos
    }
}