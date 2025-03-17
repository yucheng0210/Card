using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDefinition
{
    //戰鬥：回合相關
    public const string eventPlayerTurn = "EVENT_PLAYER_TURN";
    public const string eventBattleInitial = "EVENT_BATTLE_INITIAL";
    public const string eventEnemyTurn = "EVENT_ENEMY_TURN";
    public const string eventAfterEnemyAttack = "EVENT_AFTER_ENEMY_ATTACK";
    public const string eventBattleWin = "EVENT_BATTLE_WIN";
    public const string eventGameOver = "EVENT_GAME_OVER";
    public const string eventReloadGame = "EVENT_RELOAD_GAME";
    public const string eventAttack = "EVENT_ATTACK";
    public const string eventNextChapter = "EVENT_NEXT_CHAPTER";

    //戰鬥：卡牌相關
    public const string eventUseCard = "EVENT_USE_CARD";
    public const string eventTakeDamage = "EVENT_TAKE_DAMAGE";
    public const string eventRecover = "EVENT_RECOVER";
    public const string eventDrawCard = "EVENT_DRAW_CARD";
    public const string eventMove = "EVENT_MOVE";
    public const string eventAfterMove = "EVENT_AFTER_MOVE";

    //探索相關
    public const string eventExplore = "EVENT_EXPLORE";

    //UI相關
    public const string eventRefreshUI = "EVENT_REFRESH_UI";
    public const string eventAttackLine = "EVENT_ATTACKLINE";

    //物品相關
    public const string eventOnClickedToBag = "EVENT_ONCLICKED_TO_BAG";
    public const string eventReviseMoneyToBag = "EVENT_REVISE_MONEY_TO_BAG";
    public const string eventRemoveItemToBag = "EVENT_REMOVE_ITEM_TO_BAG";
    public const string eventAddItemToBag = "EVENT_ADD_ITEM_TO_BAG";
    public const string eventOnClickedToFarmland = "EVENT_ONCLICKED_TO_FARMLAND";
    public const string eventUseItem = "EVENT_USE_ITEM";

    //其他
    public const string eventStartGame = "EVENT_START_GAME";
    public const string eventDialog = "EVENT_DIALOG";
}
