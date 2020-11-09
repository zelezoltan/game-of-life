using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using GameOfLife.Model;
using GameOfLife.Persistence;
using GameOfLife.ViewModel;
using Microsoft.Win32;

namespace GameOfLife.View.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Model.Model _model;
        private DispatcherTimer _timer;
        private MainWindow _view;
        private ViewModel.ViewModel _viewModel;
        private Persistence.Persistence _persistence;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(Object sender, StartupEventArgs e)
        {
            _persistence = new Persistence.Persistence();

            _model = new Model.Model(_persistence);

            _viewModel = new ViewModel.ViewModel(_model);
            _viewModel.LoadConfiguration += new EventHandler(ViewModel_LoadConfiguration);
            _viewModel.Play += new EventHandler(ViewModel_Play);
            _viewModel.Step += new EventHandler(ViewModel_Step);
            _viewModel.Pause += new EventHandler(ViewModel_Pause);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.1);
            _timer.Tick += new EventHandler(TimerTick);
        }

        private void TimerTick(Object sender, EventArgs e)
        {
            _model.Step();
        }

        private void ViewModel_Play(Object sender, EventArgs e)
        {
            _model.TogglePlay();
            _timer.Start();
        }

        private void ViewModel_Step(Object sender, EventArgs e)
        {
            _model.Step();
        }

        private void ViewModel_Pause(Object sender, EventArgs e)
        {
            _model.TogglePlay();
            _timer.Stop();
        }

        private async void ViewModel_LoadConfiguration(Object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Load Configuration";
                openFileDialog.Filter = "Life files|*.lif";
                if (openFileDialog.ShowDialog() == true)
                {
                    await _model.LoadConfiguration(openFileDialog.FileName);
                }
            } catch(Exception)
            {
                MessageBox.Show("Error while loading the configuration file!", "Game of Life", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
