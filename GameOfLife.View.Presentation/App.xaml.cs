using System;
using System.Windows.Threading;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Input;

namespace GameOfLife.View.Presentation
{
    /// <summary>
    /// Game of Life application class
    /// </summary>
    public partial class App : Application
    {
        #region Fields
        private Model.Model _model;
        private DispatcherTimer _timer;
        private MainWindow _view;
        private NewPatternWindow _newPatternWindow;
        private ViewModel.ViewModel _viewModel;
        private Persistence.Persistence _persistence;
        #endregion

        #region Constructor
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }
        #endregion

        #region Application Event Handlers
        private void App_Startup(Object sender, StartupEventArgs e)
        {
            // Create the persistence
            _persistence = new Persistence.Persistence();

            // Create the model and inject the persistence
            _model = new Model.Model(_persistence);

            // Create the view model and inject the model
            _viewModel = new ViewModel.ViewModel(_model);
            _viewModel.LoadConfiguration += new EventHandler(ViewModel_LoadConfiguration);
            _viewModel.Play += new EventHandler(ViewModel_Play);
            _viewModel.Step += new EventHandler(ViewModel_Step);
            //_viewModel.Pause += new EventHandler(ViewModel_Pause);
            _viewModel.CanvasClick += new EventHandler(ViewModel_CanvasClicked);
            _viewModel.OpenNewPatternWindow += new EventHandler(ViewModel_OpenNewPatternWindow);
            _viewModel.CreatePattern += new EventHandler<ViewModel.NewPatternEventArgs>(ViewModel_CreatePattern);

            // Create the view
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();

            // Create the timer
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.1);
            _timer.Tick += new EventHandler(TimerTick);
        }

        private void TimerTick(Object sender, EventArgs e)
        {
            _model.Step();
        }
        #endregion

        #region ViewModel Event Handlers
        private void ViewModel_OpenNewPatternWindow(Object sender, EventArgs e)
        {
            if (this._newPatternWindow == null)
            {
                this._newPatternWindow = new NewPatternWindow();
                this._newPatternWindow.DataContext = _viewModel;
            }
            this._newPatternWindow.ShowDialog();
        }

        private void ViewModel_CanvasClicked(Object sender, EventArgs e)
        {
            Point p = Mouse.GetPosition(Mouse.DirectlyOver);
            int y = (int)(p.X / _viewModel.CellSizeY);
            int x = (int)(p.Y / _viewModel.CellSizeX);
            _model.ChangeCell(x, y);
            //Mouse.LeftButton == MouseButtonState.Pressed
        }

        private void ViewModel_Play(Object sender, EventArgs e)
        {
            if (_model.TogglePlay()){
                if (_model.IsPlaying)
                {
                    _timer.Start();
                } else
                {
                    _timer.Stop();
                }
            }
        }

        private void ViewModel_Step(Object sender, EventArgs e)
        {
            _model.Step();
        }

        //private void ViewModel_Pause(Object sender, EventArgs e)
        //{
        //    if (_model.TogglePlay())
        //    {
        //        _timer.Stop();
        //    }
        //}

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

        private void ViewModel_CreatePattern(Object sender, ViewModel.NewPatternEventArgs e)
        {
            if (e.Size == null || e.Size == "")
            {
                MessageBox.Show("Please give a valid size!", "Game of Life", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                int size = int.Parse(e.Size);
                _model.NewPattern(size, e.Random);
                _newPatternWindow.Hide();
            }
            catch (Exception)
            {
                MessageBox.Show("Please give a valid size!", "Game of Life", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
