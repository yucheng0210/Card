using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISavable
{
    void AddSavableRegister();

    void GenerateGameData(GameSaveData gameSaveData);
    void RestoreGameData(GameSaveData gameSaveData);
}
