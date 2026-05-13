using System.Collections.Generic;

namespace Proyecto1Analizador
{
    public class ControlSintactico
    {
        public List<string> errores;
        public Token? tokenActual;

        public ControlSintactico()
        {
            errores = new List<string>();
            tokenActual = null;
        }

        public void AgregarError(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                descripcion = "error sintáctico";
            }

            if (tokenActual == null)
            {
                errores.Add("Error sintáctico: " + descripcion);
                return;
            }

            string simbolo = tokenActual.Lexema;

            if (string.IsNullOrEmpty(simbolo))
            {
                simbolo = tokenActual.Tipo;
            }

            string mensaje = "Línea " + tokenActual.Linea +
                             ", columna " + tokenActual.ColumnaI +
                             ", cerca de '" + simbolo +
                             "': " + descripcion;

            if (!errores.Contains(mensaje))
            {
                errores.Add(mensaje);
            }
        }
    }
}