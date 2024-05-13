using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuHoy_WPF_Natif.Modele
{
    public interface IGestionPerceptron
    {
        void Sauvegarder();
        void Charger();
        void Entrainement();
    }
}
