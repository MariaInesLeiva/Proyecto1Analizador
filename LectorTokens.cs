using System.Collections.Generic;

namespace Proyecto1Analizador
{
    public class LectorTokens
    {
        private List<Token> listaTokens;
        private ControlSintactico control;
        private int posicion;
        public Token tokenActual
        {
            get;
            private set;
        }

        public LectorTokens(List<Token> tokensEntrada, ControlSintactico controlEntrada)
        {
            listaTokens = tokensEntrada;
            control = controlEntrada;
            posicion = 0;
        }

        public int yylex()
        {
            if (posicion >= listaTokens.Count)
            {   
                // Si ya no hay más tokens → EOF
                tokenActual = new Token(TipoToken.FP, "EOF", 0, 0, 0);
                control. tokenActual = tokenActual;
                return (int)listaTokens.FP;
            }

            tokenActual = listaTokens[posicion];
            posicion++;

            // Actualizamos el token actual para los errores
            control.tokenActual = tokenActual;

            switch (tokenActual.Tipo)
            {
                // Tipos
                case TipoToken.PRINT: return (int)Tokens.PRINT;
                case TipoToken.PRFLOAT: return (int)Tokens.PRFLOAT;
                case TipoToken.PRCHAR: return (int)Tokens.PRCHAR;
                case TipoToken.PRBOOL: return (int)Tokens.PRBOOL;

                // Control
                case TipoToken.PRIF: return (int)Tokens.PRIF;
                case TipoToken.PRELIF: return (int)Tokens.PRELIF;
                case TipoToken.PRELSE: return (int)Tokens.PRELSE;
                case TipoToken.PRWHILE: return (int)Tokens.PRWHILE;
                case TipoToken.PRDEF: return (int)Tokens.PRDEF;
                case TipoToken.PRRETURN: return (int)Tokens.PRRETURN;

                // Entrada / salida
                case TipoToken.PRREAD: return (int)Tokens.PRREAD;
                case TipoToken.PRWRITE: return (int)Tokens.PRWRITE;

                // Lógicos
                case TipoToken.OR: return (int)Tokens.OR;
                case TipoToken.AND: return (int)Tokens.AND;
                case TipoToken.NOT: return (int)Tokens.NOT;

                // Datos
                case TipoToken.INT: return (int)Tokens.INT;
                case TipoToken.FLOAT: return (int)Tokens.FLOAT;
                case TipoToken.CHAR: return (int)Tokens.CHAR;
                case TipoToken.BOOL: return (int)Tokens.BOOL;
                case TipoToken.ID: return (int)Tokens.ID;

                // Operadores
                case TipoToken.SUM: return (int)Tokens.SUM;
                case TipoToken.RESTA: return (int)Tokens.RESTA;
                case TipoToken.MULTI: return (int)Tokens.MULTI;
                case TipoToken.DIV: return (int)Tokens.DIV;
                case TipoToken.PORCENTAJE: return (int)Tokens.PORCENTAJE;

                case TipoToken.IGUAL: return (int)Tokens.IGUAL;
                case TipoToken.IGUALIGUAL: return (int)Tokens.IGUALIGUAL;
                case TipoToken.NOIGUAL: return (int)Tokens.NOIGUAL;
                case TipoToken.MAYORQ: return (int)Tokens.MAYORQ;
                case TipoToken.MENORQ: return (int)Tokens.MENORQ;
                case TipoToken.MAYORIGUAL: return (int)Tokens.MAYORIGUAL;
                case TipoToken.MENORIGUAL: return (int)Tokens.MENORIGUAL;

                // Símbolos
                case TipoToken.PARENI: return (int)Tokens.PARENI;
                case TipoToken.PAREND: return (int)Tokens.PAREND;
                case TipoToken.COMA: return (int)Tokens.COMA;

                // Estructura
                case TipoToken.NEWLINE: return (int)Tokens.NEWLINE;
                case TipoToken.INDENT: return (int)Tokens.INDENT;
                case TipoToken.DEDENT: return (int)Tokens.DEDENT;

                // Especiales
                case TipoToken.FP: return (int)Tokens.FP;
                case TipoToken.ERROR: return (int)Tokens.ERROR;

                default:
                    control.AgregarError("token no reconocido por el parser");
                    return (int)Tokens.ERROR;
            }

        }
    }
}