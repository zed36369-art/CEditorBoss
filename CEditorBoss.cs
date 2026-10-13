using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using CEditorBoss;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors.Actions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;

[assembly: MelonInfo(typeof(CEditorBoss.Main), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace CEditorBoss;

public enum BossType
{
    Bloonarius,
    Vortex,
    Dreadbloon,
    Lych,
    Phayze,
    Blastapopoulus,
    Diamondback
}

public class Main : BloonsTD6Mod
{
    private static bool settingsDirty = false;

    public static readonly ModSettingEnum<BossType> BossName = new(BossType.Bloonarius)
    {
        displayName = "Boss Type",
        onValueChanged = (value) => settingsDirty = true
    };

    public static ModSettingBool Elite = new(false)
    {
        displayName = "Elite",
        description = "Check if Boss should be elite or not",
        onValueChanged = (value) => settingsDirty = true
    };

    public static ModSettingBool Ranked = new(false)
    {
        displayName = "Ranked",
        description = "Check if Boss should be ranked or not",
        onValueChanged = (value) => settingsDirty = true
    };

    public static ModSettingDouble BossHealthMultiplier = new ModSettingDouble(100.0)
    {
        displayName = "Boss Health Multiplier",
        description = "Set the Percentage",
        onValueChanged = (value) => settingsDirty = true
    };

    public static ModSettingDouble BossSpeedMultiplier = new ModSettingDouble(100.0)
    {
        displayName = "Boss Speed Multiplier",
        description = "Set the Percentage",
        onValueChanged = (value) => settingsDirty = true
    };

    public override void OnApplicationStart()
    {
        ModHelper.Msg<Main>("CEditorBoss loaded!");
    }

    public override void OnUpdate()
    {
        if (!settingsDirty) return;
        
        try
        {
            if (Game.instance?.playerService?.Player?.Data == null) return;
            var challengeEditorModel = Game.instance.playerService.Player.Data.challengeEditorModel;
            if (challengeEditorModel == null) return;

            challengeEditorModel.roundSets.Clear();
            var bossStr = BossName?.GetValue()?.ToString()?.ToLower();
            if (!string.IsNullOrEmpty(bossStr))
            {
                challengeEditorModel.roundSets.Add(bossStr);
            }

            if (challengeEditorModel.bloonModifiers != null)
            {
                challengeEditorModel.bloonModifiers.healthMultipliers.boss = (float)(BossHealthMultiplier / 100.0);
                challengeEditorModel.bloonModifiers.bossSpeedMultiplier = (float)(BossSpeedMultiplier / 100.0);
            }
            
            challengeEditorModel.startRules.endRound = 140;
            settingsDirty = false;
        }
        catch (System.Exception e)
        {
            // 如果出错，将异常打印到 MelonLoader 日志中以便排查
            MelonLogger.Error($"Error in CEditorBoss OnUpdate: {e}");
            settingsDirty = false;
        }
    }

    public override void OnMatchStart()
    {
        try
        {
            var multiplier = (float)(BossHealthMultiplier / 100.0);
            var model = InGame.Bridge?.Model;
            if (model == null) return;

            foreach (var bloon in model.bloons)
            {
                if (bloon == null || !bloon.isBoss) continue;

                foreach (var armour in bloon.GetBehaviors<GenerateArmourActionModel>())
                    armour.amount *= multiplier;

                foreach (var shield in bloon.GetBehaviors<GenerateShieldActionModel>())
                    shield.amount *= multiplier;
            }
        }
        catch (System.Exception e)
        {
            MelonLogger.Error($"Error in CEditorBoss OnMatchStart: {e}");
        }
    }
}
