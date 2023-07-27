using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDefinition
{
    //戰鬥：回合相關
    public const string eventPlayerTurn = "EVENT_PLAYER_TURN";
    public const string eventBattleInitial = "EVENT_BATTLE_INITIAL";
    public const string eventEnemyTurn = "EVENT_ENEMY_TURN";
    public const string eventBattleWin = "EVENT_BATTLE_WIN";

    //戰鬥：卡牌相關
    public const string eventUseCard = "EVENT_USE_CARD";
    public const string eventTakeDamage = "EVENT_TAKE_DAMAGE";
    public const string eventDrawCard = "EVENT_DRAW_CARD";

    //探索相關
    public const string eventExplore = "EVENT_EXPLORE";

    //UI相關
    public const string eventRefreshUI = "EVENT_REFRESH_UI";
    public const string eventAttackLine = "EVENT_ATTACKLINE";

    //其他
    public const string eventStartGame = "EVENT_START_GAME";
    public const string eventDialog = "EVENT_DIALOG";
}
