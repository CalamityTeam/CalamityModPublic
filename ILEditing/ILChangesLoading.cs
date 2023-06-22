using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.FurnitureExo;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public partial class ILChanges
    {
        /// <summary>
        /// Loads all IL Editing changes in the mod.
        /// </summary>
        internal static void Load()
        {
            // Wrap the vanilla town NPC spawning function in a delegate so that it can be tossed around and called at will.
            var updateTime = typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.Static | BindingFlags.NonPublic);
            VanillaSpawnTownNPCs = Delegate.CreateDelegate(typeof(Action), updateTime) as Action;

            // Cache the six lab door tile types for efficiency.
            labDoorOpen = ModContent.TileType<LaboratoryDoorOpen>();
            labDoorClosed = ModContent.TileType<LaboratoryDoorClosed>();
            aLabDoorOpen = ModContent.TileType<AgedLaboratoryDoorOpen>();
            aLabDoorClosed = ModContent.TileType<AgedLaboratoryDoorClosed>();
            exoDoorOpen = ModContent.TileType<ExoDoorOpen>();
            exoDoorClosed = ModContent.TileType<ExoDoorClosed>();

            // Graphics
            Terraria.IL_Main.DoDraw += AdditiveDrawing;
            Terraria.On_Main.DrawGore += DrawForegroundStuff;
            Terraria.On_Main.DrawCursor += UseCoolFireCursorEffect;
            Terraria.On_Main.SetDisplayMode += ResetRenderTargetSizes;
            Terraria.On_Main.SortDrawCacheWorms += DrawFusableParticles;
            Terraria.On_Main.DrawInfernoRings += DrawForegroundParticles;
            
            // ERROR
            //Terraria.GameContent.Drawing.On_TileDrawing.DrawPartialLiquid += DrawCustomLava;
            // ERROR
            //Terraria.IL_WaterfallManager.DrawWaterfall += DrawCustomLavafalls;
            // ERROR
            //Terraria.GameContent.Liquid.IL_LiquidRenderer.InternalDraw += ChangeWaterQuadColors;
            // ERROR
            //Terraria.IL_Main.oldDrawWater += DrawCustomLava3;
            
            Terraria.Graphics.Light.On_TileLightScanner.GetTileLight += MakeSulphSeaWaterBetter;
            Terraria.GameContent.Drawing.On_TileDrawing.PreDrawTiles += ClearForegroundStuff;
            Terraria.GameContent.Drawing.On_TileDrawing.Draw += ClearTilePings;
            Terraria.GameContent.ItemDropRules.On_CommonCode.ModifyItemDropFromNPC += ColorBlightedGel;

            // NPC behavior
            Terraria.IL_Main.UpdateTime += PermitNighttimeTownNPCSpawning;
            Terraria.On_Main.UpdateTime_SpawnTownNPCs += AlterTownNPCSpawnRate;
            Terraria.On_NPC.ShouldEmpressBeEnraged += AllowEmpressToEnrageInBossRush;
            Terraria.IL_Player.CollectTaxes += MakeTaxCollectorUseful;
            Terraria.IL_Projectile.Damage += RemoveLunaticCultistHomingResist;

            // Mechanics / features
            Terraria.On_NPC.ApplyTileCollision += AllowTriggeredFallthrough;
            Terraria.IL_Player.ApplyEquipFunctional += ScopesRequireVisibilityToZoom;
            Terraria.On_Player.Hurt_PlayerDeathReason_int_int_bool_bool_int_bool_float_float_float += RemoveRNGFromDodges;
            Terraria.IL_Player.DashMovement += FixAllDashMechanics;
            Terraria.On_Player.DoCommonDashHandle += ApplyDashKeybind;
            Terraria.IL_Player.GiveImmuneTimeForCollisionAttack += MakeShieldSlamIFramesConsistent;
            Terraria.IL_Player.Update_NPCCollision += NerfShieldOfCthulhuBonkSafety;
            Terraria.On_WorldGen.OpenDoor += OpenDoor_LabDoorOverride;
            Terraria.On_WorldGen.CloseDoor += CloseDoor_LabDoorOverride;
            Terraria.On_Item.AffixName += IncorporateEnchantmentInAffix;
            Terraria.On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += IncorporateMinionExplodingCountdown;
            // TODO -- This should be unnecessary. There is now a TML hook for platform collision for ModNPCs.
            Terraria.On_NPC.Collision_DecideFallThroughPlatforms += EnableCalamityBossPlatformCollision;
            Terraria.IL_Wiring.HitWireSingle += AddTwinklersToStatue;
            Terraria.On_Player.UpdateItemDye += FindCalamityItemDyeShader;

            // Mana Burn
            Terraria.IL_Player.ApplyLifeAndOrMana += ConditionallyReplaceManaSickness;

            // Custom grappling
            Terraria.On_Player.GrappleMovement += CustomGrappleMovementCheck;
            Terraria.On_Player.UpdatePettingAnimal += CustomGrapplePreDefaultMovement;
            Terraria.On_Player.PlayerFrame += CustomGrapplePostFrame;
            Terraria.On_Player.SlopeDownMovement += CustomGrapplePreStepUp;

            // Damage and health balance
            Terraria.On_Main.DamageVar_float_int_float += AdjustDamageVariance;            
            Terraria.IL_Projectile.Damage += MakeTagDamageMultiplicative;
            Terraria.IL_NPC.ScaleStats_ApplyExpertTweaks += RemoveExpertHardmodeScaling;
            Terraria.IL_Projectile.AI_001 += AdjustChlorophyteBullets;
            Terraria.IL_Player.UpdateBuffs += NerfSharpeningStation;

            // Movement speed balance
            Terraria.IL_Player.UpdateJumpHeight += FixJumpHeightBoosts;
            Terraria.IL_Player.Update += BaseJumpHeightAdjustment;
            Terraria.IL_Player.Update += RunSpeedAdjustments;
            Terraria.IL_Player.Update += NerfMagiluminescence;
            Terraria.IL_Player.Update += NerfSoaringInsigniaRunAcceleration;
            Terraria.IL_Player.WingMovement += RemoveSoaringInsigniaInfiniteWingTime;

            // Life regen balance
            Terraria.IL_Player.UpdateLifeRegen += PreventWellFedFromBeingRequiredInExpertModeForFullLifeRegen;

            // Mana regen balance
            Terraria.IL_Player.Update += ManaRegenDelayAdjustment;
            Terraria.IL_Player.UpdateManaRegen += ManaRegenAdjustment;

            // World generation
            Terraria.IL_WorldGen.Pyramid += ReplacePharaohSetInPyramids;
            Terraria.IL_WorldGen.GrowLivingTree += BlockLivingTreesNearOcean;
            Terraria.On_WorldGen.SmashAltar += PreventSmashAltarCode;
            Terraria.IL_WorldGen.hardUpdateWorld += AdjustChlorophyteSpawnRate;
            Terraria.IL_WorldGen.Chlorophyte += AdjustChlorophyteSpawnLimits;
            Terraria.GameContent.UI.States.IL_UIWorldCreation.SetDefaultOptions += ChangeDefaultWorldSize;
            Terraria.GameContent.UI.States.IL_UIWorldCreation.AddWorldSizeOptions += SwapSmallDescriptionKey;
            Terraria.IO.On_WorldFile.ClearTempTiles += ClearModdedTempTiles;

            // Removal of vanilla stupidity
            Terraria.IL_Player.UpdateBuffs += RemoveFeralBiteRandomDebuffs;
            Terraria.GameContent.Events.IL_Sandstorm.HasSufficientWind += DecreaseSandstormWindSpeedRequirement;
            Terraria.IL_Item.TryGetPrefixStatMultipliersForItem += RelaxPrefixRequirements;
            Terraria.On_NPC.SlimeRainSpawns += PreventBossSlimeRainSpawns;
            
            // ERROR
            //Terraria.IL_NPC.SpawnNPC += MakeVoodooDemonDollWork;
            
            // TODO -- Beat Lava Slimes once and for all
            // IL.Terraria.NPC.VanillaHitEffect += RemoveLavaDropsFromExpertLavaSlimes;
            Terraria.IL_Projectile.CanExplodeTile += MakeMeteoriteExplodable;
            Terraria.IL_Main.UpdateWindyDayState += MakeWindyDayMusicPlayLessOften;
            Terraria.IL_Main.UpdateTime_StartNight += BloodMoonsRequire200MaxLife;
            Terraria.IL_WorldGen.AttemptFossilShattering += PreventFossilShattering;
            Terraria.On_Player.GetPickaxeDamage += RemoveHellforgePickaxeRequirement;
            Terraria.On_Player.GetAnglerReward += ImproveAnglerRewards;

            // Fix vanilla bugs exposed by Calamity mechanics
            Terraria.IL_NPC.NPCLoot += FixSplittingWormBannerDrops;

            //Additional detours that are in their own item files given they are only relevant to these specific items:
            //Rover drive detours on Player.DrawInfernoRings to draw its shield
            //Wulfrum armor hooks on Player.KeyDoubleTap and DrawPendingMouseText to activate its set bonus and spoof the mouse text to display the stats of the activated weapon if shift is held
            //HeldOnlyItem detours Player.dropItemCheck, ItemSlot.Draw (Sb, itemarray, int, int, vector2, color) and ItemSlot.LeftClick_ItemArray to make its stuff work
        }

        /// <summary>
        /// Unloads all IL Editing changes in the mod.
        /// </summary>
        internal static void Unload()
        {
            VanillaSpawnTownNPCs = null;
            labDoorOpen = labDoorClosed = aLabDoorOpen = aLabDoorClosed = exoDoorClosed = exoDoorOpen = -1;
        }
    }
}
