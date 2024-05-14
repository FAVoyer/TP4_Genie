using JeuHoy_WPF_Natif.Presentation;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JeuHoy_WPF_Natif.Modele
{
    /// <summary>
    /// Auteurs: Félix-Antoine Voyer & Félix Langlois
    /// Description: Classe contenant la logique du modèle de gestion de perceptron
    /// Date: 2024-05-12
    /// </summary>
    public class GestionPerceptron : IGestionPerceptron
    {
        //Variables locales
        Dictionary<int, List<double>> trainingData = new Dictionary<int, List<double>>();
        Dictionary<int, Perceptron> perceptrons = new Dictionary<int, Perceptron>();


        /// <summary>
        /// Permet d'entrainer le perceptron
        /// </summary>
        public void Entrainement()
        {
            foreach(var entry in trainingData)
            {
                int target = entry.Key;
                List<double> inputs = entry.Value;

                if(!perceptrons.ContainsKey(target))
                {
                    int inputSize = inputs.Count;
                    perceptrons[target] = new Perceptron(inputSize);
                }

                perceptrons[target].Entrainement(trainingData);
            }
        }

        /// <summary>
        /// Permet de prédire le mouvement de l'utilisateur
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public int Prediction(int target)
        {
            List<double> input = trainingData[target];

            if (perceptrons.ContainsKey(target))
            {
                return perceptrons[target].Prediction(input);
            }
            else
            {
                MessageBox.Show("Pas de perceptron entraîné pour le mouvement");
                return 0;
            }
        }

        /// <summary>
        /// Permet de mettre les positios des joints dans un dictionnaire
        /// </summary>
        /// <param name="joints"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public Dictionary<int, List<double>> CollectTrainingData(Joint[] joints, int position)
        {
            List<double> data = new List<double>();

            foreach (Joint joint in joints)
            {
                data.Add(joint.Position.X);
                data.Add(joint.Position.Y);
            }

            trainingData.Add(position, data);

            return trainingData;
        }
    }
}
