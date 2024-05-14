using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JeuHoy_WPF_Natif.Presentation
{
    /// <summary>
    /// Interface contenant les définitions des méthodes pour le présentateur
    /// </summary>
    public interface IPresentateur
    {
        void Apprentissage(object sender, RoutedEventArgs e);
    }
}
