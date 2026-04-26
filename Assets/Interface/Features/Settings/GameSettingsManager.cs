using System;
using UnityEngine;

public static class GameSettingsManager {
    const string ItemsPerPagePlayerPrefsKey = "settings_items_per_page";
    const string VolumePlayerPrefsKey = "settings_volume";
    static GameSettings _settings;

    public static event System.Action OnSettingsChanged;

    public static int ItemsPerPage {
        get {
            EnsureLoaded();
            return _settings.itemsPerPage;
        }
    }

    public static int Volume {
        get {
            EnsureLoaded();
            return _settings.volume;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize() {
        EnsureLoaded();
        ApplyVolume();
    }

    public static void SetItemsPerPage(int value) {
        EnsureLoaded();

        int clampedValue = Mathf.Clamp(value, 1, 20);
        if (_settings.itemsPerPage == clampedValue) {
            return;
        }

        _settings.itemsPerPage = clampedValue;
        PlayerPrefs.SetInt(ItemsPerPagePlayerPrefsKey, clampedValue);
        PlayerPrefs.Save();
        OnSettingsChanged?.Invoke();
    }

    public static void SetVolume(int value) {
        EnsureLoaded();

        int clampedValue = Mathf.Clamp(value, 0, 100);
        if (_settings.volume == clampedValue) {
            return;
        }

        _settings.volume = clampedValue;
        PlayerPrefs.SetInt(VolumePlayerPrefsKey, clampedValue);
        PlayerPrefs.Save();
        ApplyVolume();
        OnSettingsChanged?.Invoke();
    }

    static void EnsureLoaded() {
        if (_settings != null) {
            return;
        }

        _settings = new GameSettings {
            itemsPerPage = Mathf.Clamp(PlayerPrefs.GetInt(ItemsPerPagePlayerPrefsKey, 10), 1, 20),
            volume = Mathf.Clamp(PlayerPrefs.GetInt(VolumePlayerPrefsKey, 100), 0, 100),
        };
    }

    static void ApplyVolume() {
        AudioListener.volume = Volume / 100f;
    }
}
