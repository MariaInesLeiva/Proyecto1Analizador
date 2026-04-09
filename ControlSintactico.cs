using System.Collections.Generic;

namespace Proyecto1Analizador
{
    public class ControlSintactico
    {
         public List<string> errores; //lista para guardar los errores sintacticos
         public Token tokenActual; //guarda el token actual para que lo lea el parser
         public ControlSintactico()
        {
            errores = new List<string>();
            tokenActual = null;
        }

    }
}