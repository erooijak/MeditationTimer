﻿using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Rooijakkers.MeditationTimer.Messages;
using Rooijakkers.MeditationTimer.Model;
using Rooijakkers.MeditationTimer.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Rooijakkers.MeditationTimer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Point _initialPoint; // Point used to store start position so a swipe can be recognized

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            Messenger.Default.Register<PlayMessage>(this, ReceivePlayMessage);
            Messenger.Default.Register<StartTimerMessage>(this, ReceiveStartTimerMessage);
            Messenger.Default.Register<StopTimerMessage>(this, ReceiveStopTimerMessage);
            Messenger.Default.Register<DisplaySitReadyMessage>(this, ReceiveSitReadyMessage);

            SwipingSurface.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            SwipingSurface.ManipulationStarted += SetInitialPosition;
            SwipingSurface.ManipulationCompleted += ToDiaryIfSwipedLeft;
        }

        public void SetInitialPosition(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _initialPoint = e.Position;
        }

        public void ToDiaryIfSwipedLeft(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var currentPoint = e.Position;
            if (_initialPoint.X - currentPoint.X >= Constants.SwipingTreshold)
            {
                NavigateToDiary();
            }
        }

        private void ReceivePlayMessage(PlayMessage msg)
        {
            switch (msg.BellSound)
            {
                case BellSound.Burmese:
                    BurmeseGongMediaElement.Play();
                    break;
                case BellSound.Cymbals:
                    CymbalsGongMediaElement.Play();
                    break;
                default:
                    throw new ArgumentException("BellSound not found.", nameof(msg));
            }
        }

        private void ReceiveStartTimerMessage(StartTimerMessage msg)
        {
            StartTimerButton.Visibility = Visibility.Collapsed;
            StopTimerButton.Visibility = Visibility.Visible;
            AddFiveMinutesButton.IsEnabled = false;
            ResetInitialTimeButton.IsEnabled = false;
        }

        private void ReceiveStopTimerMessage(StopTimerMessage msg)
        {
            StartTimerButton.Visibility = Visibility.Visible;
            StopTimerButton.Visibility = Visibility.Collapsed;
            AddFiveMinutesButton.IsEnabled = true;
            ResetInitialTimeButton.IsEnabled = true;
        }

        private void ReceiveSitReadyMessage(DisplaySitReadyMessage msg)
        {
            SitReadyTextBlock.Visibility = msg.Display ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Check if SplashScreenPage is on the backstack and remove so that the loading screen is only displayed once   
            if (Frame.BackStack.Count == 1)
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }
        }

        private void ViewHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDiary();
        }

        private void NavigateToDiary()
        {
            Frame.Navigate(typeof(MeditationDiaryPage));
        }
    }
}
