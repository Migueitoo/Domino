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
            int fichasTotal = fichas.Count;
            var listGlobal = fichas.ToList();
            int countIntentos = 0;
            //La primera ficha es el punto de partida
          
            bool coincidio = false;
            bool giroFicha1 = false;
            int primeraLadoA = 0;

            bool procesoExitoso = false;
           
            do
            {
                var primeraFicha = fichas[0];
                //Variable con lado B de la última ficha agregada a la lista
                var ladoActual = primeraFicha.LadoB;

                for (int j = 1; j < fichas.Count; j++)
                {
                    var fichaComparacion = fichas[j];

                    if (ladoActual == fichaComparacion.LadoA || ladoActual == fichaComparacion.LadoB)
                    {
                        fichasOrdenadas.Add(primeraFicha);
                        coincidio = true;
                        break;
                    }
                    else if (primeraFicha.LadoA == fichaComparacion.LadoA || primeraFicha.LadoA == fichaComparacion.LadoB)
                    {
                        fichasOrdenadas.Add(new Fichas { LadoA = primeraFicha.LadoB, LadoB = primeraFicha.LadoA });
                        giroFicha1 = true;
                        coincidio = true;
                        ladoActual = primeraFicha.LadoA;
                        break;
                    }
                }

                //si la primera ficha no encuentra coincidencias con el resto no se puede armar la cadena
                if (!coincidio) return null;


                int i = 1;
                bool fichaValidada = false;

                while (i < fichas.Count || !fichaValidada) // 6 1 - 5 1 - 4 1 - 3 1 
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
                if (giroFicha1) primeraLadoA = primeraFicha.LadoB;
                else primeraLadoA = primeraFicha.LadoA;

                //Si organiza exitosamente las fichas
                if (ladoActual == primeraLadoA && fichasOrdenadas.Count() == fichasTotal)
                {
                    procesoExitoso = true;               
                }
                //Si no logra continuar organizando, envía la primera ficha al final
                else
                {
                    countIntentos++;
                    Fichas primerElemento = listGlobal[0];
                    listGlobal.RemoveAt(0);
                    listGlobal.Add(primerElemento);
                    fichas = listGlobal.ToList();
                    fichasOrdenadas = new List<Fichas>();
                    if (countIntentos == fichasTotal)
                    {
                        break;
                    }                  
                }

            } while (!procesoExitoso );

            if (procesoExitoso) return fichasOrdenadas;
            else return null;
        }
    }
}
