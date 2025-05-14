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

    private readonly List<(int row, int col)> _allValidPositions = new()
    {
        (0,0),(0,2), (0,4),
        (2,0),(2,4),
        (4,0),(4,2), (4,4)
    };

    private readonly Grid _mainGrid = new();

    private readonly Random _random = new();
    private Button _middleBox;
    private const int currentLevel = 1;
    private const int rowCount = 5;
    private const int columnCount = 5;
    private int _gameTimerIntervallSeconds = 2; 
    private const int gameGridBoxCount = 5;
    private int _currentScore;
    private readonly Label _scoreLabel = new();
    private Label _timeLabel = new();
    private TimeSpan _levelTimeSpan = TimeSpan.FromSeconds(60);
    private TimeSpan _currentTimeSpan = new TimeSpan();
    private const int _boxLength = 70;
    private IDispatcherTimer _gameTimer;
    private IDispatcherTimer _timeTimer;
    private Label _gameInfoLabel = new Label();
    private Button _startOrTryAgainButton = new Button();
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

    public MainPage()
    {
        _gameTimer = Dispatcher.CreateTimer();
        _timeTimer = Dispatcher.CreateTimer();
        Title = "Dynamic Box Game";
        UpdatePageBackground();
        InitializeSound();
        InitializeGameUi();
        AddMiddleBox();
        UpdateOuterBoxes();
        AddGameInfoLabel();
        AddLauncherOverlay(false);
    }



    private void InitializeTime()
    {
        this._currentTimeSpan = _levelTimeSpan;
    }

    private void UpdatePageBackground()
    {
        var firstColor = GetRandomColor();
        var secondColor = GetRandomColor(firstColor);

        // Random gradient background
        Background = new LinearGradientBrush(
            [
                new GradientStop { Color = firstColor, Offset = 0.0f },
                new GradientStop { Color = secondColor, Offset = 1.0f }
            ],
            new Point(0, 0),
            new Point(1, 1));
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

        _gameGrid.Margin = new Thickness(10, 150, 10, 150);
        _gameGrid.Padding = 20;
        _gameGrid.BackgroundColor = new Color(0xFF, 0xFF, 0xFF, 0x1A);

        for (var i = 0; i < rowCount; i++)
        {
            _gameGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            _gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }




        _mainGrid.AddRowDefinition(new RowDefinition(GridLength.Auto));
        _mainGrid.AddRowDefinition(new RowDefinition(GridLength.Star));
        _mainGrid.AddRowDefinition(new RowDefinition(GridLength.Auto));

        var headerLayout = new Grid();
        headerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
        headerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Star));


        _scoreLabel.HorizontalOptions = LayoutOptions.Start;
        _scoreLabel.Margin = 5;
        _scoreLabel.FontSize = 30;
        _scoreLabel.TextColor = Colors.White;
        _scoreLabel.Text = "\u2b50\ufe0f " + _currentScore;


        headerLayout.Add(_scoreLabel, 0);

        _timeLabel.TextColor = Colors.White;
        _timeLabel.FontSize = 30;
        _timeLabel.HorizontalOptions = LayoutOptions.Center;

        headerLayout.Add(_timeLabel, 1);



        _mainGrid.Add(headerLayout, 0);
        _mainGrid.Add(_gameGrid, 0, 1);
        var swipeGestureRecognizer = new SwipeGestureRecognizer()
        {
            Direction = SwipeDirection.Right,
        };
        swipeGestureRecognizer.Swiped += (s, e) =>
        {
            UpdateGame();
        };
        _gameGrid.GestureRecognizers.Add(swipeGestureRecognizer);



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

        _startOrTryAgainButton.BackgroundColor = Colors.Blue;
        _startOrTryAgainButton.TextColor = Colors.White;
        _startOrTryAgainButton.FontSize = 20;
        _startOrTryAgainButton.WidthRequest = 200;
        _startOrTryAgainButton.HeightRequest = 50;

        _helpOverlayButton.BackgroundColor = Colors.Blue;
        _helpOverlayButton.TextColor = Colors.White;
        _helpOverlayButton.FontSize = 20;
        _helpOverlayButton.WidthRequest = 200;
        _helpOverlayButton.HeightRequest = 50;

        _launcherOverlayGrid.BackgroundColor = new Color(0xFF,0xFF,0xFF);
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
        this._gameGrid.Add(_gameInfoLabel, 0, 0);
        this._gameGrid.SetColumnSpan(_gameInfoLabel, _boxLength);
        this._gameGrid.SetRowSpan(_gameInfoLabel, _boxLength);
    }

    private void AddLauncherOverlay(bool retry)
    {
        _launcherOverlayGrid.Clear();
        
        var currentSettings = UserSettings.LoadDecrypted<AppUserSettings>();
        HighScoreEntry? topScore = currentSettings.GetTopScore();
        var topScoreString = topScore?.Score == null ? "-" : topScore.Score.ToString();
        var topScorePlayerString = topScore?.Name == null ? "" : $"({topScore.Name})";

        this._startOrTryAgainButton.Text = retry ? "\ud83d\ude80 Nochmal?" : "\ud83d\ude80 Start?";

        if (!retry)
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
                _launcherOverlayGrid.Add(titleLabelLaunchFirstTime,0,0);

                var titleLabelLaunchPlayerName = new Label()
                {
                    Text = "Bitte gib hier deinen Spielernamen für die Highscore Liste ein: ",
                    Margin = new Thickness(20,20,20,5),
                    FontSize = 15
                };

                _launcherOverlayGrid.Add(titleLabelLaunchPlayerName, 0, 1);

                _launcherOverlayGrid.Add(_playerEntry,0,2);
                var labelStart = new Label()
                {
                    Text = "Möchtest du direkt starten?",
                    Margin = 20,
                    FontSize = 15
                };
                _launcherOverlayGrid.Add(labelStart,0,3);
                
                _launcherOverlayGrid.Add(_startOrTryAgainButton, 0, 4);
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
                _launcherOverlayGrid.Add(_startOrTryAgainButton, 0, 3);
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
            if (this._currentScore > (topScore?.Score??0))
            {
                var titleLabelLaunchNotFirstTime = new Label()
                {
                    Text= $"🏆 Super {currentSettings.PlayerName} - Neuer Highscore!!!\nDu hast {this._currentScore} Punkte!",
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

            _launcherOverlayGrid.Add(_startOrTryAgainButton, 0, 2);
            var labelChangePlayerName = new Label()
            {
                Text = "... oder Spielernamen (für die Highscore Liste) ändern: ",
                Margin = new Thickness(20, 20, 20, 5),
                FontSize = 15
            };
            _launcherOverlayGrid.Add(labelChangePlayerName, 0, 3);
            _playerEntry.Text = currentSettings.PlayerName;
            _launcherOverlayGrid.Add(_playerEntry, 0, 4);
        }



        this._mainGrid.Add(_launchOverlayBorder, 0, 1);



        _startOrTryAgainButton.Clicked += StartOrTryAgainButtonOnClicked;
        _helpOverlayButton.Clicked += HelpButtonOnClicked;

        if (currentSettings.IsGameStartedFirstTime)
        {
            currentSettings.IsGameStartedFirstTime = false;
            currentSettings.SaveEncrypted();
        }
    }

    private void StartOrTryAgainButtonOnClicked(object? sender, EventArgs e)
    {
        StartGame();
    }

    private void AddMiddleBox()
    {
        _middleBox = new Button
        {
            BackgroundColor = GetRandomColor(),
            BorderWidth = 2,
            CornerRadius = 5,
            BorderColor = Colors.White,
            WidthRequest = _boxLength + 20,
            HeightRequest = _boxLength + 20,
            Shadow = new Shadow()
            {
                Offset = new Point(5, 5),
                Brush = new SolidColorBrush(Colors.Goldenrod)
            }
        };

        _middleBox.VerticalOptions = LayoutOptions.Fill;
        _middleBox.HorizontalOptions = LayoutOptions.Fill;

        _gameGrid.Add(_middleBox, 2, 2); // Place in the center of the grid
    }

    private async Task StartGame()
    {
        AppUserSettings appUserSettings = UserSettings.LoadDecrypted<AppUserSettings>();
        appUserSettings.PlayerName = string.IsNullOrEmpty(_playerEntry.Text) ? DefaultPlayerName : _playerEntry.Text;
        appUserSettings.SaveEncrypted();

        _mainGrid.Remove(_launchOverlayBorder);

        _currentScore = 0;
        _scoreLabel.Text = "⭐️ " + _currentScore;
        this.InitializeTime();

        this._gameTimer.Interval = TimeSpan.FromSeconds(_gameTimerIntervallSeconds);
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

        
        _startOrTryAgainButton.Clicked -= StartOrTryAgainButtonOnClicked;
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
            Score = _currentScore
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
        UpdatePageBackground();
        UpdateMiddleBoxColor();
        UpdateOuterBoxes();
        this._comboHit = 0;
    }


    private void UpdateMiddleBoxColor()
    {
        _middleBox.BackgroundColor = GetRandomColor();
        _middleBox.Shadow.Offset = new Point(5, 5);
        _middleBox.Shadow.Brush = Brush.Goldenrod;
    }

    private void UpdateOuterBoxes()
    {
        // Remove existing outer boxes
        foreach (var box in _outerBoxes) _gameGrid.Children.Remove(box);

        _outerBoxes.Clear();


        var randomPositions = GetRandomPositions(_allValidPositions);

        foreach (var pos in randomPositions)
        {
            var outerBox = new Button
            {
                BackgroundColor = GetRandomColor(),
                BorderWidth = 1,
                CornerRadius = 5,
                BorderColor = Colors.White,
                WidthRequest = _boxLength,
                HeightRequest = _boxLength,
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

            var scoreToAdd = 5 * currentLevel;
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

    private Color GetRandomColor(Color? exeptColor = null)
    {
        var colors = new List<Color> { Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Purple };
        if (exeptColor != null) colors.Remove(exeptColor);
        return colors[_random.Next(colors.Count)];
    }

    private List<(int row, int col)> GetRandomPositions(List<(int row, int col)> allValidPositions)
    {
        var positionsForRandomization = allValidPositions.ToList();
        var randomPositions = new List<(int row, int col)>();

        for (int i = 0; i < gameGridBoxCount; i++)
        {
            var randomItemIndex = _random.Next(0, positionsForRandomization.Count);
            randomPositions.Add(positionsForRandomization[randomItemIndex]);
            positionsForRandomization.RemoveAt(randomItemIndex);
        }

        return randomPositions;
    }
}