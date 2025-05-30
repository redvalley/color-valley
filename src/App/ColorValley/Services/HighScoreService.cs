using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ColorValley.Models;
using ColorValley.Settings;
using iJus.Core.Settings;

namespace ColorValley.Services
{
    public class HighScoreService
    {
        private readonly HttpClient _client = new HttpClient();
        private static IDictionary<string, HighScoreEntry> _lastNotSavedEntryForUser = new Dictionary<string, HighScoreEntry>();


        public async Task<HighScoreEntrySaveResult?> SaveScoreAsync(HighScoreEntry highScoreEntry)
        {
            HighScoreEntrySaveResult saveResult = new HighScoreEntrySaveResult()
            {
                WasSavedOnline = false
            };

            highScoreEntry.Hash = highScoreEntry.ComputeHash();

            AppUserSettings currentUserSettings = UserSettings.LoadDecrypted<AppUserSettings>()??new AppUserSettings();

            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType == NetworkAccess.Internet)
            {
                try
                {
                    if (_lastNotSavedEntryForUser.TryGetValue(highScoreEntry.Name, out var lastNotSavedEntryForUserToSave))
                    {
                        if (currentUserSettings.LocalHighScoreEntries.Any(localHighScoreEntry =>
                                localHighScoreEntry.Name == lastNotSavedEntryForUserToSave.Name &&
                                localHighScoreEntry.Level == lastNotSavedEntryForUserToSave.Level &&
                                localHighScoreEntry.Score == lastNotSavedEntryForUserToSave.Score))
                        {
                            var savedLastNotSavedEntry = await SaveOnline(lastNotSavedEntryForUserToSave);
                            if (savedLastNotSavedEntry != null)
                            {
                                _lastNotSavedEntryForUser.Remove(savedLastNotSavedEntry.Name);
                            }
                        }
                        else
                        {
                            _lastNotSavedEntryForUser.Remove(lastNotSavedEntryForUserToSave.Name);
                        }
                    }

                    var savedEntry = await SaveOnline(highScoreEntry);
                    if (savedEntry != null)
                    {
                        saveResult.Entry = savedEntry;
                        saveResult.WasSavedOnline = true;
                    }
                }
                catch (Exception exception)
                {
                    saveResult.WasSavedOnline = false;
                    saveResult.SavedOnlineError = exception.Message;
                }
            }
            else
            {
                saveResult.WasSavedOnline = false;
            }

            try
            {
                
                if (currentUserSettings.AddLocalHighScore(highScoreEntry) && !saveResult.WasSavedOnline)
                {
                    _lastNotSavedEntryForUser.Add(highScoreEntry.Name, highScoreEntry);
                }
                currentUserSettings.SaveEncrypted();
                saveResult.WasSavedLocal = true;
            }
            catch (Exception exception)
            {
                saveResult.WasSavedLocal = false;
                saveResult.SavedLocalError = exception.Message;
            }


            return saveResult;
        }

        private async Task<HighScoreEntry?> SaveOnline(HighScoreEntry highScoreEntry)
        {
            var json = JsonSerializer.Serialize(highScoreEntry);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(AppSettings.HighScoreServiceUrl + "scores", content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return !string.IsNullOrEmpty(responseBody) ? JsonSerializer.Deserialize<HighScoreEntry>(responseBody) : null;
        }

        public async Task<GetOverallScoresResult> GetOverallScores()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            AppUserSettings currentUserSettings = UserSettings.LoadDecrypted<AppUserSettings>() ?? new AppUserSettings();

            if (accessType == NetworkAccess.Internet)
            {
                try
                {
                    HttpResponseMessage response =
                        await _client.GetAsync(AppSettings.HighScoreServiceUrl + "scores/top-overall");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseBody))
                    {
                        return new GetOverallScoresResult()
                        {
                            Entries = new List<HighScoreEntry>()
                        };
                    }

                    var onlineEntries = JsonSerializer.Deserialize<IEnumerable<HighScoreEntry>>(responseBody) ??
                                        new List<HighScoreEntry>();


                    currentUserSettings.LastOnlineHighScoreEntries = onlineEntries.ToList();
                    currentUserSettings.SaveEncrypted();

                    return new GetOverallScoresResult()
                    {
                        Entries = onlineEntries
                    };


                }
                catch (HttpRequestException e)
                {
                    return new GetOverallScoresResult()
                    {
                        Entries = currentUserSettings?.LastOnlineHighScoreEntries??new List<HighScoreEntry>(),
                        ErrorMessage = e.Message
                    };
                }
            }

            return new GetOverallScoresResult()
            {
                Entries = currentUserSettings?.LastOnlineHighScoreEntries ?? new List<HighScoreEntry>(),
            };
        }

    }
}
