using System.Collections.Generic;

namespace Proyecto1Analizador
{
    public class ControlSintactico
    {
         public List<string> errores; //lista para guardar los errores sintacticos
         public Token? tokenActual; //guardamos el token actual para que lo lea el parser
         public ControlSintactico()
        {
            errores = new List<string>();
            tokenActual = null;
        }

        public void AgregarError(string descripcion) //registramos los errores sintácticos
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

            string mensaje = "Linea " + tokenActual.Linea +
                             ", columna " + tokenActual.ColumnaI +
                             ", simbolo '" + simbolo +
                             "', Error: " + descripcion;
            errores.Add(mensaje);
        }

    }
}