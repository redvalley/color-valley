using System.ComponentModel;
using System.Runtime.CompilerServices;
using ColorValley.Models;

namespace ColorValley.ViewModels;

public class HighScoreViewModel : INotifyPropertyChanged
{
    private IEnumerable<HighScoreEntry> _localEntries = new List<HighScoreEntry>();
    private IEnumerable<HighScoreEntry> _onlineEntries = new List<HighScoreEntry>();

    public IEnumerable<HighScoreEntry> LocalEntries
    {
        get => _localEntries;
        set => SetField(ref _localEntries, value);
    }

    public IEnumerable<HighScoreEntry> OnlineEntries
    {
        get => _onlineEntries;
        set => SetField(ref _onlineEntries, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}