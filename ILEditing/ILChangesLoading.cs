using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.FurnitureExo;
using System;
using System.Collections.Generic;
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

            // Re-initialize the projectile cache list.
            OrderedProjectiles = new List<OrderedProjectileEntry>();

            // Mechanics / features
            On.Terraria.NPC.ApplyTileCollision += AllowTriggeredFallthrough;
            On.Terraria.NPC.ShouldEmpressBeEnraged += AllowEmpressToEnrageInBossRush;
            IL.Terraria.Main.UpdateTime += PermitNighttimeTownNPCSpawning;
            On.Terraria.Main.UpdateTime_SpawnTownNPCs += AlterTownNPCSpawnRate;
            IL.Terraria.Player.Hurt += RemoveRNGFromDodges;
            IL.Terraria.Player.DashMovement += FixAllDashMechanics;
            IL.Terraria.Player.GiveImmuneTimeForCollisionAttack += MakeShieldSlamIFramesConsistent;
            IL.Terraria.Player.Update_NPCCollision += NerfShieldOfCthulhuBonkSafety;
            On.Terraria.WorldGen.OpenDoor += OpenDoor_LabDoorOverride;
            On.Terraria.WorldGen.CloseDoor += CloseDoor_LabDoorOverride;
            IL.Terraria.Main.DrawInterface_40_InteractItemIcon += MakeMouseHoverItemsSupportAnimations;
            On.Terraria.Item.AffixName += IncorporateEnchantmentInAffix;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float += IncorporateMinionExplodingCountdown;
            On.Terraria.Main.DrawCursor += UseCoolFireCursorEffect;
            IL.Terraria.Player.QuickHeal += ConditionallyReplaceManaSickness;
            IL.Terraria.Player.QuickMana += ConditionallyReplaceManaSickness;
            IL.Terraria.Player.ItemCheck_Inner += ConditionallyReplaceManaSickness;
            IL.Terraria.Main.DoDraw += AdditiveDrawing;
            On.Terraria.Main.SortDrawCacheWorms += DrawFusableParticles;
            On.Terraria.Main.DrawInfernoRings += DrawForegroundParticles;
            On.Terraria.Main.SetDisplayMode += ResetRenderTargetSizes;
            IL.Terraria.GameContent.Drawing.TileDrawing.DrawPartialLiquid += DrawCustomLava;
            IL.Terraria.GameContent.Liquid.LiquidRenderer.InternalDraw += DrawCustomLava2;
            IL.Terraria.Main.oldDrawWater += DrawCustomLava3;
            IL.Terraria.Player.CollectTaxes += MakeTaxCollectorUseful;
            On.Terraria.Main.DrawGore += DrawForegroundStuff;
            On.Terraria.GameContent.Drawing.TileDrawing.PreDrawTiles += ClearForegroundStuff;
            On.Terraria.GameContent.Drawing.TileDrawing.Draw += ClearTilePings;
            On.Terraria.Player.GrappleMovement += CustomGrappleMovementCheck;
            On.Terraria.Player.UpdatePettingAnimal += CustomGrapplePreDefaultMovement;
            On.Terraria.Player.PlayerFrame += CustomGrapplePostFrame;
            On.Terraria.Player.SlopeDownMovement += CustomGrapplePreStepUp;

            // TODO -- Revisit this. It's not an extremely important thing, but it'd be ideal to not just abandon it.
            // IL.Terraria.WaterfallManager.DrawWaterfall += DrawCustomLavafalls;
            // TODO -- This should be unnecessary. There is now a TML hook for platform collision for ModNPCs.
            On.Terraria.NPC.Collision_DecideFallThroughPlatforms += EnableCalamityBossPlatformCollision;
            IL.Terraria.Wiring.HitWireSingle += AddTwinklersToStatue;

            // Damage and health balance
            IL.Terraria.Main.DamageVar += AdjustDamageVariance;
            IL.Terraria.NPC.ScaleStats_ApplyExpertTweaks += RemoveExpertHardmodeScaling;
            IL.Terraria.Projectile.AI_001 += AdjustChlorophyteBullets;

            // Movement speed balance
            IL.Terraria.Player.UpdateJumpHeight += FixJumpHeightBoosts;
            IL.Terraria.Player.Update += BaseJumpHeightAdjustment;
            IL.Terraria.Player.Update += RunSpeedAdjustments;
            IL.Terraria.Initializers.WingStatsInitializer.Load += ReduceWingHoverVelocities;
            IL.Terraria.Player.Update += NerfMagiluminescence;
            IL.Terraria.Player.Update += NerfSoaringInsigniaRunAcceleration;
            IL.Terraria.Player.WingMovement += RemoveSoaringInsigniaInfiniteWingTime;

            // Damage balance
            IL.Terraria.Player.UpdateBuffs += NerfSharpeningStation;

            // Life regen balance
            IL.Terraria.Player.UpdateLifeRegen += PreventWellFedFromBeingRequiredInExpertModeForFullLifeRegen;

            // Mana regen balance
            IL.Terraria.Player.Update += ManaRegenDelayAdjustment;
            IL.Terraria.Player.UpdateManaRegen += ManaRegenAdjustment;

            // World generation
            IL.Terraria.WorldGen.Pyramid += ReplacePharaohSetInPyramids;
            IL.Terraria.WorldGen.MakeDungeon += PreventDungeonHorizontalCollisions;
            IL.Terraria.WorldGen.DungeonHalls += PreventDungeonHallCollisions;
            IL.Terraria.WorldGen.GrowLivingTree += BlockLivingTreesNearOcean;
            On.Terraria.WorldGen.SmashAltar += PreventSmashAltarCode;
            IL.Terraria.WorldGen.hardUpdateWorld += AdjustChlorophyteSpawnRate;
            IL.Terraria.WorldGen.Chlorophyte += AdjustChlorophyteSpawnLimits;
            IL.Terraria.GameContent.UI.States.UIWorldCreation.SetDefaultOptions += ChangeDefaultWorldSize;
            IL.Terraria.GameContent.UI.States.UIWorldCreation.AddWorldSizeOptions += SwapSmallDescriptionKey;
            On.Terraria.IO.WorldFile.ClearTempTiles += ClearModdedTempTiles;

            // Removal of vanilla stupidity
            IL.Terraria.GameContent.Events.Sandstorm.HasSufficientWind += DecreaseSandstormWindSpeedRequirement;
            IL.Terraria.Item.Prefix += RelaxPrefixRequirements;
            On.Terraria.NPC.SlimeRainSpawns += PreventBossSlimeRainSpawns;
            IL.Terraria.NPC.SpawnNPC += MakeVoodooDemonDollWork;
            // TODO -- Beat Lava Slimes once and for all
            // IL.Terraria.NPC.VanillaHitEffect += RemoveLavaDropsFromExpertLavaSlimes;
            IL.Terraria.Player.IsTileTypeInInteractionRange += IncreasePylonInteractionRange;
            IL.Terraria.Projectile.CanExplodeTile += MakeMeteoriteExplodable;
            IL.Terraria.Main.UpdateWindyDayState += MakeWindyDayMusicPlayLessOften;
            IL.Terraria.Main.UpdateTime_StartNight += BloodMoonsRequire200MaxLife;
            IL.Terraria.WorldGen.AttemptFossilShattering += PreventFossilShattering;
            IL.Terraria.Projectile.Damage += MakeTagDamageMultiplicative;
            On.Terraria.Player.GetPickaxeDamage += RemoveHellforgePickaxeRequirement;
            On.Terraria.GameContent.ItemDropRules.CommonCode.ModifyItemDropFromNPC += ColorBlightedGel;

            // Fix vanilla bugs exposed by Calamity mechanics
            IL.Terraria.NPC.NPCLoot += FixSplittingWormBannerDrops;
            On.Terraria.Item.Prefix += LetRogueItemsBeReforgeable;
            // IL.Terraria.Main.DoUpdate += FixProjectileUpdatePriorityProblems;


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

            // Mechanics / features
            On.Terraria.NPC.ApplyTileCollision -= AllowTriggeredFallthrough;
            On.Terraria.NPC.ShouldEmpressBeEnraged -= AllowEmpressToEnrageInBossRush;
            IL.Terraria.Main.UpdateTime -= PermitNighttimeTownNPCSpawning;
            On.Terraria.Main.UpdateTime_SpawnTownNPCs -= AlterTownNPCSpawnRate;
            IL.Terraria.Player.Hurt -= RemoveRNGFromDodges;
            IL.Terraria.Player.DashMovement -= FixAllDashMechanics;
            IL.Terraria.Player.GiveImmuneTimeForCollisionAttack -= MakeShieldSlamIFramesConsistent;
            IL.Terraria.Player.Update_NPCCollision -= NerfShieldOfCthulhuBonkSafety;
            On.Terraria.WorldGen.OpenDoor -= OpenDoor_LabDoorOverride;
            On.Terraria.WorldGen.CloseDoor -= CloseDoor_LabDoorOverride;
            IL.Terraria.Main.DrawInterface_40_InteractItemIcon -= MakeMouseHoverItemsSupportAnimations;
            On.Terraria.Item.AffixName -= IncorporateEnchantmentInAffix;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float -= IncorporateMinionExplodingCountdown;
            On.Terraria.Main.DrawCursor -= UseCoolFireCursorEffect;
            IL.Terraria.Player.QuickHeal -= ConditionallyReplaceManaSickness;
            IL.Terraria.Player.QuickMana -= ConditionallyReplaceManaSickness;
            IL.Terraria.Player.ItemCheck_Inner -= ConditionallyReplaceManaSickness;
            IL.Terraria.Main.DoDraw -= AdditiveDrawing;
            On.Terraria.Main.SortDrawCacheWorms -= DrawFusableParticles;
            On.Terraria.Main.DrawInfernoRings -= DrawForegroundParticles;
            On.Terraria.Main.SetDisplayMode -= ResetRenderTargetSizes;
            IL.Terraria.GameContent.Drawing.TileDrawing.DrawPartialLiquid -= DrawCustomLava;
            IL.Terraria.GameContent.Liquid.LiquidRenderer.InternalDraw -= DrawCustomLava2;
            IL.Terraria.Main.oldDrawWater -= DrawCustomLava3;
            IL.Terraria.Player.CollectTaxes -= MakeTaxCollectorUseful;
            // IL.Terraria.WaterfallManager.DrawWaterfall -= DrawCustomLavafalls;
            On.Terraria.NPC.Collision_DecideFallThroughPlatforms -= EnableCalamityBossPlatformCollision;
            IL.Terraria.Wiring.HitWireSingle -= AddTwinklersToStatue;
            On.Terraria.Main.DrawGore -= DrawForegroundStuff;
            On.Terraria.GameContent.Drawing.TileDrawing.PreDrawTiles -= ClearForegroundStuff;
            On.Terraria.GameContent.Drawing.TileDrawing.Draw -= ClearTilePings;
            On.Terraria.Player.GrappleMovement -= CustomGrappleMovementCheck;
            On.Terraria.Player.UpdatePettingAnimal -= CustomGrapplePreDefaultMovement;
            On.Terraria.Player.PlayerFrame -= CustomGrapplePostFrame;
            On.Terraria.Player.SlopeDownMovement -= CustomGrapplePreStepUp;

            // Damage and health balance
            IL.Terraria.Main.DamageVar -= AdjustDamageVariance;
            IL.Terraria.NPC.ScaleStats_ApplyExpertTweaks -= RemoveExpertHardmodeScaling;
            IL.Terraria.Projectile.AI_001 -= AdjustChlorophyteBullets;

            // Movement speed balance
            IL.Terraria.Player.UpdateJumpHeight -= FixJumpHeightBoosts;
            IL.Terraria.Player.Update -= BaseJumpHeightAdjustment;
            IL.Terraria.Player.Update -= RunSpeedAdjustments;
            IL.Terraria.Initializers.WingStatsInitializer.Load -= ReduceWingHoverVelocities;
            IL.Terraria.Player.Update -= NerfMagiluminescence;
            IL.Terraria.Player.Update -= NerfSoaringInsigniaRunAcceleration;
            IL.Terraria.Player.WingMovement -= RemoveSoaringInsigniaInfiniteWingTime;

            // Damage balance
            IL.Terraria.Player.UpdateBuffs -= NerfSharpeningStation;

            // Life regen balance
            IL.Terraria.Player.UpdateLifeRegen -= PreventWellFedFromBeingRequiredInExpertModeForFullLifeRegen;

            // Mana regen balance
            IL.Terraria.Player.Update -= ManaRegenDelayAdjustment;
            IL.Terraria.Player.UpdateManaRegen -= ManaRegenAdjustment;

            // World generation
            IL.Terraria.WorldGen.Pyramid -= ReplacePharaohSetInPyramids;
            IL.Terraria.WorldGen.MakeDungeon -= PreventDungeonHorizontalCollisions;
            IL.Terraria.WorldGen.DungeonHalls -= PreventDungeonHallCollisions;
            IL.Terraria.WorldGen.GrowLivingTree -= BlockLivingTreesNearOcean;
            On.Terraria.WorldGen.SmashAltar -= PreventSmashAltarCode;
            IL.Terraria.WorldGen.hardUpdateWorld -= AdjustChlorophyteSpawnRate;
            IL.Terraria.WorldGen.Chlorophyte -= AdjustChlorophyteSpawnLimits;
            IL.Terraria.GameContent.UI.States.UIWorldCreation.SetDefaultOptions -= ChangeDefaultWorldSize;
            IL.Terraria.GameContent.UI.States.UIWorldCreation.AddWorldSizeOptions -= SwapSmallDescriptionKey;
            On.Terraria.IO.WorldFile.ClearTempTiles -= ClearModdedTempTiles;

            // Removal of vanilla stupidity
            IL.Terraria.GameContent.Events.Sandstorm.HasSufficientWind -= DecreaseSandstormWindSpeedRequirement;
            IL.Terraria.Item.Prefix -= RelaxPrefixRequirements;
            On.Terraria.NPC.SlimeRainSpawns -= PreventBossSlimeRainSpawns;
            IL.Terraria.NPC.SpawnNPC -= MakeVoodooDemonDollWork;
            //IL.Terraria.NPC.VanillaHitEffect -= RemoveLavaDropsFromExpertLavaSlimes;
            IL.Terraria.Player.IsTileTypeInInteractionRange -= IncreasePylonInteractionRange;
            IL.Terraria.Projectile.CanExplodeTile -= MakeMeteoriteExplodable;
            IL.Terraria.Main.UpdateWindyDayState -= MakeWindyDayMusicPlayLessOften;
            IL.Terraria.Main.UpdateTime_StartNight -= BloodMoonsRequire200MaxLife;
            IL.Terraria.WorldGen.AttemptFossilShattering -= PreventFossilShattering;
            IL.Terraria.Projectile.Damage -= MakeTagDamageMultiplicative;
            On.Terraria.Player.GetPickaxeDamage -= RemoveHellforgePickaxeRequirement;
            On.Terraria.GameContent.ItemDropRules.CommonCode.ModifyItemDropFromNPC -= ColorBlightedGel;

            // Fix vanilla bugs exposed by Calamity mechanics
            IL.Terraria.NPC.NPCLoot -= FixSplittingWormBannerDrops;
            On.Terraria.Item.Prefix -= LetRogueItemsBeReforgeable;
            // IL.Terraria.Main.DoUpdate -= FixProjectileUpdatePriorityProblems;
        }
    }
}
