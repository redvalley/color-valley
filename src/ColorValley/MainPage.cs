namespace ACAB.App;

public class MainPage : ContentPage
{
    private readonly Grid _gameGrid = new();
    private readonly List<Button> _outerBoxes = new();

    private readonly List<(int row, int col)> _positions = new()
    {
        (0, 2), (2, 0), (2, 4), (4, 2), (0, 4) // Specific outer positions
    };

    private readonly Random _random = new();
    private Button _middleBox;
    private const int currentLevel = 1;
    private const int rowCount = 5;
    private const int columnCount = 5;
    private int _currentScore;
    private readonly Label _scoreLabel = new();
    private Label _timeLabel = new();
    private TimeSpan _levelTimeSpan = TimeSpan.FromMinutes(1);
    private TimeSpan _currentTimeSpan = new TimeSpan();
    private const int _boxLength = 70;
    private IDispatcherTimer _gameTimer;
    private IDispatcherTimer _timeTimer;
    private Label _gameInfoLabel = new Label();
    private Button _startOrTryAgainButton = new Button();
    private bool _isRunning = false;
    private int _comboHit = 0;
    public MainPage()
    {
        _gameTimer = Dispatcher.CreateTimer();
        _timeTimer = Dispatcher.CreateTimer();
        Title = "Dynamic Box Game";
        UpdatePageBackground();
        InitializeGameUi();
        AddMiddleBox();
        UpdateOuterBoxes();
        AddGameInfoLabel();
        AddStartOrTryAgainButton(false);
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

    private void InitializeGameUi()
    {
        _gameGrid.Margin = new Thickness(10,150,10,150);
        _gameGrid.Padding = 20;
        _gameGrid.BackgroundColor = new Color(0xFF, 0xFF, 0xFF,0x1A);

        for (var i = 0; i < rowCount; i++)
        {
            _gameGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            _gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        var mainGrid = new Grid();
        mainGrid.AddRowDefinition(new RowDefinition(50));
        mainGrid.AddRowDefinition(new RowDefinition(GridLength.Star));

        var headerLayout = new Grid();
        headerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
        headerLayout.AddColumnDefinition(new ColumnDefinition(GridLength.Star));

        _scoreLabel.HorizontalOptions = LayoutOptions.Start;
        _scoreLabel.Margin = 5;
        _scoreLabel.FontSize = 30;
        _scoreLabel.TextColor = Colors.White;
        _scoreLabel.Text = "\u2b50\ufe0f " + _currentScore;


        headerLayout.Add(_scoreLabel, 0);

        
        mainGrid.Add(headerLayout, 0);
        mainGrid.Add(_gameGrid, 0, 1);
        var swipeGestureRecognizer = new SwipeGestureRecognizer()
        {
            Direction = SwipeDirection.Right,
        };
        swipeGestureRecognizer.Swiped += (s, e) =>
        {
            UpdateGame();
        };
        _gameGrid.GestureRecognizers.Add(swipeGestureRecognizer);
        
        

        headerLayout.Add(_timeLabel,1);
        _timeLabel.TextColor = Colors.White;
        _timeLabel.FontSize = 30;
        _timeLabel.HorizontalOptions = LayoutOptions.Center;
        Content = mainGrid;

        _gameInfoLabel.FontSize = 50;
        _gameInfoLabel.TextColor = Colors.White;
        _gameInfoLabel.VerticalOptions = LayoutOptions.Center;
        _gameInfoLabel.HorizontalOptions = LayoutOptions.Center;
        _startOrTryAgainButton.BackgroundColor = Colors.White;
        _startOrTryAgainButton.TextColor = Colors.Black;
        _startOrTryAgainButton.FontSize = 20;
        _startOrTryAgainButton.WidthRequest = 200;
        _startOrTryAgainButton.HeightRequest = 50;

    }

    private void AddGameInfoLabel()
    {
        this._gameGrid.Add(_gameInfoLabel, 0, 0);
        this._gameGrid.SetColumnSpan(_gameInfoLabel, _boxLength);
        this._gameGrid.SetRowSpan(_gameInfoLabel, _boxLength);
    }

    private void AddStartOrTryAgainButton(bool retry)
    {
        this._gameGrid.Add(_startOrTryAgainButton, 0, 0);
        this._gameGrid.SetColumnSpan(_startOrTryAgainButton, _boxLength);
        this._gameGrid.SetRowSpan(_startOrTryAgainButton, _boxLength);
        this._startOrTryAgainButton.Text = retry ? "Retry?" : "\ud83d\ude80 Start?";

        _startOrTryAgainButton.Clicked += StartOrTryAgainButtonOnClicked;
    }

    private void StartOrTryAgainButtonOnClicked(object? sender, EventArgs e)
    {
        StartGame();
        _gameGrid.Remove(_startOrTryAgainButton);
        _startOrTryAgainButton.Clicked -= StartOrTryAgainButtonOnClicked;
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
        _currentScore = 0;
        _scoreLabel.Text = "⭐️ " + _currentScore;
        this.InitializeTime();

        this._gameTimer.Interval = TimeSpan.FromSeconds(2);
        this._gameTimer.Tick += GameTimerOnTick;
        this._timeTimer.Interval = TimeSpan.FromSeconds(1);
        this._timeTimer.Tick += TimeTimerOnTick;
        
        for (int gameCountDown = 3; gameCountDown > 0; gameCountDown--)
        {
            _gameInfoLabel.Text = gameCountDown.ToString();
            _gameInfoLabel.FontSize = 50;
            _gameInfoLabel.Animate("GameInfoLabelCountDown", d =>
            {
                _gameInfoLabel.FontSize += 1;
            },0, 2000, 10, 2000);
            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        _gameInfoLabel.Text = "GO!";
        _gameInfoLabel.FontSize = 50;
        
        _gameInfoLabel.Animate("GameInfoLabelCountDown", d =>
        {
            _gameInfoLabel.FontSize += 1;
        }, 0, 2000, 10, 2000,finished: (d, b) =>
        {
            _gameInfoLabel.Text = string.Empty;
            _gameTimer.Start();
            _timeTimer.Start();
            _isRunning = true;
        });
        
        


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
        AddStartOrTryAgainButton(true);
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

        // Create new outer boxes with random positions and colors
        var shuffledPositions = new List<(int row, int col)>(_positions);
        ShuffleList(shuffledPositions);

        foreach (var pos in shuffledPositions)
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
                    Offset = new Point(5,5),
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
                scoreToAdd = scoreToAdd * (int)Math.Pow(10, _comboHit -1);
            }
            
            UpdateScore(scoreToAdd);
        }
        else
        {
            UpdateScore(-1);
        }
    }

    private void UpdateScore(int scoreToAdd)
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
            } else if (_comboHit == 3)
            {
                newScoreLabel.Text = "\u2604\ufe0f +" + scoreToAdd;
            } else if (_comboHit > 3)
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
        
        this._gameGrid.Add(newScoreLabel, 0, 0);
        this._gameGrid.SetColumnSpan(newScoreLabel, _boxLength);
        this._gameGrid.SetRowSpan(newScoreLabel, _boxLength);
        if (_comboHit > 1)
        {
            newScoreLabel.Margin = new Thickness(-_comboHit * 50, -_comboHit * 50);
        }
        newScoreLabel.Animate("GameInfoLabelCountDown", d =>
        {
            newScoreLabel.FontSize += 1;
            
        }, 0, 1000, 10, 1000, finished: (d, b) =>
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

    private void ShuffleList<T>(IList<T> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}