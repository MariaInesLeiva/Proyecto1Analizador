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

        public void AgregarError(string descripcion) //registra los errores sintácticos
        {
            if (tokenActual == null)
            {
                errores.Add("Error: " + descripcion);
                return;
            }
            string simbolo = tokenActual.Lexema;
            if (string.IsNullOrEmpty(simbolo))
            {
                simbolo = tokenActual.Tipo; 
            }
        }

    }
}