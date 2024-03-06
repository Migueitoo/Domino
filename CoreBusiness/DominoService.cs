using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModels;

namespace CoreBusiness
{
    public class DominoService
    {
        public List<Fichas> VerificarFichas(List<Fichas> fichas)
        {
            // Lista que se devuelve si se organizan los números
            var fichasOrdenadas = new List<Fichas>();

            //La primera ficha es el punto de partida
            var primeraFicha = fichas[0];
            //Variable con lado B de la última ficha agregada a la lista
            var ladoActual = primeraFicha.LadoB;
            fichasOrdenadas.Add(primeraFicha);

            int i = 1;
            bool fichaValidada = false;

            while (i < fichas.Count || !fichaValidada)
            {
                if (i >= fichas.Count)
                {
                    //Fin del ciclo
                    break;
                }

                //Ficha de comparación con la primera ficha
                var fichaActual = fichas[i];

                if (fichaActual.LadoA == ladoActual)
                {
                    // Si el lado A coincide, se agrega ficha actual y se actualiza el lado actual
                    fichasOrdenadas.Add(fichaActual);
                    ladoActual = fichaActual.LadoB;
                    fichas.RemoveAt(i);
                    fichaValidada = true; // Indicamos que hemos validado una ficha
                }
                else if (fichaActual.LadoB == ladoActual)
                {
                    // Si el lado B coincide, invertir los lados y agregar la ficha
                    fichasOrdenadas.Add(new Fichas { LadoA = fichaActual.LadoB, LadoB = fichaActual.LadoA });
                    ladoActual = fichaActual.LadoA;
                    fichas.RemoveAt(i);
                    fichaValidada = true; // Indicamos que hemos validado una ficha
                }
                else
                {
                    // Si ninguno de los lados coincide
                    i++;
                }

                // Si se agrega una ficha a la lista, se reinicia el ciclo
                if (fichaValidada)
                {
                    i = 1;
                    ladoActual = fichasOrdenadas.Last().LadoB; // Actualizamos el lado actual al lado B de la última ficha agregada
                    fichaValidada = false; // Reiniciamos la bandera de validación de ficha
                }
            }

            // Verificar si el último lado coincide con el lado inicial
            if (ladoActual == primeraFicha.LadoA)
            {
                return fichasOrdenadas;
            }

            return null;
        }
    }
}
