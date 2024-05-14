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

        public Perceptron(int inputSize)
        {
            Random rand = new Random();
            _poidsSyn = new double[inputSize + 1];
            for (int i = 0; i < _poidsSyn.Length; i++)
            {
                _poidsSyn[i] = rand.NextDouble() * 2 - 1;
            }
        }

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
        }

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

        public void Charger(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length == _poidsSyn.Length)
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
