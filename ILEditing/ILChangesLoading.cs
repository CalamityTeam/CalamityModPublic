using System;
using System.Reflection;
using CalamityMod.Graphics.Renderers.CalamityRenderers;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.LunicCorps;
using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.FurnitureExo;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.UI.States;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges : ModSystem
    {
        /// <summary>
        /// Loads all IL Editing changes in the mod.
        /// </summary>
        public override void OnModLoad()
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
            IL_Main.DoDraw += AdditiveDrawing;
            On_Main.DrawGore += DrawForegroundStuff;
            On_Main.DrawCursor += UseCoolFireCursorEffect;
            On_Main.SortDrawCacheWorms += DrawFusableParticles;
            On_Main.DrawInfernoRings += DrawForegroundParticles;
            On_TileDrawing.DrawPartialLiquid += DrawCustomLava;
            On_WaterfallManager.DrawWaterfall_int_int_int_float_Vector2_Rectangle_Color_SpriteEffects += DrawCustomLavafalls;
            IL_LiquidRenderer.DrawNormalLiquids += ChangeWaterQuadColors;
            IL_Main.oldDrawWater += DrawCustomLava3;
            On_TileLightScanner.GetTileLight += MakeSulphSeaWaterBetter;
            On_TileDrawing.PreDrawTiles += ClearForegroundStuff;
            On_TileDrawing.Draw += ClearTilePings;
            On_CommonCode.ModifyItemDropFromNPC += ColorBlightedGel;

            // Graphics (dyeable shader stuff)
            On_Player.UpdateItemDye += DyeableShadersRenderer.FindDyesDetour;
            On_Player.ApplyEquipFunctional += DyeableShadersRenderer.CheckAccessoryDetour;
            On_Player.ApplyEquipVanity_Item += DyeableShadersRenderer.CheckVanityDetour;
            On_Player.UpdateArmorSets += DyeableShadersRenderer.CheckArmorSetsDetour;

            // NPC behavior
            IL_Main.UpdateTime += PermitNighttimeTownNPCSpawning;
            On_Main.UpdateTime_SpawnTownNPCs += AlterTownNPCSpawnRate;
            On_NPC.ShouldEmpressBeEnraged += AllowEmpressToEnrageInBossRush;
            IL_Player.CollectTaxes += MakeTaxCollectorUseful;
            IL_Projectile.Damage += RemoveLunaticCultistHomingResist;

            // Mechanics / features
            On_NPC.ApplyTileCollision += AllowTriggeredFallthrough;
            IL_Player.ApplyEquipFunctional += ScopesRequireVisibilityToZoom;
            IL_Player.Hurt_PlayerDeathReason_int_int_refHurtInfo_bool_bool_int_bool_float_float_float += DodgeMechanicAdjustments;
            IL_Player.DashMovement += FixAllDashMechanics;
            On_Player.DoCommonDashHandle += ApplyDashKeybind;
            IL_Player.GiveImmuneTimeForCollisionAttack += MakeShieldSlamIFramesConsistent;
            IL_Player.Update_NPCCollision += NerfShieldOfCthulhuBonkSafety;
            On_WorldGen.OpenDoor += OpenDoor_LabDoorOverride;
            On_WorldGen.CloseDoor += CloseDoor_LabDoorOverride;
            On_Item.AffixName += IncorporateEnchantmentInAffix;
            On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += IncorporateMinionExplodingCountdown;
            // TODO -- This should be unnecessary. There is now a TML hook for platform collision for ModNPCs.
            On_NPC.Collision_DecideFallThroughPlatforms += EnableCalamityBossPlatformCollision;
            IL_Wiring.HitWireSingle += AddTwinklersToStatue;
            On_Player.UpdateItemDye += FindCalamityItemDyeShader;

            // Mana Burn (Chaos Stone) and Chalice of the Blood God
            IL_Player.ApplyLifeAndOrMana += ManaSicknessAndChaliceBufferHeal;

            // Custom grappling
            On_Player.GrappleMovement += CustomGrappleMovementCheck;
            On_Player.UpdatePettingAnimal += CustomGrapplePreDefaultMovement;
            On_Player.PlayerFrame += CustomGrapplePostFrame;
            On_Player.SlopeDownMovement += CustomGrapplePreStepUp;

            // Damage and health balance
            On_Main.DamageVar_float_int_float += AdjustDamageVariance;
            IL_NPC.ScaleStats_ApplyExpertTweaks += RemoveExpertHardmodeScaling;
            IL_Projectile.AI_001 += AdjustChlorophyteBullets;
            IL_Projectile.AI_099_2 += LimitTerrarianProjectiles;
            IL_Player.UpdateBuffs += NerfSharpeningStation;

            // Movement speed balance
            IL_Player.UpdateJumpHeight += FixJumpHeightBoosts;
            IL_Player.Update += BaseJumpHeightAdjustment;
            IL_Player.Update += RunSpeedAdjustments;
            IL_Player.Update += NerfSoaringInsigniaRunAcceleration;
            IL_Player.WingMovement += RemoveSoaringInsigniaInfiniteWingTime;

            // Life regen balance
            IL_Player.UpdateLifeRegen += PreventWellFedFromBeingRequiredInExpertModeForFullLifeRegen;

            // Mana regen balance
            IL_Player.Update += ManaRegenDelayAdjustment;
            IL_Player.UpdateManaRegen += ManaRegenAdjustment;

            // Debuff balancing
            IL_Projectile.StatusPlayer += RemoveFrozenInflictionFromDeerclopsIceSpikes;

            // World generation
            IL_WorldGen.Pyramid += ReplacePharaohSetInPyramids;
            IL_WorldGen.GrowLivingTree += BlockLivingTreesNearOcean;
            On_WorldGen.SmashAltar += PreventSmashAltarCode;
            IL_WorldGen.hardUpdateWorld += AdjustChlorophyteSpawnRate;
            IL_WorldGen.Chlorophyte += AdjustChlorophyteSpawnLimits;
            IL_UIWorldCreation.SetDefaultOptions += ChangeDefaultWorldSize;
            IL_UIWorldCreation.AddWorldSizeOptions += SwapSmallDescriptionKey;
            Terraria.IO.On_WorldFile.ClearTempTiles += ClearModdedTempTiles;
            On_WorldGen.MakeDungeon += LimitDungeonEntranceXPosition;
            IL_WorldGen.DungeonHalls += LimitDungeonHallsXPosition;
            IL_WorldGen.MakeDungeon += ChangeDungeonSpikeQuantities;

            // Removal of vanilla stupidity
            IL_Player.UpdateBuffs += RemoveFeralBiteRandomDebuffs;
            IL_Sandstorm.HasSufficientWind += DecreaseSandstormWindSpeedRequirement;
            IL_Item.TryGetPrefixStatMultipliersForItem += RelaxPrefixRequirements;
            On_NPC.SlimeRainSpawns += PreventBossSlimeRainSpawns;
            On_ShimmerTransforms.IsItemTransformLocked += AdjustShimmerRequirements;

            // TODO -- Beat Lava Slimes once and for all
            // IL.Terraria.NPC.VanillaHitEffect += RemoveLavaDropsFromExpertLavaSlimes;
            IL_Projectile.CanExplodeTile += MakeMeteoriteExplodable;
            IL_Main.UpdateWindyDayState += MakeWindyDayMusicPlayLessOften;
            IL_Main.UpdateTime_StartNight += BloodMoonsRequire200MaxLife;
            IL_WorldGen.AttemptFossilShattering += PreventFossilShattering;
            On_Player.GetPickaxeDamage += RemoveHellforgePickaxeRequirement;
            On_Player.GetAnglerReward += ImproveAnglerRewards;

            // Fix vanilla bugs exposed by Calamity mechanics
            IL_NPC.NPCLoot += FixSplittingWormBannerDrops;

            // Fix vanilla not accounting for spritebatch modification in held projectile drawing
            On_PlayerDrawLayers.DrawHeldProj += FixHeldProjectileBlendState;

            //Additional detours that are in their own item files given they are only relevant to these specific items:
            //Rover drive detours on Player.DrawInfernoRings to draw its shield
            //Wulfrum armor hooks on Player.KeyDoubleTap and DrawPendingMouseText to activate its set bonus and spoof the mouse text to display the stats of the activated weapon if shift is held
            //HeldOnlyItem detours Player.dropItemCheck, ItemSlot.Draw (Sb, itemarray, int, int, vector2, color) and ItemSlot.LeftClick_ItemArray to make its stuff work
        }

        /// <summary>
        /// Unloads all IL Editing changes in the mod.
        /// </summary>
        public override void OnModUnload()
        {
            // (2023/07) Manually unloading IL hooks is no longer necessary as TML should automatically do it for you.
            // https://discord.com/channels/103110554649894912/445276626352209920/1099856743820959855 (links to TML server)

            VanillaSpawnTownNPCs = null;
            labDoorOpen = labDoorClosed = aLabDoorOpen = aLabDoorClosed = exoDoorClosed = exoDoorOpen = -1;
        }
    }
}
