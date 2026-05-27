using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;

/// <summary>
/// Manages user data for the application.
/// </summary>
public partial class UserDataManager : Node
{   
    private Dictionary<Identifier, UserData> userDataStore = new();

    #region Singleton Pattern
    public static UserDataManager Instance { get; private set; }

    private UserDataManager() { }

    public override void _EnterTree()
    {
        GD.Print("UserDataManager: EnterTree");
        if (Instance != null)
        {
            GD.PrintErr("UserDataManager: Multiple instances detected. This should not happen.");
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _ExitTree()
    {
        GD.Print("UserDataManager: ExitTree");
        if (Instance == this)
        {
            Instance = null;
        }

        WriteAll();
    }
    #endregion

    #region User Data Management

    public T RegisterUserData<T>(Identifier id, T userData)
        where T : UserData
    {
        if (userDataStore.TryGetValue(id, out UserData existing))
            return (T)existing;

        userDataStore[id] = userData;
        return userData;
    }

    public T GetUserData<T>(Identifier id) where T : UserData
    {
        if (userDataStore.TryGetValue(id, out var userData))
        {
            GD.Print($"UserDataManager: GetUserData - found data for {id}");
            return userData as T;
        }
        GD.PrintErr($"UserDataManager: GetUserData - user data with ID {id} not found.");
        return null;
    }
    #endregion

    #region File I/O 

    public string GetSaveFilePath()
    {
        string path = System.IO.Path.Combine(
            ProjectSettings.GlobalizePath("res://"), "userdata"
        );
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        GD.Print($"UserDataManager: GetSaveFilePath - base path {path}");
        return path;
    }
    public string GetSaveFilePath(Identifier id)
    {
        string filePath = System.IO.Path.Combine(GetSaveFilePath(), id.Namespace, $"{id.Path}.json");
        GD.Print($"UserDataManager: GetSaveFilePath - file path for {id} is {filePath}");
        return filePath;
    }

    public void WriteAll()
    {
        foreach (var (id, userData) in userDataStore)
        {
            string filePath = GetSaveFilePath(id);

            Directory.CreateDirectory(
                Path.GetDirectoryName(filePath)
            );

            string json = JsonSerializer.Serialize(
                userData,
                userData.GetType()
            );

            File.WriteAllText(filePath, json);
        }
    }

    public void ReadAll()
    {
        foreach (var (id, userData) in userDataStore)
        {
            string filePath = GetSaveFilePath(id);

            if (!File.Exists(filePath))
                continue;

            string json = File.ReadAllText(filePath);

            UserData loaded =
                (UserData)JsonSerializer.Deserialize(
                    json,
                    userData.GetType()
                );

            userDataStore[id] = loaded;
        }
    }
    #endregion

    #region Hooks(pois é um Node)
    public override void _Ready()
    {
        GD.Print("UserDataManager: Ready - initializing and reading saved user data");
        ReadAll();
    }
    // _ExitTree
    #endregion

    #region Utils
    public bool HasUserData(Identifier id)
    {
        return userDataStore.ContainsKey(id);
    }

    public T RegisterIfNotExists<T>(Identifier id, T userData) where T : UserData
    {
        if (HasUserData(id))
        {
            GD.Print($"UserDataManager: RegisterIfNotExists - data already exists for {id}, returning existing data");
            return userDataStore[id] as T;
        }

        GD.Print($"UserDataManager: RegisterIfNotExists - no existing data for {id}, registering new data");
        userDataStore[id] = userData;
        return userData;
    }
    #endregion
}