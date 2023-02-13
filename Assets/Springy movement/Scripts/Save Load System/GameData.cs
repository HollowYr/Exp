using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEditor;
using UnityEngine;

public class GameData : Singleton<GameData>
{
    [SerializeField] private Transform player;

    private SaveData saveData;
    void Start()
    {
        // Application.quitting += Application_quitting;
        SLS.Init();
        Load();
    }

    public void Resubscribe() => Application.quitting += Application_quitting;

    public void Save()
    {
        saveData = new SaveData(player.position);
        SLS.Save(JsonUtility.ToJson(saveData));
    }

    public SaveData Load()
    {
        saveData = JsonUtility.FromJson<SaveData>(SLS.Load());
        return saveData;
    }


    private void Application_quitting()
    {
        Save();
    }


    public class SaveData
    {
        public Vector3 playerPosition;
        public Vector3 GetPlayerPosition() => playerPosition;

        public SaveData(Vector3 playerPosition)
        {
            this.playerPosition = playerPosition;
        }
    }
}
