namespace ColorValley.Services;

public interface IColorValleyAdService
{
    void LoadAd();
    Task ShowAd(Action onAdShownAction);
}