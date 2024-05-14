using JeuHoy_WPF_Natif.Presentation;
using JeuHoy_WPF_Natif.Vue;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Speech.Recognition;



namespace JeuHoy_WPF
{
    /// <summary>
    /// Enumeration des types d'affichage de la kinect
    /// </summary>
    public enum DisplayFrameType
    {
        Infrared,
        Color,
        Depth
    }

    /// <summary>
    /// Auteur:      Hugo St-Louis
    /// Description: Permet de faire l'entrainement des différentes figures de danse.
    /// Date:        2023-04-17
    /// </summary>
    public partial class wEntrainement : Window, IEntrainement
    {
        #region Constants
        private const DisplayFrameType DEFAULT_DISPLAYFRAMETYPE = DisplayFrameType.Color;
        public static readonly double DPI = 96.0;
        public static readonly PixelFormat FORMAT = PixelFormats.Bgra32;
        SpeechRecognitionEngine _recognizer;

        #endregion

        #region Champs
        private Dictionary<string, BitmapImage> _dicImgFigure = new Dictionary<string, BitmapImage>();
        private JouerSon _son = new JouerSon();
        private int _positionEnCours = 1;

        private DisplayFrameType _currentDisplayFrameType;
        private KinectSensor _kinectSensor = null;
        private WriteableBitmap _bitmap = null;

        private ushort[] _picFrameData = null;
        private byte[] _picPixels = null;

        private PresentateurEntrainementWindow _presentateur;
        #endregion

        #region Events
        public event RoutedEventHandler ChangerFigure;
        public event EventHandler Apprendre;
        #endregion

        #region Propriete
        public Joint[] aJoints { get; set; }

        public string Console { set { txtConsole.Text = value; } }


        public int PositionEnCour { get { return _positionEnCours; } }

        #endregion

        /// <summary>
        /// Constructeur
        /// </summary>
        public wEntrainement()
        {
            InitializeComponent();
            _kinectSensor = KinectSensor.GetDefault();
            _presentateur = new PresentateurEntrainementWindow(this);
            _recognizer = new SpeechRecognitionEngine();


            if (_kinectSensor != null)
            {
                _kinectSensor.Open();
                _kinectSensor.IsAvailableChanged += IsAvailableChanged;
                SetupCurrentDisplay(DEFAULT_DISPLAYFRAMETYPE);

                MultiSourceFrameReader multi = _kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Infrared | FrameSourceTypes.Depth);

                multi.MultiSourceFrameArrived += Multi_MultiSourceFrameArrived;
                picKinect.Source = _bitmap;

                BodyFrameReader bodyframe = _kinectSensor.BodyFrameSource.OpenReader();
                bodyframe.FrameArrived += Bodyframe_FrameArrived;
            }

            for (int i = 1; i <= CstApplication.NBFIGURE; i++)
            {
                Uri uriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"./HoyContent/fig" + i + ".png", UriKind.Absolute);
                _dicImgFigure.Add("fig" + i, new BitmapImage(uriSource));
            }

            lblNbPositions.Content = "/ " + CstApplication.NBFIGURE.ToString();
            ChargerFigure();
            _son.JouerSonAsync(@"./HoyContent/hoy.wav");

            Choices commands = new Choices();
            commands.Add("HOY!");

            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(commands);

            Grammar grammar = new Grammar(gb);

            _recognizer.LoadGrammar(grammar);

            // Écouter les résultats de la reconnaissance vocale
            _recognizer.SpeechRecognized += Recognizer_SpeechRecognized;

            // Démarrer la reconnaissance vocale
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// Permet de reconnaitre la voix lorsque l'on crie HOY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "HOY!")
            {
                _positionEnCours++;
                ChargerFigure();
            }
        }

        /// <summary>
        /// Permet d'aller chercher les positions des joints dans l'écran
        /// </summary>
        /// <param name="body"></param>
        public void GetJoitsPos(Body body)
        {
            aJoints = body.Joints.Values.ToArray();
        }

        /// <summary>
        /// Permet de dessiner le squelette de l'utilisateur
        /// Date: 2024-05-12
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Bodyframe_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            pDessinSquelette.Children.Clear();
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    Body[] bodies = new Body[bodyFrame.BodyCount];

                    //Remplir le vecteur avec les squelettes
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    foreach (Body body in bodies.Where(b => b.IsTracked))
                    {
                        if (body != null)
                        {
                            GetJoitsPos(body);
                            DessinerSquelette(body, _kinectSensor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Permet de déterminer le mode d'affichage de la kinect
        /// Date: 2024-05-12
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Multi_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {

            MultiSourceFrame sourceFrame = e.FrameReference.AcquireFrame();
            if (sourceFrame == null)
                return;
            switch (_currentDisplayFrameType)
            {
                case DisplayFrameType.Color:
                    using (ColorFrame colorFrame = sourceFrame.ColorFrameReference.AcquireFrame())
                    {
                        if (colorFrame != null)
                            ShowColorFrame(colorFrame);
                    }
                    break;
            }
        }

        /// <summary>
        /// Permet de créer un frame en couleur
        /// Date: 2024-05-12
        /// </summary>
        /// <param name="colorFrame"></param>
        public void ShowColorFrame(ColorFrame colorFrame)
        {

            if (colorFrame != null)
            {
                FrameDescription frameDescription = colorFrame.FrameDescription;
                if (_bitmap == null)
                    _bitmap = new WriteableBitmap(frameDescription.Width, frameDescription.Height, DPI, DPI, FORMAT, null);

                colorFrame.CopyConvertedFrameDataToArray(_picPixels, ColorImageFormat.Bgra);
                RenderPixelArray(_picPixels, frameDescription);
            }
        }

        /// <summary>
        /// Permet de render le pixelArray en image
        /// Date: 2024-05-12
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="currentFrameDescription"></param>
        public void RenderPixelArray(byte[] pixels, FrameDescription currentFrameDescription)
        {
            _bitmap.Lock();
            _bitmap.WritePixels(new Int32Rect(0, 0, currentFrameDescription.Width, currentFrameDescription.Height), pixels, currentFrameDescription.Width * 4, 0);
            _bitmap.Unlock();
            picKinect.Source = _bitmap;
        }

        /// <summary>
        /// Permet de choisir la méthode d'affichage de la kinect
        /// Date: 2024-05-12
        /// </summary>
        /// <param name="newDisplayFrameType"></param>
        public void SetupCurrentDisplay(DisplayFrameType newDisplayFrameType)
        {
            _currentDisplayFrameType = newDisplayFrameType;
            FrameDescription frameDescription;

            switch (_currentDisplayFrameType)
            {
                case DisplayFrameType.Color:
                    frameDescription = _kinectSensor.ColorFrameSource.FrameDescription;
                    _picPixels = new byte[frameDescription.Width * frameDescription.Height * 4];
                    _picFrameData = new ushort[frameDescription.Width * frameDescription.Height];
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Permet de changer le titre lorsque la kinect est connectée
        /// Date: 2024-05-12
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            this.Title = "Kinect 2.0 " + (_kinectSensor.IsAvailable ? "connectée" : "Non connectée");
        }

        /// <summary>
        /// Dessine un ellipse pour chacune des jointure du squelette détecté.
        /// </summary>
        /// <param name="joueur">Le joueur détecté</param>
        /// <param name="sensor">Le sensor Kinect</param>
        public void DessinerSquelette(Body body, KinectSensor sensor)
        {
            try
            {
                if (body != null)
                {
                    Joint[] joints = body.Joints.Values.ToArray();
                    for (int i = 0; i < joints.Count(); i++)
                        DrawJoint(sensor, joints[i], CstApplication.BODY_ELLIPSE_SIZE, pDessinSquelette);
                }
            }
            catch (Exception ex)
            {
                txtConsole.Text = ex.Message;
            }
        }


        /// <summary>
        /// Dessine le joint d'un squellete d'un senseur Kinect sur le canvas passé en paramètre
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="joint"></param>
        /// <param name="size"></param>
        /// <param name="canvas"></param>
        public void DrawJoint(KinectSensor sensor, Joint joint, int size, Canvas canvas)
        {
            if (joint.Position.X != 0 && joint.Position.Y != 0 && joint.Position.Z != 0)
            {
                // Convertir la position du joint en coordonnées d'écran
                System.Windows.Point point = GetPoint(sensor, joint.Position, canvas.Height, canvas.Width);

                // Créer un cercle à la position du joint
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = new SolidColorBrush(Colors.Green);
                ellipse.Width = size;
                ellipse.Height = size;

                // Positionner le cercle sur l'élément de dessin Canvas
                Canvas.SetLeft(ellipse, point.X - size / 2);
                Canvas.SetTop(ellipse, point.Y - size / 2);

                // Ajouter le cercle à l'élément de dessin Canvas
                canvas.Children.Add(ellipse);
            }
        }

        /// <summary>
        /// Retourne le point x,y d'un joint par rapport à la taille d'un canvas. 
        /// J'ai permis de dépasser le canvas car je trouvais ça drole :-)
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="position"></param>
        /// <param name="iCanvasHeight"></param>
        /// <param name="iCanvasWidth"></param>
        /// <returns></returns>
        public System.Windows.Point GetPoint(KinectSensor sensor, CameraSpacePoint position, double iCanvasHeight, double iCanvasWidth)
        {
            System.Windows.Point point = new System.Windows.Point();

            DepthSpacePoint depthPoint = sensor.CoordinateMapper.MapCameraPointToDepthSpace(position);
            point.X = float.IsInfinity(depthPoint.X) ? 0.0 : depthPoint.X;
            point.Y = float.IsInfinity(depthPoint.Y) ? 0.0 : depthPoint.Y;

            // La Kinect pour Xbox One utilise également le SDK 2 de Microsoft, et sa résolution de profondeur est de 512x424 pixels.
            //// Ainsi, la résolution de la carte de profondeur pour la Kinect pour Xbox One est également de 512x424 pixels.
            point.X = point.X / 512 * iCanvasHeight;
            point.Y = point.Y / 424 * iCanvasWidth;

            return point;
        }


        /// <summary>
        /// Charger la figure de danse en cours.
        /// </summary>
        public void ChargerFigure()
        {
            BitmapImage imgValue;
            bool bResultat;

            if (_positionEnCours > CstApplication.NBFIGURE)
                _positionEnCours = 1;

            if (_positionEnCours < 1)
                _positionEnCours = CstApplication.NBFIGURE;

            lblFigureEnCours.Content = _positionEnCours.ToString();

            bResultat = _dicImgFigure.TryGetValue("fig" + _positionEnCours, out imgValue);
            if (bResultat == true)
                picPositionAFaire.Source = imgValue;

        }


        /// <summary>
        /// Fermeture de la fenêtre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void picRetour_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Change le curseur lorsque le curseur est sur l'image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void picRetour_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Change le curseur lorsque le curseur est sur l'image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void picRetour_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
        /// <summary>
        /// Lorsqu'on appuie sur le bouton suivant ou précédent, modifier la figure en conséquence.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnClickChangerFigure_Click(object sender, RoutedEventArgs e)
        {
            Control bouton = (Control)sender;

            if (bouton.Name == "btnSuivant")
                _positionEnCours++;
            else if (bouton.Name == "btnPrecedent")
                _positionEnCours--;

            ChargerFigure();
        }


        /// <summary>
        /// Apprentissage avec la position obtenu à partir de la Kinect versus l'image affichée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnApprendre_Click(object sender, RoutedEventArgs e)
        {
            Apprendre(this, EventArgs.Empty);
        }
    }
}
