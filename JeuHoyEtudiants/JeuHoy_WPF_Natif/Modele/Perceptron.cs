using JeuHoy_WPF_Natif.Vue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JeuHoy_WPF_Natif.Modele
{
    /// <summary>
    /// Classe contenant la logique du fonctionnement du perceptron
    /// </summary>
    public class Perceptron
    {
        //Variables globales
        private double[] _poidsSyn;
        private double learningRate = 0.1;
        private int epochs = 1000;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="inputSize"></param>
        public Perceptron(int inputSize)
        {
            Charger("test.txt", inputSize);

            if (_poidsSyn == null)
            {
                Random rand = new Random();
                _poidsSyn = new double[inputSize + 1];
                for (int i = 0; i < _poidsSyn.Length; i++)
                {
                    _poidsSyn[i] = rand.NextDouble() * 2 - 1;
                }
            }
        }

        /// <summary>
        /// Permet d'entrainer un perceptron
        /// </summary>
        /// <param name="trainingData"></param>
        public void Entrainement(Dictionary<int, List<double>> trainingData)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                foreach (var entry in trainingData)
                {
                    int target = entry.Key;
                    List<double> inputs = entry.Value;

                    List<double> inputWithBias = new List<double>(trainingData.Count);
                    inputWithBias.Insert(0, 1);

                    double sum = 0;
                    for (int i = 0; i < inputWithBias.Count; i++)
                    {
                        sum += _poidsSyn[i] * inputWithBias[i];
                    }

                    int output = sum > 0 ? 1 : -1;

                    if (output != target)
                    {
                        for (int i = 0; i < _poidsSyn.Length; i++)
                        {
                            _poidsSyn[i] += learningRate * (target - output) * inputWithBias[i];
                        }
                    }
                }
            }

            Sauvegarder("test.txt");
        }

        /// <summary>
        /// Permet de sauvegarder les statistiques d'apprentissage
        /// </summary>
        /// <param name="filePath"></param>
        public void Sauvegarder(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (double poid in _poidsSyn)
                {
                    writer.WriteLine(poid);
                }
            }
        }

        /// <summary>
        /// Permet de charger les statistiques d'apprentissage
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="inputSize"></param>
        public void Charger(string filePath, int inputSize)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length == inputSize + 1)
                {
                    for (int i = 0; i < _poidsSyn.Length; i++)
                    {
                        _poidsSyn[i] = double.Parse(lines[i]);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Permet de prédire le mouvement de l'utilisateur
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public int Prediction(List<double> input)
        {
            List<double> inputWithBias = new List<double>(input);
            inputWithBias.Insert(0, 1);

            double sum = 0;
            for (int i = 0; i < inputWithBias.Count; i++)
            {
                sum += _poidsSyn[i] * inputWithBias[i];
            }

            return sum > 0 ? 1 : -1;
        }
    }
}
