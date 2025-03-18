using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISavable
{
    void AddSavableRegister();
    GameSaveData GenerateGameData();
    void RestoreGameData(GameSaveData gameSaveData);
}
