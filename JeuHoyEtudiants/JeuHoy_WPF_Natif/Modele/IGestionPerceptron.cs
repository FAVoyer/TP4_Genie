using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuHoy_WPF_Natif.Modele
{
    /// <summary>
    /// Interface contenant les définitions des méthodes pour la gestion des perceptrons
    /// </summary>
    public interface IGestionPerceptron
    {
        int Prediction(int target);
        void Entrainement();
        Dictionary<int, List<double>> CollectTrainingData(Joint[] joints, int position);
    }
}
