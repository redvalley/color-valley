namespace ColorValley.Services;

public abstract class ColorValleyAdService<TAd, TAdService>(TAdService adService, string adUnitId)
    : IColorValleyAdService
    where TAd : class?
{
    public TAd? Ad { get; private set; }
    protected TAdService AdService { get; set; } = adService;
    private bool _isLoadingAd = false;
    private bool _isShowingAd = false;
    protected string AdUnitId { get; } = adUnitId;

    private bool IsAdAvailable => Ad != null;

    public void LoadAd()
    {

        if (_isLoadingAd || IsAdAvailable)
        {
            return;
        }
        _isLoadingAd = true;

        Ad = CreateAd();
        DoLoadAd();

        AttachAdLoadedHandler();
        AttachAdFailedToLoadHandler();
    }

    protected abstract TAd CreateAd();

    protected abstract void DoLoadAd();

    protected abstract void AttachAdFailedToLoadHandler();

    protected abstract void AttachAdLoadedHandler();

    protected abstract void AttachAdFailedToShowHandler(Action onAdShownAction);

    protected abstract void AttachAdDismissedHandler(Action onAdShownAction);

    public void ShowAd(Action onAdShownAction)
    {
        if (_isShowingAd)
        {
            return;
        }

        if (!IsAdAvailable)
        {
            onAdShownAction();
            LoadAd();
            return;
        }

        if (Ad != null)
        {
           AttachAdFailedToShowHandler(onAdShownAction);

           AttachAdDismissedHandler(onAdShownAction);
        }
            
    }

    protected virtual void OnAdDismissed(Action onAdShownAction)
    {
        Ad = null;
        _isShowingAd = false;
        onAdShownAction();
        LoadAd();
    }

    protected virtual void OnAdFailedToShow(Action onAdShownAction)
    {
        Ad = null;
        _isShowingAd = false;
        onAdShownAction();
        LoadAd();
    }

    protected virtual void OnAdLoaded()
    {
        _isLoadingAd = false;
    }

    protected virtual void OnAdFailedToLoad()
    {
        _isLoadingAd = false;
        Ad = null;
    }
}