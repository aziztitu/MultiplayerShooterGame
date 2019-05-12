using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using UnityEngine;

/// <summary>
/// Modify the list of entries in this enum based on the data stored & used in the game.
/// </summary>
public enum GameDataKey
{
    
}


/// <summary>
/// A Singleton implementation of GameManager that can be accessed from any script.
/// 
/// Controls the flow of Game Data, and handles generic game mechanics irrespective of the loaded scene.
/// </summary>
[Serializable]
public class GameManager
{
    private static GameManager _instance;

    /// <summary>
    /// A static reference to the GameManager instance
    /// </summary>
    [HideInInspector]
    public static GameManager Instance
    {
        get { return _instance ?? (_instance = new GameManager()); }
    }

    /// <summary>
    /// Used as the key for the JSON formatted profile list in the PlayerPrefs
    /// </summary>
    private const string ProfileListKey = "ProfileList";

    /// <summary>
    /// Used to join a profile name and a key value.
    /// </summary>
    private const string ProfileKeyJoiner = ".";

    /// <summary>
    /// The default profile name to use.
    /// </summary>
    public const string DefaultProfile = "default";

    /// <summary>
    /// Title of the game
    /// </summary>
    [ReadOnly] public string GameTitle = "MyAwsmaticGame!";

    [SerializeField] [ReadOnly] private ProfileListManager _profileListManager = new ProfileListManager();

    /// <summary>
    /// The currently loaded profile. Defaults to the DefaultProfile
    /// </summary>
    [SerializeField] private string _currentProfile = DefaultProfile;

    public string CurrentProfile
    {
        get { return _currentProfile; }
    }

    /// <summary>
    /// The actual GameData used during the game.
    /// Any changes to this variable will be visible throughout the game's runtime.
    /// 
    /// Although, changes will not be written to the disk automatically.
    /// You need to call SaveGameData() to save the changes to the disk.
    /// </summary>
    private Dictionary<GameDataKey, string> _gameData = new Dictionary<GameDataKey, string>();

    /// <summary>
    /// Controls if the _gameDataPreview is refreshed every time the _gameData changes.
    /// </summary>
#if UNITY_EDITOR
    [SerializeField] private bool _previewGameData = true;
#else
    private bool _previewGameData = false;
#endif

    /// <summary>
    /// Used to display the game data in the inspector.
    /// </summary>
    [SerializeField] [ReadOnly] private GameDataPreview _gameDataPreview = new GameDataPreview();


    private GameManager()
    {
    }

    /// <summary>
    /// Get a list of profiles from disk.
    /// </summary>
    /// <returns>A List of profile names</returns>
    public List<string> GetProfileList()
    {
        _profileListManager.Refresh();
        return _profileListManager.GetList();
    }

    public void SetGameData(GameDataKey key, int value)
    {
        SetGameData(key, value.ToString());
    }

    public void SetGameData(GameDataKey key, float value)
    {
        SetGameData(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public void SetGameData(GameDataKey key, string value)
    {
        _gameData[key] = value;
        _gameDataPreview.RefreshGameData();
    }

    public int GetIntGameData(GameDataKey key, int defaultValue)
    {
        if (_gameData.ContainsKey(key))
        {
            int value;
            if (Int32.TryParse(_gameData[key], out value))
            {
                return value;
            }
        }

        return defaultValue;
    }

    public float GetFloatGameData(GameDataKey key, float defaultValue)
    {
        if (_gameData.ContainsKey(key))
        {
            float value;
            if (float.TryParse(_gameData[key], out value))
            {
                return value;
            }
        }

        return defaultValue;
    }

    public string GetStringGameData(GameDataKey key, string defaultValue)
    {
        if (_gameData.ContainsKey(key))
        {
            return _gameData[key];
        }

        return defaultValue;
    }

    /// <summary>
    /// Clears the game data for the specified profile from the disk
    /// </summary>
    /// <param name="profile">Profile name to clear the data from</param>
    /// <param name="deleteEntry">If nor not to delete the entry from profile list</param>
    private void ClearProfileFromDisk(string profile, bool deleteEntry)
    {
        if (deleteEntry)
        {
            _profileListManager.DeleteProfileEntry(profile);
        }

        GameDataKey[] dataKeys = Enum.GetValues(typeof(GameDataKey)) as GameDataKey[];
        if (dataKeys != null)
        {
            foreach (GameDataKey dataKey in dataKeys)
            {
                string key = profile + ProfileKeyJoiner + dataKey.ToString();
                if (PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.DeleteKey(key);
                }
            }
        }

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Saves the game data to current loaded profile.
    /// If no profile is loaded, the default profile is considered to be the current profile. 
    /// </summary>
    /// <seealso cref="SaveGameData(string)"/>
    public void SaveGameData()
    {
        SaveGameData(_currentProfile);
    }

    /// <summary>
    /// Saves the game data to disk on the specified profile. 
    /// The saved game data will persist even after the game closes.
    /// </summary>
    /// <param name="profile">Profile name to save the data to.</param>
    public void SaveGameData(string profile)
    {
        bool gameDataCleared = _gameData.Count == 0;

        ClearProfileFromDisk(profile, gameDataCleared);

        if (!gameDataCleared)
        {
            _profileListManager.AddProfileEntry(profile);
        }

        foreach (KeyValuePair<GameDataKey, string> keyValuePair in _gameData)
        {
            string key = profile + ProfileKeyJoiner + keyValuePair.Key.ToString();
            PlayerPrefs.SetString(key, keyValuePair.Value);
        }

        PlayerPrefs.Save();
    }


    /// <summary>
    /// Loads the game data from the current loaded profile from the disk. 
    /// If no profile is loaded, the default profile is considered to be the current profile. 
    /// </summary>
    /// <seealso cref="LoadGameData(string)"/>
    public void LoadGameData()
    {
        LoadGameData(_currentProfile);
    }

    /// <summary>
    /// Loads the game data from a specified profile from the disk. 
    /// NOTE: It is okay for the profile to be non-existent. When SaveGameData() is called the current loaded profile will be saved.
    /// </summary>
    /// <param name="profile">Profile name to load the data from.</param>
    public void LoadGameData(string profile)
    {
        _gameData.Clear();

        GameDataKey[] dataKeys = Enum.GetValues(typeof(GameDataKey)) as GameDataKey[];
        if (dataKeys != null)
        {
            foreach (GameDataKey dataKey in dataKeys)
            {
                string key = profile + ProfileKeyJoiner + dataKey.ToString();
                if (PlayerPrefs.HasKey(key))
                {
                    _gameData[dataKey] = PlayerPrefs.GetString(key);
                }
            }
        }

        _currentProfile = profile;

        _gameDataPreview.RefreshGameData();
    }

    /// <summary>
    /// Clears Game Data from the current loaded profile
    /// If no profile is loaded, the default profile is considered to be the current profile. 
    /// </summary>
    /// <param name="clearFromDisk">
    /// If set to true, along with the runtime data being cleared, the game data will also be cleared from disk. 
    /// If set to false, only the runtime data is cleared, leaving the game data on disk untouched. 
    /// </param>
    /// <seealso cref="ClearGameData(bool, string)"/>
    public void ClearGameData(bool clearFromDisk)
    {
        ClearGameData(clearFromDisk, _currentProfile);
    }

    /// <summary>
    /// Clears Game Data
    /// </summary>
    /// <param name="clearFromDisk">
    /// If set to true, along with the runtime data being cleared, the game data will also be cleared from disk. 
    /// If set to false, only the runtime data is cleared, leaving the game data on disk untouched. 
    /// </param>
    /// <param name="profile">Profile name to clear data from</param>
    public void ClearGameData(bool clearFromDisk, string profile)
    {
        _gameData.Clear();
        _gameDataPreview.RefreshGameData();

        if (clearFromDisk)
        {
            SaveGameData(profile);
        }
    }

    /// <summary>
    /// A Wrapper class for managing the profile list.
    /// </summary>
    [Serializable]
    private class ProfileListManager
    {
        [SerializeField] ProfileListWrapper _profileListWrapper = new ProfileListWrapper();

        /// <summary>
        /// Needed to convert the List into a JSON data using JsonUtility.
        /// </summary>
        [Serializable]
        private class ProfileListWrapper
        {
            [SerializeField] public List<string> ProfileList = new List<string>();
        }

        /// <summary>
        /// Loads the profile list from disk into memory
        /// </summary>
        public void Refresh()
        {
            _profileListWrapper.ProfileList.Clear();

            if (PlayerPrefs.HasKey(ProfileListKey))
            {
                string profilesJson = PlayerPrefs.GetString(ProfileListKey);

                _profileListWrapper.ProfileList = JsonUtility.FromJson<ProfileListWrapper>(profilesJson).ProfileList;
            }
        }

        /// <summary>
        /// Writes the profile list from memory to disk
        /// </summary>
        private void WriteThrough()
        {
            string profilesJson = JsonUtility.ToJson(_profileListWrapper);
            PlayerPrefs.SetString(ProfileListKey, profilesJson);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// If the specified profile doesn't exist, adds a new profile entry to the list and writes to disk.
        /// </summary>
        /// <param name="profile">Profile name</param>
        public void AddProfileEntry(string profile)
        {
            Refresh();

            if (!_profileListWrapper.ProfileList.Contains(profile))
            {
                _profileListWrapper.ProfileList.Add(profile);
                WriteThrough();
            }
        }

        /// <summary>
        /// If the specified profile exists, deletes the profile entry from the list and writes to disk.
        /// </summary>
        /// <param name="profile">Profile name</param>
        public void DeleteProfileEntry(string profile)
        {
            Refresh();

            if (_profileListWrapper.ProfileList.Contains(profile))
            {
                _profileListWrapper.ProfileList.Remove(profile);
                WriteThrough();
            }
        }

        /// <summary>
        /// Get the list of profiles.
        /// </summary>
        /// <returns>Profile List</returns>
        public List<string> GetList()
        {
            return _profileListWrapper.ProfileList;
        }
    }


    /// <summary>
    /// A Wrapper class for previewing the game data in a read only manner
    /// </summary>
    [Serializable]
    private class GameDataPreview
    {
        [SerializeField] List<GameDataPreviewItem> _gameDataPreviewItems = new List<GameDataPreviewItem>();

        /// <summary>
        /// Refreshes the game data preview list by loading all the values from the actual game data.
        /// 
        /// If the previewGameData is set to false, this method simply exits.
        /// </summary>
        public void RefreshGameData()
        {
            if (!GameManager.Instance._previewGameData)
                return;

            _gameDataPreviewItems.Clear();

            foreach (KeyValuePair<GameDataKey, string> keyValuePair in GameManager.Instance._gameData)
            {
                GameDataPreviewItem gameDataPreviewItem = new GameDataPreviewItem
                {
                    Name = keyValuePair.Key.ToString(),
                    Value = keyValuePair.Value
                };

                _gameDataPreviewItems.Add(gameDataPreviewItem);
            }
        }
    }

    /// <summary>
    /// A simple model class to store an item for the game data preview.
    /// </summary>
    [Serializable]
    private class GameDataPreviewItem
    {
        [HideInInspector] public string Name;
        public string Value;
    }
}