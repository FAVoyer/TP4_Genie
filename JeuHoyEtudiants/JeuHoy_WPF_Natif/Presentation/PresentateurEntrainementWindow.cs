using JeuHoy_WPF;
using JeuHoy_WPF_Natif.Modele;
using JeuHoy_WPF_Natif.Vue;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JeuHoy_WPF_Natif.Presentation
{
    /// <summary>
    /// Auteurs: Félix-Antoine Voyer & Félix Langlois
    /// Description: Classe contenant la logique du présentateur
    /// Date: 2024-05-12
    /// </summary>
    public class PresentateurEntrainementWindow
    {
        private IEntrainement _vue;
        private GestionPerceptron _gestionPerceptron;
        Dictionary<int, List<double>> trainingData = new Dictionary<int, List<double>>();

        public PresentateurEntrainementWindow(IEntrainement vue)
        {
            _vue = vue;
            _vue.Apprendre += Apprentissage;
            _gestionPerceptron = new GestionPerceptron();
        }

        private void Apprentissage(object sender, EventArgs e)
        {
            _gestionPerceptron.CollectTrainingData(_vue.aJoints, _vue.PositionEnCour);

            _gestionPerceptron.Entrainement(trainingData);
        }

        
    }
}
