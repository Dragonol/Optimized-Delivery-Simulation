﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Priority_Queue;

namespace Optimized_Delivery_Simulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly Random Random = new Random();
        public static readonly (int height, int width) MapSize = (30, 65);
        public static readonly int Space = 40;
        public static readonly int SplitChance = 3;
        public static readonly double AverageDistance = 6;
        public static readonly int Thickness = 30;

        public static WorldMap Map = new WorldMap(MapSize.height,MapSize.width);
        public static Dictionary<Point, Dictionary<Point, Trail>> LookupPath = new Dictionary<Point, Dictionary<Point, Trail>>();
        public static List<Point> OptimizedRoute = new List<Point>();
        public static Point Start;
        public static List<Point> Depots = new List<Point>();

        public static System.Windows.Point GridMargin = new System.Windows.Point(0, 0);
        public static System.Windows.Point BasePosition;
        public static System.Windows.Point CurrPosition;
        public static bool IsDragging = false;
        public static bool MouseLeftDown = false;

        public static double MapScale = 1;

        public MainWindow()
        {
            InitializeComponent();                                
            GenerateMap(Brushes.PowderBlue);
            DrawNode(Start, Brushes.AntiqueWhite, Thickness / 2);
        }

        private void MapSection_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsDragging && (Math.Abs(CurrPosition.X-BasePosition.X)+Math.Abs(CurrPosition.Y-BasePosition.Y))>0.2)
                return;

            var rawPoint = Mouse.GetPosition(MapSection);
            Point currMousePos = Map[(int)((rawPoint.Y + Space / 2) / Space), (int)((rawPoint.X + Space / 2) / Space)]?.Point;

            if ((object)currMousePos==null) 
                return;

            if (Depots.Contains(currMousePos))
                return;

            Console.WriteLine("add node");

            Depots.Add(currMousePos);
            DrawNode(currMousePos, Brushes.Blue, Thickness / 2);

            if (Depots.Count == 15)
            {
                CreateLookupDistances(Depots);
                CreateOptimizedRoute();
                for (int i = 0; i < OptimizedRoute.Count; i++)
                    DrawRoute(OptimizedRoute[i], OptimizedRoute[(i + 1) % OptimizedRoute.Count]);
            }

            //foreach (var node in Map.Nodes)
            //{
            //    Point run = node.Point;
            //    while (run != currMousePos)
            //    {
            //        Point pre = LookupPath[currMousePos][run].Previous;
            //        DrawPath(run, pre, Brushes.Blue, Thickness / 2);
            //        run = pre;
            //    }
            //}
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BasePosition = Mouse.GetPosition(MapSection);
            MouseLeftDown = true;
            Console.WriteLine("click " + BasePosition.X + " " + BasePosition.Y);
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(MouseLeftDown)
            {
                IsDragging = true;
                Console.WriteLine("moving");
                CurrPosition = Mouse.GetPosition(MapSection);
                double absLeft = CurrPosition.X - BasePosition.X;
                double absTop = CurrPosition.Y - BasePosition.Y;
                MapSection.Margin = new Thickness(MapSection.Margin.Left + absLeft, MapSection.Margin.Top + absTop, 0, 0);
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false;
            MouseLeftDown = false;
            Console.WriteLine("release");
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var currMousePos = Mouse.GetPosition(MapSectionCover);
            Console.WriteLine(currMousePos.X + " " + currMousePos.Y);
            double scale = 0.03;
            if (e.Delta < 0)
                scale *= -1;
            MapScale += scale;
            MapSectionCover.LayoutTransform = 
                new ScaleTransform(MapScale,MapScale,currMousePos.X,currMousePos.Y);
        }
    }
}
