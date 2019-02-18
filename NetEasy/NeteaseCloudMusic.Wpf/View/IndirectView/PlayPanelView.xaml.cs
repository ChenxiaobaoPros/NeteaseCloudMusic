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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NeteaseCloudMusic.Wpf.ViewModel.IndirectView;

namespace NeteaseCloudMusic.Wpf.View.IndirectView
{
    /// <summary>
    /// PlayPanelView.xaml 的交互逻辑
    /// </summary>
    public partial class PlayPanelView  
    {
        private readonly Services.AudioDecode.IAudioPlayableServices _audioPlayableServices;
        private DoubleAnimation _diskRotate;
        private DoubleAnimation _diskControlRotate;
        public PlayPanelView(ViewModel.IndirectView.PlayPanelViewModel viewModel,Services.AudioDecode.IAudioPlayableServices audioPlayableServices)
        {
            this.DataContext = viewModel;
            _audioPlayableServices = audioPlayableServices;
            InitializeComponent(); this.Loaded += PlayPanelView_Loaded;
        }

        

        private void PlayPanelView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this._diskRotate==null )
            {
                 _diskRotate = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(5)) { RepeatBehavior=RepeatBehavior.Forever};
                RotateTransform rotateTransform = new RotateTransform();
                
                this.grdImage.RenderTransform = rotateTransform;
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, _diskRotate);
            }
           
        }

        private PlayPanelViewModel ViewModel => this.DataContext as PlayPanelViewModel;
        private void ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshLyric();
        }

        /// <summary>
        /// 刷新歌词的同时可以获取到播放状态
        /// </summary>
        private async void RefreshLyric(  )
        {
            while (true )
            {
                if (_audioPlayableServices!=null &&(ViewModel?.Lryics?.Count).GetValueOrDefault()>0)
                {
                    var playTime = _audioPlayableServices.Position;
                    var lyrics = ViewModel.Lryics;
                  var minTime=  lyrics.Min(x => Math.Abs(x.Time.TotalMilliseconds - playTime.TotalMilliseconds));
                    if (minTime<=100)
                    {
                        var item = lyrics.First(x => Math.Abs(x.Time.TotalMilliseconds - playTime.TotalMilliseconds) == minTime);
                        this.lstLryics.SelectedItem = item;
                        lstLryics.ScrollIntoView(item);
                    }
                }
                SetDiskControl(_audioPlayableServices.PlayState == Services.AudioDecode.PlayState.Playing);
                await Task.Delay(100);
            }
        }
        private void SetDiskControl(bool isPlaying)
        {
            if (_diskControlRotate==null)
            {
                _diskControlRotate = new DoubleAnimation(0, -40, TimeSpan.FromSeconds(0.5));
                _diskControlRotate.FillBehavior = FillBehavior.HoldEnd;
                _diskControlRotate.EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut };
            }
           
            if (isPlaying)
            {
                var rt = new RotateTransform(0);
                imgDiskControl.RenderTransform = rt;
                // _diskControlRotate.
            }
            else
            {
                var rt = new RotateTransform(-40);
                imgDiskControl.RenderTransform = rt;
               // rt.BeginAnimation(RotateTransform.AngleProperty, _diskControlRotate);
            }
        }
        private void LrcButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = (e.Source as Button).Content;
            _audioPlayableServices.Position = item.Time;
        }
    }
    
}
