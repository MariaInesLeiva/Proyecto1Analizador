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
                tokenActual = new Token(TipoToken.FP, "EOF", 0, 0, 0);
                control. tokenActual = tokenActual;
                return
            }
        }
    }
}