using System.Diagnostics;
using ColorValley;
using ColorValley.Models;
using ColorValley.Settings;
using iJus.Core.Settings;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Shapes;
using Plugin.Maui.Audio;

namespace ACAB.App;

public class MainPage : ContentPage
{
    private readonly Grid _gameGrid = new();
    private readonly List<Button> _outerBoxes = new();
    private const string DefaultPlayerName = "Color Valley";

    private readonly Grid _mainGrid = new();
    private readonly Random _random = new();
    private Button _middleBox;

    private int _currentScore;
    private readonly Label _scoreLabel = new();
    private readonly Label _levelNumberLabel = new();
    private readonly Label _levelNameLabel = new();

    private Label _timeLabel = new();
    private TimeSpan _currentTimeSpan = TimeSpan.Zero;

    private IDispatcherTimer _gameTimer;
    private IDispatcherTimer _timeTimer;
    private Label _gameInfoLabel = new Label();
    private Button _startOrRestartButton = new Button();
    private Button _nextLevelButton = new Button();

    private bool _isRunning = false;
    private int _comboHit = 0;
    private Button _impressumButton = new Button();
    private Button _dataPrivacyButton = new Button();
    private Button _helpButton = new Button();
    private Button _highScoreButton = new Button()
    {
        Text = ColorValley.Properties.Resources.ButtonTextHighScore
    };

    private Button _helpOverlayButton = new Button() { Text = ColorValley.Properties.Resources.ButtonTextHelp };
    private IAudioPlayer _audioPlayerSuccessSound;
    private IAudioPlayer _audioPlayerFailedSound;
    private IAudioPlayer _audioPlayerGameCountdown;
    private Grid _launcherOverlayGrid = new Grid();
    private Border _launchOverlayBorder = new Border();
    private Entry _playerEntry = new Entry();

    private LevelSettings _levelSettings = new LevelSettings();

    public MainPage()
    {
        _gameTimer = Dispatcher.CreateTimer();
        _timeTimer = Dispatcher.CreateTimer();
        _levelSettings = LevelSettings.CreateLevel1Settings();
        Title = ColorValley.Properties.Resources.MainPageTitle;
        InitializeSound();
        InitializeGameUi();

        UpdateGame();

        AddLauncherOverlay(false);
    }



    private void InitializeTime()
    {
        this._currentTimeSpan = TimeSpan.FromSeconds(_levelSettings.TotalTimeSeconds);
    }

    private void UpdatePageBackground()
    {
        Background = _levelSettings.GetRandomLinearGradientBrush();
    }

    private void InitializeSound()
    {
        IAudioManager audioManager = AudioManager.Current;
        Stream successSoundStream = FileSystem.OpenAppPackageFileAsync("game_success.mp3").Result;
        _audioPlayerSuccessSound = audioManager.CreatePlayer(successSoundStream);

        Stream failedSoundStream = FileSystem.OpenAppPackageFileAsync("game_failed.mp3").Result;
        _audioPlayerFailedSound = audioManager.CreatePlayer(failedSoundStream);

        Stream gameCountDownSoundStream = FileSystem.OpenAppPackageFileAsync("game_countdown.mp3").Result;
        _audioPlayerGameCountdown = audioManager.CreatePlayer(gameCountDownSoundStream);



    }

    private void InitializeGameUi()
    {
        Background = _levelSettings.GetRandomLinearGradientBrush();

        _gameGrid.Margin = new Thickness(10, 150, 10, 150);
        _gameGrid.Padding = 20;
        _gameGrid.BackgroundColor = new Color(0xFF, 0xFF, 0xFF, 0x1A);

        _mainGrid.AddRowDefinition(new RowDefinition(GridLength.Auto));
        _mainGrid.AddRowDefinition(new RowDefinition(GridLength.Star));
        _mainGrid.AddRowDefinition(new RowDefinition(GridLength.Auto));

        var headerLayout = new Grid();
        headerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
        headerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
        headerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
        headerLayout.AddRowDefinition(new RowDefinition(GridLength.Auto));
        headerLayout.AddRowDefinition(new RowDefinition(GridLength.Auto));


        _scoreLabel.HorizontalOptions = LayoutOptions.Start;
        _scoreLabel.Margin = 5;
        _scoreLabel.FontSize = 30;
        _scoreLabel.TextColor = Colors.White;
        _scoreLabel.Text = "\u2b50\ufe0f " + _currentScore;

        headerLayout.Add(_scoreLabel, 0);

        _levelNumberLabel.HorizontalOptions = LayoutOptions.Start;
        _levelNumberLabel.Margin = 5;
        _levelNumberLabel.FontSize = 30;
        _levelNumberLabel.TextColor = Colors.White;
        _levelNumberLabel.Text = ColorValley.Properties.Resources.LabelLevelText + " " + _levelSettings.Level;

        headerLayout.Add(_levelNumberLabel, 1);

        _levelNameLabel.HorizontalOptions = LayoutOptions.Center;
        _levelNameLabel.Margin = 5;
        _levelNameLabel.FontSize = 20;
        _levelNameLabel.HorizontalTextAlignment = TextAlignment.Center;
        _levelNameLabel.TextColor = Colors.White;
        _levelNameLabel.Text = _levelSettings.Name;

        headerLayout.Add(_levelNameLabel, 0, 1);
        Grid.SetColumnSpan(_levelNameLabel, 3);


        _timeLabel.TextColor = Colors.White;
        _timeLabel.FontSize = 30;
        _timeLabel.HorizontalOptions = LayoutOptions.Center;

        headerLayout.Add(_timeLabel, 2);



        _mainGrid.Add(headerLayout, 0);
        _mainGrid.Add(_gameGrid, 0, 1);
        var swipeGestureRecognizer = new SwipeGestureRecognizer()
        {
            Direction = SwipeDirection.Right | SwipeDirection.Left
        };
        swipeGestureRecognizer.Swiped += (s, e) =>
        {
            UpdateGame();
        };
        _gameGrid.GestureRecognizers.Add(swipeGestureRecognizer);
        _mainGrid.GestureRecognizers.Add(swipeGestureRecognizer);


        Content = _mainGrid;

        var footerLayout = new Grid();
        footerLayout.AddRowDefinition(new RowDefinition(GridLength.Auto));
        footerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        footerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        footerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
        footerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));

        _impressumButton.HorizontalOptions = LayoutOptions.Start;
        _impressumButton.Margin = 5;
        _impressumButton.FontSize = 15;
        _impressumButton.TextColor = Colors.White;
        _impressumButton.Text = ColorValley.Properties.Resources.ButtonTextImpressum;
        _impressumButton.BackgroundColor = Colors.Transparent;
        _impressumButton.Clicked += ImpressumButtonOnClicked;
        footerLayout.Add(_impressumButton, 0);

        _helpButton.HorizontalOptions = LayoutOptions.Start;
        _helpButton.Margin = 5;
        _helpButton.FontSize = 15;
        _helpButton.TextColor = Colors.White;
        _helpButton.Text = ColorValley.Properties.Resources.ButtonTextHelp;
        _helpButton.BackgroundColor = Colors.Transparent;
        _helpButton.Clicked += HelpButtonOnClicked;
        footerLayout.Add(_helpButton, 1);

        _highScoreButton.HorizontalOptions = LayoutOptions.Start;
        _highScoreButton.Margin = 5;
        _highScoreButton.FontSize = 20;
        _highScoreButton.FontAttributes = FontAttributes.Bold;
        _highScoreButton.TextColor = Colors.White;
        _highScoreButton.Text = ColorValley.Properties.Resources.ButtonTextHighScore;
        _highScoreButton.BackgroundColor = Colors.Transparent;
        _highScoreButton.Clicked += HighScoreButtonOnClicked;
        footerLayout.Add(_highScoreButton, 2);

        _dataPrivacyButton.HorizontalOptions = LayoutOptions.Start;
        _dataPrivacyButton.Margin = 5;
        _dataPrivacyButton.FontSize = 15;
        _dataPrivacyButton.WidthRequest = 150;
        _dataPrivacyButton.TextColor = Colors.White;
        _dataPrivacyButton.Text = ColorValley.Properties.Resources.ButtonTextDataPrivacyDeclaration;
        _dataPrivacyButton.LineBreakMode = LineBreakMode.WordWrap;
        _dataPrivacyButton.BackgroundColor = Colors.Transparent;
        _dataPrivacyButton.Clicked += DataPrivacyButtonOnClicked;
        footerLayout.Add(_dataPrivacyButton, 3);

        _mainGrid.Add(footerLayout, 0, 2);

        _gameInfoLabel.FontSize = 50;
        _gameInfoLabel.TextColor = Colors.White;
        _gameInfoLabel.VerticalOptions = LayoutOptions.Center;
        _gameInfoLabel.HorizontalOptions = LayoutOptions.Center;

        _startOrRestartButton.BackgroundColor = Colors.Blue;
        _startOrRestartButton.TextColor = Colors.White;
        _startOrRestartButton.FontSize = 20;
        _startOrRestartButton.WidthRequest = 200;
        _startOrRestartButton.HeightRequest = 50;
        _startOrRestartButton.Margin = new Thickness(20);

        _nextLevelButton.BackgroundColor = Colors.Blue;
        _nextLevelButton.TextColor = Colors.White;
        _nextLevelButton.FontSize = 20;
        _nextLevelButton.WidthRequest = 200;
        _nextLevelButton.HeightRequest = 50;
        _nextLevelButton.Text = ColorValley.Properties.Resources.ButtonTextNextLevel;
        _nextLevelButton.Margin = new Thickness(20);
        

        _helpOverlayButton.BackgroundColor = Colors.Blue;
        _helpOverlayButton.TextColor = Colors.White;
        _helpOverlayButton.FontSize = 20;
        _helpOverlayButton.WidthRequest = 200;
        _helpOverlayButton.HeightRequest = 50;

        _launcherOverlayGrid.BackgroundColor = new Color(0xFF, 0xFF, 0xFF);
        _launcherOverlayGrid.RowDefinitions = new RowDefinitionCollection()
        {
            new RowDefinition(GridLength.Auto),
            new RowDefinition(GridLength.Auto),
            new RowDefinition(GridLength.Auto),
            new RowDefinition(GridLength.Auto),
            new RowDefinition(GridLength.Auto),
            new RowDefinition(GridLength.Auto),
            new RowDefinition(GridLength.Auto)
        };
        _launchOverlayBorder.BackgroundColor = new Color(0xFF, 0xFF, 0xFF);
        _launchOverlayBorder.Content = _launcherOverlayGrid;
        _launchOverlayBorder.StrokeShape = new RoundRectangle()
        {
            CornerRadius = new CornerRadius(45)
        };
        _launchOverlayBorder.HeightRequest = 500;
        _launchOverlayBorder.Margin = new Thickness(20);

        _playerEntry.Margin = new Thickness(20, 0, 20, 0);
        _playerEntry.FontSize = 15;
        _playerEntry.Placeholder = DefaultPlayerName;

        NavigationPage.SetHasNavigationBar(this, false);

        _middleBox = new Button
        {
            BackgroundColor = _levelSettings.GetRandomColor(),
            BorderWidth = 2,
            CornerRadius = 5,
            BorderColor = Colors.Black,
            Margin = new Thickness(_levelSettings.BoxMargin),
            Shadow = new Shadow()
            {
                Offset = new Point(10, 10),
                Brush = new SolidColorBrush(Colors.Goldenrod)
            },
            Text = "🌟",
            FontSize = 20,
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill
        };
    }

    private void UpdateGameGridRowsAndColumns()
    {
        _gameGrid.RowDefinitions.Clear();
        _gameGrid.ColumnDefinitions.Clear();

        for (var i = 0; i < _levelSettings.RowCount; i++)
        {
            _gameGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        }

        for (var i = 0; i < _levelSettings.ColumnCount; i++)
        {
            _gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }
    }


    private void HighScoreButtonOnClicked(object? sender, EventArgs e)
    {
        this.Navigation.PushAsync(new HighScorePage()
        {
            Background = this.Background
        });
    }

    private void HelpButtonOnClicked(object? sender, EventArgs e)
    {
        this.Navigation.PushAsync(new HelpPage()
        {
            Background = this.Background
        });
    }


    private void DataPrivacyButtonOnClicked(object? sender, EventArgs e)
    {
        this.Navigation.PushAsync(new DataPrivacyPage()
        {
            Background = this.Background
        });
    }

    private void ImpressumButtonOnClicked(object? sender, EventArgs e)
    {
        this.Navigation.PushAsync(new ImpressumPage()
        {
            Background = this.Background
        });
    }

    private void AddGameInfoLabel()
    {
        this._gameGrid.Remove(_gameInfoLabel);
        this._gameGrid.Add(_gameInfoLabel, 0, 0);
        this._gameGrid.SetColumnSpan(_gameInfoLabel, _levelSettings.ColumnCount);
        this._gameGrid.SetRowSpan(_gameInfoLabel, _levelSettings.RowCount);
    }

    private void AddLauncherOverlay(bool levelDone)
    {
        _launcherOverlayGrid.Clear();

        var currentSettings = UserSettings.LoadDecrypted<AppUserSettings>();
        HighScoreEntry? topScore = currentSettings.GetTopScore();
        var topScoreString = topScore?.Score == null ? "-" : topScore.Score.ToString();
        var topScorePlayerString = topScore?.Name == null ? "" : $"({topScore.Name})";

        this._startOrRestartButton.Text = levelDone ? "\ud83d\ude80 Neustart?" : "\ud83d\ude80 Start?";

        if (!levelDone)
        {
            if (currentSettings.IsGameStartedFirstTime)
            {
                var titleLabelLaunchFirstTime = new Label()
                {
                    Text = "Hallo 🙂",
                    Margin = 20,
                    FontSize = 20,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                _launcherOverlayGrid.Add(titleLabelLaunchFirstTime, 0, 0);

                var titleLabelLaunchPlayerName = new Label()
                {
                    Text = "Bitte gib hier deinen Spielernamen für die Highscore Liste ein: ",
                    Margin = new Thickness(20, 20, 20, 5),
                    FontSize = 15
                };

                _launcherOverlayGrid.Add(titleLabelLaunchPlayerName, 0, 1);

                _launcherOverlayGrid.Add(_playerEntry, 0, 2);
                var labelStart = new Label()
                {
                    Text = "Möchtest du direkt starten?",
                    Margin = 20,
                    FontSize = 15
                };
                _launcherOverlayGrid.Add(labelStart, 0, 3);

                _launcherOverlayGrid.Add(_startOrRestartButton, 0, 4);
                var titleLabelLaunchFirstTimeManual = new Label()
                {
                    Text = "oder zuerst die Anleitung anschaun: ",
                    Margin = 20,
                    FontSize = 15
                };
                _launcherOverlayGrid.Add(titleLabelLaunchFirstTimeManual, 0, 5);
                _launcherOverlayGrid.Add(_helpOverlayButton, 0, 6);
            }
            else
            {


                var titleLabelLaunchNotFirstTime = new Label()
                {
                    Text = $"Hallo {currentSettings.PlayerName} 🙂",
                    Margin = 20,
                    FontSize = 25,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                _launcherOverlayGrid.Add(titleLabelLaunchNotFirstTime, 0, 0);


                if (topScore != null)
                {
                    var labelCurrentHighScore = new Label()
                    {
                        Text = $"🏆 Top Score: {topScoreString} \u2b50\ufe0f {topScorePlayerString}",
                        Margin = new Thickness(20, 10, 20, 5),
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    _launcherOverlayGrid.Add(labelCurrentHighScore, 0, 1);
                }


                var labelStart = new Label()
                {
                    Text = "Möchtest du direkt starten?",
                    Margin = 20,
                    FontSize = 15,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                _launcherOverlayGrid.Add(labelStart, 0, 2);
                _launcherOverlayGrid.Add(_startOrRestartButton, 0, 3);
                var labelChangePlayerName = new Label()
                {
                    Text = "... oder deinen Spielernamen (für die Highscore Liste) ändern: ",
                    Margin = new Thickness(20, 20, 20, 5),
                    FontSize = 15
                };
                _launcherOverlayGrid.Add(labelChangePlayerName, 0, 4);
                _playerEntry.Text = currentSettings.PlayerName;

                _launcherOverlayGrid.Add(_playerEntry, 0, 5);
            }
        }
        else
        {
            if (this._currentScore > (topScore?.Score ?? 0))
            {
                var titleLabelLaunchNotFirstTime = new Label()
                {
                    Text = $"🏆 Super {currentSettings.PlayerName} - Neuer Highscore!!!\nDu hast {this._currentScore} Punkte!",
                    Margin = 20,
                    FontSize = 20,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                _launcherOverlayGrid.Add(titleLabelLaunchNotFirstTime, 0, 0);
            }
            else
            {
                var scoreText = $"🎖 Super {currentSettings.PlayerName}\n\n du hast {this._currentScore} Punkte!";

                if (this._currentScore == 0)
                {
                    scoreText = $"🙁 Schade {currentSettings.PlayerName}, du hast leider {this._currentScore} Punkte!";
                }

                var titleLabelLaunchNotFirstTime = new Label()
                {
                    Text = scoreText,
                    Margin = 20,
                    FontSize = 20,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                _launcherOverlayGrid.Add(titleLabelLaunchNotFirstTime, 0, 0);

                if (topScore != null)
                {
                    var labelCurrentHighScore = new Label()
                    {
                        Text = $"🏆 Top Score: {topScoreString} \u2b50\ufe0f {topScorePlayerString}",
                        Margin = 20,
                        FontSize = 20,
                    };
                    _launcherOverlayGrid.Add(labelCurrentHighScore, 0, 1);
                }

            }

            _launcherOverlayGrid.Add(_startOrRestartButton, 0, 2);
            if (_levelSettings.Level != LevelSettings.LastLevel)
            {
                _launcherOverlayGrid.Add(_nextLevelButton, 0, 3);
            }
            
            var labelChangePlayerName = new Label()
            {
                Text = "... oder Spielernamen (für die Highscore Liste) ändern: ",
                Margin = new Thickness(20, 20, 20, 5),
                FontSize = 15
            };
            _launcherOverlayGrid.Add(labelChangePlayerName, 0, 4);
            _playerEntry.Text = currentSettings.PlayerName;
            _launcherOverlayGrid.Add(_playerEntry, 0, 5);
        }



        this._mainGrid.Add(_launchOverlayBorder, 0, 1);



        _startOrRestartButton.Clicked += StartOrRestartButtonOnClicked;
        _nextLevelButton.Clicked += NextLevelButtonOnClicked;
        _helpOverlayButton.Clicked += HelpButtonOnClicked;

        if (currentSettings.IsGameStartedFirstTime)
        {
            currentSettings.IsGameStartedFirstTime = false;
            currentSettings.SaveEncrypted();
        }
    }

    private void NextLevelButtonOnClicked(object? sender, EventArgs e)
    {
        StartGame(true);
    }

    private void StartOrRestartButtonOnClicked(object? sender, EventArgs e)
    {
        StartGame(false);
    }


    private async Task StartGame(bool nextLevel)
    {
        if (nextLevel)
        {
            UpdateLevel(false);
        }
        else
        {
            UpdateLevel(true);
        }

        UpdateGame();

        AppUserSettings appUserSettings = UserSettings.LoadDecrypted<AppUserSettings>();
        appUserSettings.PlayerName = string.IsNullOrEmpty(_playerEntry.Text) ? DefaultPlayerName : _playerEntry.Text;
        appUserSettings.SaveEncrypted();

        _mainGrid.Remove(_launchOverlayBorder);
        if (!nextLevel)
        {
            _currentScore = 0;
        }

        _scoreLabel.Text = "⭐️ " + _currentScore;
        this.InitializeTime();

        this._gameTimer.Interval = TimeSpan.FromSeconds(_levelSettings.GameTimerIntervallSeconds);
        this._gameTimer.Tick += GameTimerOnTick;
        this._timeTimer.Interval = TimeSpan.FromSeconds(1);
        this._timeTimer.Tick += TimeTimerOnTick;

        _audioPlayerGameCountdown.Play();
        for (int gameCountDown = 3; gameCountDown > 0; gameCountDown--)
        {
            _gameInfoLabel.Text = gameCountDown.ToString();
            _gameInfoLabel.FontSize = 50;
            _gameInfoLabel.Animate("GameInfoLabelCountDown", d =>
            {
                _gameInfoLabel.FontSize += 1;
            }, 0, 1000, 10, 1000);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        _gameInfoLabel.Text = "GO!";
        _gameInfoLabel.FontSize = 50;
        _gameInfoLabel.Animate("GameInfoLabelCountDown", d =>
        {
            _gameInfoLabel.FontSize += 1;
        }, 0, 1000, 10, 1000, finished: (d, b) =>
        {
            _gameInfoLabel.Text = string.Empty;
            _gameTimer.Start();
            _timeTimer.Start();
            _isRunning = true;
        });


        _startOrRestartButton.Clicked -= StartOrRestartButtonOnClicked;
        _nextLevelButton.Clicked -= NextLevelButtonOnClicked;
    }

    private void UpdateLevel(bool restart)
    {
        if (restart)
        {
            _levelSettings = LevelSettings.CreateLevel1Settings();
        }
        else
        {
            if (_levelSettings.Level == 1)
            {
                _levelSettings = LevelSettings.CreateLevel2Settings();
            } else if (_levelSettings.Level == 2)
            {
                _levelSettings = LevelSettings.CreateLevel3Settings();
            } else if (_levelSettings.Level == 3)
            {
                _levelSettings = LevelSettings.CreateLevel4Settings();
            } else if (_levelSettings.Level == 4)
            {
                _levelSettings = LevelSettings.CreateLevel5Settings();
            }
        }

        _levelNumberLabel.Text = ColorValley.Properties.Resources.LabelLevelText + " " + _levelSettings.Level;
        _levelNameLabel.Text = _levelSettings.Name;
    }

    private void TimeTimerOnTick(object? sender, EventArgs e)
    {
        this.UpdateTime(TimeSpan.FromSeconds(-1));
        if (this._currentTimeSpan.TotalMilliseconds <= 0)
        {
            this._gameTimer.Stop();
            this._timeTimer.Stop();

            _gameInfoLabel.Text = "STOP!";
            _gameInfoLabel.FontSize = 20;

            _gameInfoLabel.Animate("GameInfoLabelCountDown", d =>
            {
                _gameInfoLabel.FontSize += 1;
            }, 0, 1000, 10, 1000, finished: (d, b) =>
            {
                _gameInfoLabel.Text = string.Empty;
                _isRunning = false;
                this._gameTimer.Tick -= GameTimerOnTick;
                this._timeTimer.Tick -= TimeTimerOnTick;
                this.AskTryAgain();
            });
        }
    }

    private void AskTryAgain()
    {
        var currentUserSettings = UserSettings.LoadDecrypted<AppUserSettings>();
        currentUserSettings.AddHighScore(new HighScoreEntry()
        {
            Name = currentUserSettings.PlayerName,
            Score = _currentScore,
            Level = _levelSettings.Level
        });
        currentUserSettings.SaveEncrypted();
        AddLauncherOverlay(true);
    }

    private void GameTimerOnTick(object? sender, EventArgs e)
    {
        UpdateGame();
    }

    private void UpdateGame()
    {
        if (_levelSettings.GameTimerIntervallSeconds > 1)
        {
            RemoveOldGameGrid();
        }
        
        UpdateGameGridRowsAndColumns();
        UpdatePageBackground();
        UpdateMiddleBox();
        UpdateOuterBoxes();
        AddGameInfoLabel();
        this._comboHit = 0;
    }

    private void RemoveOldGameGrid()
    {
        _gameGrid.Animate("Remove Old Game Grid", d =>
        {
            _gameGrid.TranslationX = d;

        }, 0, -500, 100, 500, finished: (d, b) =>
        {
            _gameGrid.TranslationX = 0;
        });
    }

    private void UpdateMiddleBox()
    {
        _gameGrid.Remove(_middleBox);
        _gameGrid.Add(_middleBox, _levelSettings.MiddleColumnIndex, _levelSettings.MiddleRowIndex);
        _middleBox.BackgroundColor = _levelSettings.GetRandomColor();
    }

    private void UpdateOuterBoxes()
    {
        // Remove existing outer boxes
        foreach (var box in _outerBoxes) _gameGrid.Children.Remove(box);

        _outerBoxes.Clear();


        var randomPositions = GetRandomPositions(GenerateValidPositions());

        foreach (var pos in randomPositions)
        {
            var outerBox = new Button
            {
                BackgroundColor = _levelSettings.GetRandomColor(),
                BorderWidth = 1,
                CornerRadius = 5,
                BorderColor = Colors.White,
                Margin = _levelSettings.BoxMargin,
                Shadow = new Shadow()
                {
                    Offset = new Point(5, 5),
                    Brush = new SolidColorBrush(Colors.Black)
                }
            };
            outerBox.VerticalOptions = LayoutOptions.Fill;
            outerBox.HorizontalOptions = LayoutOptions.Fill;
            outerBox.Clicked += OnOuterBoxClicked;
            _outerBoxes.Add(outerBox);
            _gameGrid.Add(outerBox, pos.col, pos.row);
        }
    }

    private void UpdateTime(TimeSpan minusTimeSpan)
    {
        this._currentTimeSpan = this._currentTimeSpan.Add(minusTimeSpan);
        this._timeLabel.Text = "\u23f1 " + _currentTimeSpan.ToString("mm\\:ss");
    }

    private void OnOuterBoxClicked(object sender, EventArgs e)
    {
        if (!_isRunning)
        {
            return;
        }
        var clickedBox = (Button)sender;

        if (clickedBox.BackgroundColor.Equals(_middleBox.BackgroundColor) &&
            !clickedBox.BackgroundColor.Equals(Colors.Transparent))
        {
            clickedBox.BackgroundColor = Colors.Transparent;
            clickedBox.Shadow.Brush = Colors.Transparent;
            var currentOuterBoxes = _outerBoxes.ToList();
            if (!currentOuterBoxes.Any(outerBox => outerBox.BackgroundColor.Equals(_middleBox.BackgroundColor)))
            {
                _middleBox.BackgroundColor = Colors.Transparent;
                _middleBox.Shadow.Brush = Brush.Transparent;
            }

            _comboHit++;

            var scoreToAdd = 5 * _levelSettings.Level;
            if (_comboHit > 1)
            {
                scoreToAdd = scoreToAdd * (int)Math.Pow(10, _comboHit - 1);
            }

            _audioPlayerSuccessSound.Play();
            UpdateScore(clickedBox, scoreToAdd);
        }
        else
        {
            _audioPlayerFailedSound.Play();
            UpdateScore(clickedBox, -1);
        }
    }

    private void UpdateScore(Button clickedBox, int scoreToAdd)
    {
        _currentScore += scoreToAdd;
        _scoreLabel.Text = "⭐️ " + _currentScore;
        var newScoreLabel = new Label();
        newScoreLabel.FontSize = 20;
        if (scoreToAdd < 0)
        {
            newScoreLabel.TextColor = Colors.Red;
            newScoreLabel.Text = "\ud83d\udca5" + scoreToAdd;
        }
        else
        {
            newScoreLabel.TextColor = Colors.White;
            if (_comboHit == 2)
            {
                newScoreLabel.Text = "\ud83c\udf1f +" + scoreToAdd;
            }
            else if (_comboHit == 3)
            {
                newScoreLabel.Text = "\u2604\ufe0f +" + scoreToAdd;
            }
            else if (_comboHit > 3)
            {
                newScoreLabel.Text = "\ud83c\udf96 +" + scoreToAdd;
            }
            else
            {
                newScoreLabel.Text = "\u2b50\ufe0f +" + scoreToAdd;
            }


        }

        newScoreLabel.VerticalOptions = LayoutOptions.Center;
        newScoreLabel.HorizontalOptions = LayoutOptions.Center;

        this._gameGrid.Add(newScoreLabel, Grid.GetColumn(clickedBox), Grid.GetRow(clickedBox));


        if (_comboHit > 1)
        {
            newScoreLabel.Margin = new Thickness(-_comboHit * 50, -_comboHit * 50);
        }
        newScoreLabel.Animate("NewScoreLabel", d =>
        {
            newScoreLabel.FontSize += 1;

        }, 0, 500, 100, 500, finished: (d, b) =>
        {
            newScoreLabel.Text = string.Empty;
            this._gameGrid.Remove(newScoreLabel);
        });
    }


    private List<(int row, int col)> GetRandomPositions(List<(int row, int col)> allValidPositions)
    {
        var positionsForRandomization = allValidPositions.ToList();
        var randomPositions = new List<(int row, int col)>();

        for (int i = 0; i < _levelSettings.BoxCount; i++)
        {
            var randomItemIndex = _random.Next(0, positionsForRandomization.Count);
            randomPositions.Add(positionsForRandomization[randomItemIndex]);
            positionsForRandomization.RemoveAt(randomItemIndex);
        }

        return randomPositions;
    }

    private List<(int row, int col)> GenerateValidPositions()
    {
        var midColumnIndex = _levelSettings.MiddleColumnIndex;
        var midRowIndex = _levelSettings.MiddleRowIndex;

        List<(int row, int col)> validPositions = new List<(int row, int col)>();

        for (int columnIndex = 0; columnIndex < _levelSettings.ColumnCount; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < _levelSettings.RowCount; rowIndex++)
            {
                if (!(columnIndex == midColumnIndex && rowIndex == midRowIndex))
                {
                    validPositions.Add((rowIndex, columnIndex));
                }
            }
        }

        return validPositions;
    }
}