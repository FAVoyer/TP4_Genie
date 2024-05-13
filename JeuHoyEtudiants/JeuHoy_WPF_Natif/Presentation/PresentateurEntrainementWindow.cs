using JeuHoy_WPF;
using JeuHoy_WPF_Natif.Modele;
using JeuHoy_WPF_Natif.Vue;
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
    public class PresentateurEntrainementWindow : IPresentateur
    {
        private wEntrainement _vue;
        private GestionPerceptron _gestionPerceptron;

        public PresentateurEntrainementWindow(wEntrainement vue)
        {
            _vue = vue;
            _vue.Apprendre += Apprentissage;
            _gestionPerceptron = new GestionPerceptron();
        }

        public void Apprentissage(object sender, RoutedEventArgs e)
        {
            _gestionPerceptron.Entrainement();
        }
    }
}
