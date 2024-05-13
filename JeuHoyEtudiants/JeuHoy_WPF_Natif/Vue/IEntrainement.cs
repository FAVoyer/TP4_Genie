using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JeuHoy_WPF_Natif.Vue
{
    public interface IEntrainement
    {
        // Méthode pour charger la figure de danse en cours
        void ChargerFigure();

        // Méthode pour dessiner le squelette de l'utilisateur
        void DessinerSquelette(Body body, KinectSensor sensor);

        // Méthode pour afficher un cadre de couleur
        void ShowColorFrame(ColorFrame colorFrame);

        // Méthode pour afficher un pixelArray en image
        void RenderPixelArray(byte[] pixels, FrameDescription currentFrameDescription);

        // Méthode pour changer le titre lorsque la Kinect est connectée
        void IsAvailableChanged(object sender, IsAvailableChangedEventArgs e);

        // Méthode pour fermer la fenêtre
        void picRetour_Click(object sender, EventArgs e);

        // Méthode pour changer le curseur lorsque le curseur est sur l'image
        void picRetour_MouseHover(object sender, EventArgs e);

        // Méthode pour changer le curseur lorsque le curseur est sur l'image
        void picRetour_MouseLeave(object sender, EventArgs e);

        // Méthode pour gérer le clic sur le bouton suivant ou précédent
        event RoutedEventHandler ChangerFigure;

        // Méthode pour apprendre avec la position obtenue à partir de la Kinect versus l'image affichée
        event RoutedEventHandler Apprendre;
    }
}
