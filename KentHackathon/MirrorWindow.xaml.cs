//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace KentHackathon
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System;
    using System.Diagnostics;
    using System.Windows.Media.Imaging;
    using System.Net;
    using Newtonsoft.Json.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using System.Windows.Controls;
    using Microsoft.Kinect.Toolkit.Controls;
    using System.Collections.Generic;/// <summary>
                                     /// Interaction logic for MainWindow.xaml
                                     /// </summary>
    public partial class MirrorWindow : Window
    {
        private IList<Article> ArticleList = new List<Article>();

        private BitmapImage shirt;
        private Catalog catalog;

        private Point boxTopRight = new Point();
        private Point boxTopLeft = new Point();
        private Point boxBotRight = new Point();
        private Point boxBotLeft = new Point();



        private const double ScaleX = 2.846;
        private const double OffsetX = -596.592;
        private const double ScaleX2 = 1.2;

        private const double ScaleY = 2.846 * 1.3;
        private const double OffsetY = -475;
        private const double depthScale = 1960;
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 768.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 1366.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        /// 

        public MirrorWindow()
        {

            InitializeComponent();
            string domain = "https://stoh.io/recScript.php?";
            string getParam = "get_suggests";
            // can only be get_suggests or recalculate
            DataController myController = new DataController();
            string responseUrl = myController.MakeRequest(domain + getParam);
            IList<SearchResult> myList = myController.parseJson();
            foreach (SearchResult searchResult in myList)
            {
                ArticleList.Add(new Article(searchResult.id, searchResult.url));
            }
            Debug.Print(ArticleList.Count.ToString());

            catalog = new Catalog(ArticleList);


        }

        private void ParseJSON(WebResponse response)
        {
            //token = JObject.Parse(response.ToString());
            Debug.Print(response.ToString());
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            //Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                //this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);
                        setBoundingBox(skel);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);
            //Debug.Print(skeleton.Joints[JointType.HipCenter].Position.Z.ToString());

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
            if (shirt != null) {
                ImageSourceConverter c = new ImageSourceConverter();
                ImageSource image = (ImageSource)c.ConvertFrom(shirt);
                drawingContext.DrawImage(image, new Rect(boxTopLeft, boxBotRight));
            }
            drawingContext.DrawRectangle(null, this.trackedBonePen, new Rect(boxTopLeft, boxBotRight));
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            double x = (depthPoint.X * ScaleX + OffsetX) * ScaleX2;
            double y = depthPoint.Y * ScaleY * (Math.Pow((depthScale / depthPoint.Depth), .000000000000000001)) + OffsetY;
            return new Point(x, y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }
            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        /// <summary>
        /// Handles the checking or unchecking of the seated mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != this.sensor)
            {
            }
        }

        private void setBoundingBox(Skeleton skeleton)
        {
            DepthImagePoint shoulderLeftDepthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeleton.Joints[JointType.ShoulderLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint shoulderRightDepthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeleton.Joints[JointType.ShoulderRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint shoulderCenterDepthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeleton.Joints[JointType.ShoulderCenter].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint HipLeftDepthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeleton.Joints[JointType.HipLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint HipRightDepthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeleton.Joints[JointType.HipRight].Position, DepthImageFormat.Resolution640x480Fps30);


            boxTopLeft.X = (shoulderLeftDepthPoint.X * ScaleX + OffsetX) * ScaleX2;
            boxTopLeft.Y = shoulderCenterDepthPoint.Y * ScaleY * (Math.Pow((depthScale / shoulderCenterDepthPoint.Depth), .000000000000000001)) + OffsetY;

            boxTopRight.X = (shoulderRightDepthPoint.X * ScaleX + OffsetX) * ScaleX2;
            boxTopRight.Y = boxTopLeft.Y;

            boxBotLeft.X = boxTopLeft.X;
            boxBotLeft.Y = ((HipLeftDepthPoint.Y * ScaleY * (Math.Pow((depthScale / HipLeftDepthPoint.Depth), .000000000000000001)) + OffsetY) + (HipRightDepthPoint.Y * ScaleY * (Math.Pow((depthScale / HipRightDepthPoint.Depth), .000000000000000001)) + OffsetY)) / 2;

            boxBotRight.X = boxTopRight.X;
            boxBotRight.Y = boxBotLeft.Y;

        }


        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            KinectTileButton tB = (KinectTileButton)e.OriginalSource;
            if (tB.Name != "Cat_Btn")
            {
                String id = tB.Name.Substring(3);
                foreach (Article a in ArticleList)
                {
                    if (a.id == id)
                    {
                        shirt = a.bitmapImage;
                    }
                }


                layoutGrid.Children.Remove(catalog);
            }

        }

        private void Get_Url_Image()
        {

        }

        private void Catalog_On_Click(object sender, RoutedEventArgs e)
        {

            layoutGrid.Children.Add(catalog);
            Grid.SetRowSpan(catalog, 2);



        }
    }
}