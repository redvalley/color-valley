namespace ColorValley.Services;

public interface IColorValleyAdService
{
    void LoadAd();
    void ShowAd(Action onAdShownAction);
}