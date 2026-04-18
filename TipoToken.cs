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

        // Estructuras de control
        public const string PRIF = "PRIF";
        public const string PRELIF = "PRELIF";
        public const string PRELSE = "PRELSE";
        public const string PRWHILE = "PRWHILE";
        public const string PRDEF = "PRDEF";
        public const string PRRETURN = "PRRETURN";

        // Entrada y salida
        public const string PRREAD = "PRREAD";
        public const string PRWRITE = "PRWRITE";

        // Valores y variables
        public const string INT = "INT";
        public const string FLOAT = "FLOAT";
        public const string CHAR = "CHAR";
        public const string BOOL = "BOOL";
        public const string ID = "ID";

        // Operadores lógicos
        public const string OR = "OR";
        public const string AND = "AND";
        public const string NOT = "NOT";

        // Operadores aritméticos y relacionales
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

        // Estructura de indentación
        public const string NEWLINE = "NEWLINE";
        public const string INDENT = "INDENT";
        public const string DEDENT = "DEDENT";

        // Final y error
        public const string FP = "FP";
        public const string ERROR = "ERROR";
    }
}