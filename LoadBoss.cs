using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.ServerEvents;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;

namespace CEditorBoss;

[HarmonyPatch(typeof(ChallengeEditorPlay), nameof(ChallengeEditorPlay.StartNewGame))]
public static class StartNewGamePatch
{
    private static void Prefix()
    {
        BossLoader.ExecuteSetup();
    }
}

[HarmonyPatch(typeof(ChallengeEditorPlay), nameof(ChallengeEditorPlay.ContinueClicked))]
public static class ContinueBtnPatch
{
    private static void Prefix()
    {
        BossLoader.ExecuteSetup();
    }
}

public static class BossLoader
{
    private static Il2CppAssets.Scripts.Data.Boss.BossType ConvertToGameBossType(BossType bossType)
    {
        return bossType switch
        {
            BossType.Bloonarius => Il2CppAssets.Scripts.Data.Boss.BossType.Bloonarius,
            BossType.Lych => Il2CppAssets.Scripts.Data.Boss.BossType.Lych,
            BossType.Vortex => Il2CppAssets.Scripts.Data.Boss.BossType.Vortex,
            BossType.Dreadbloon => Il2CppAssets.Scripts.Data.Boss.BossType.Dreadbloon,
            BossType.Phayze => Il2CppAssets.Scripts.Data.Boss.BossType.Phayze,
            BossType.Blastapopoulus => Il2CppAssets.Scripts.Data.Boss.BossType.Blastapopoulos,
            BossType.Diamondback => Il2CppAssets.Scripts.Data.Boss.BossType.Diamondback,
            _ => Il2CppAssets.Scripts.Data.Boss.BossType.Bloonarius
        };
    }

    public static void ExecuteSetup()
    {
        try
        {
            var bossType = ConvertToGameBossType(Main.BossName);
            var isElite = Main.Elite;
            var isRanked = Main.Ranked;

            InGameData.Editable.SetupBoss(
                bossType.ToString(),
                bossType,
                isElite,
                isRanked,
                BossGameData.DefaultSpawnRounds,
                new DailyChallengeModel
                {
                    difficulty = InGameData.Editable.selectedDifficulty,
                    map = InGameData.Editable.selectedMap,
                    mode = InGameData.Editable.selectedMode,
                    towers = new TowerData[]
                    {
                        new() { isHero = true, tower = DailyChallengeModel.CHOSENPRIMARYHERO, max = 1 }
                    }.ToIl2CppList(),
                },
                LeaderboardScoringType.GameTime
            );
            
            ModHelper.Msg<Main>("Boss setup successfully applied via Harmony patch.");
        }
        catch (System.Exception e)
        {
            MelonLogger.Error($"Failed to setup boss in CEditorBoss: {e}");
        }
    }
}
