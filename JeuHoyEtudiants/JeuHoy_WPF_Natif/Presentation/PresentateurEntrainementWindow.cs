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
        //Variables globales
        private IEntrainement _vue;
        private GestionPerceptron _gestionPerceptron;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="vue"></param>
        public PresentateurEntrainementWindow(IEntrainement vue)
        {
            _vue = vue;
            _vue.Apprendre += Apprentissage;
            _gestionPerceptron = new GestionPerceptron();
        }

        /// <summary>
        /// Permet d'exécuter les méthodes d'apprentissages de la classe gestionPerceptron
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apprentissage(object sender, EventArgs e)
        {
            _gestionPerceptron.CollectTrainingData(_vue.aJoints, _vue.PositionEnCour);

            _gestionPerceptron.Entrainement();
            
            _vue.Console = _gestionPerceptron.Prediction(_vue.PositionEnCour).ToString();
        }
    }
}
