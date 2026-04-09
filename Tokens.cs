namespace Proyecto1Analizador
{
    public enum Tokens
    {
        PRINT = 257,    
        PRFLOAT,  
        PRCHAR, 
        PRBOOL,

        // Funciones
        PRIF,  
        PRELIF,
        PRELSE,
        PRWHILE,  
        PRDEF,      
        PRRETURN,
        // Entrada y salida
        PRREAD,   
        PRWRITE,

        // Varoles y variables
        INT,          
        FLOAT,    
        CHAR,
        BOOL,        
        ID,             

        // Operadores or/and/not 
        OR,
        AND,
        NOT,
        
        // Operadores
        SUM,         
        RESTA,      
        MULTI,   
        DIV,       
        IGUAL,    
        IGUALIGUAL,
        NOIGUAL,
        MAYORQ,   
        MENORQ,
        MAYORIGUAL,
        MENORIGUAL,
        PORCENTAJE,

        // Símbolos
        PARENI,
        PAREND,
        COMA ,   

        NEWLINE,   // salto de línea
        INDENT,    // subir indent
        DEDENT,   // bajar indent

        // Final y error
        FP,           // fin 
        ERROR        // errores léxicos
    }
    
}