using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.FurnitureExo;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
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
            IL.Terraria.Main.UpdateTime += PermitNighttimeTownNPCSpawning;
            On.Terraria.Main.UpdateTime_SpawnTownNPCs += AlterTownNPCSpawnRate;
            IL.Terraria.Player.Hurt += RemoveRNGFromBlackBelt;
            IL.Terraria.Player.DashMovement += FixVanillaShieldSlams;
            IL.Terraria.Player.Update_NPCCollision += NerfShieldOfCthulhuBonkSafety;
            On.Terraria.WorldGen.OpenDoor += OpenDoor_LabDoorOverride;
            On.Terraria.WorldGen.CloseDoor += CloseDoor_LabDoorOverride;
            On.Terraria.Wiring.Teleport += DisableTeleporters; // only applies in boss rush
            IL.Terraria.Main.DrawInterface_40_InteractItemIcon += MakeMouseHoverItemsSupportAnimations;
            On.Terraria.Item.AffixName += IncorporateEnchantmentInAffix;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float += IncorporateMinionExplodingCountdown;
            On.Terraria.Main.DrawCursor += UseCoolFireCursorEffect;
            IL.Terraria.Player.QuickHeal += ApplyManaBurnIfNeeded;
            IL.Terraria.Player.QuickMana += ApplyManaBurnIfNeeded;
            IL.Terraria.Player.ItemCheck += ApplyManaBurnIfNeeded;
            IL.Terraria.Player.AddBuff += AllowBuffTimeStackingForManaBurn;
            On.Terraria.Main.DrawInterface += DrawGeneralParticles;
            On.Terraria.Main.SortDrawCacheWorms += DrawFusableParticles;
            On.Terraria.Main.SetDisplayMode += ResetRenderTargetSizes;
            IL.Terraria.Main.DrawTiles += DrawCustomLava;
            IL.Terraria.GameContent.Liquid.LiquidRenderer.InternalDraw += DrawCustomLava2;
            IL.Terraria.Main.oldDrawWater += DrawCustomLava3;

            // TODO -- Revisit this. It's not an extremely important thing, but it'd be ideal to not just abandon it.
            // IL.Terraria.WaterfallManager.DrawWaterfall += DrawCustomLavafalls;
            On.Terraria.NPC.Collision_DecideFallThroughPlatforms += EnableCalamityBossPlatformCollision;
            IL.Terraria.Wiring.HitWireSingle += AddTwinklersToStatue;

            // Damage and health balance
            IL.Terraria.Main.DamageVar += AdjustDamageVariance;
            IL.Terraria.NPC.ScaleStats += RemoveExpertHardmodeScaling;
            IL.Terraria.Projectile.AI_001 += AdjustChlorophyteBullets;

            // Movement speed balance
            IL.Terraria.Player.UpdateJumpHeight += FixJumpHeightBoosts;
            IL.Terraria.Player.Update += JumpSpeedAdjustment;
            IL.Terraria.Player.Update += MaxRunSpeedAdjustment;
            IL.Terraria.Player.Update += RunSpeedAdjustments;
            IL.Terraria.Player.Update += ReduceWingHoverVelocities;
            IL.Terraria.Player.Update += NerfMagiluminescence;
            IL.Terraria.Player.Update += NerfSoaringInsigniaRunAcceleration;
            IL.Terraria.Player.WingMovement += RemoveSoaringInsigniaInfiniteWingTime;

            // Mana regen balance
            IL.Terraria.Player.Update += ManaRegenDelayAdjustment;
            IL.Terraria.Player.UpdateManaRegen += ManaRegenAdjustment;
            IL.Terraria.Player.ApplyEquipFunctional += DecreaseMagnetFlowerAndArcaneFlowerManaCost;

            // World generation
            IL.Terraria.WorldGen.Pyramid += ReplacePharaohSetInPyramids;
            IL.Terraria.WorldGen.MakeDungeon += PreventDungeonHorizontalCollisions;
            IL.Terraria.WorldGen.DungeonHalls += PreventDungeonHallCollisions;
            IL.Terraria.WorldGen.GrowLivingTree += BlockLivingTreesNearOcean;
            On.Terraria.WorldGen.SmashAltar += PreventSmashAltarCode;
            IL.Terraria.WorldGen.hardUpdateWorld += AdjustChlorophyteSpawnRate;
            IL.Terraria.WorldGen.Chlorophyte += AdjustChlorophyteSpawnLimits;

            // Removal of vanilla stupidity
            IL.Terraria.Item.Prefix += RelaxPrefixRequirements;
            On.Terraria.NPC.SlimeRainSpawns += PreventBossSlimeRainSpawns;
            IL.Terraria.NPC.SpawnNPC += MakeVoodooDemonDollWork;
            // TODO -- Beat Lava Slimes once and for all
            //IL.Terraria.NPC.VanillaHitEffect += RemoveLavaDropsFromExpertLavaSlimes;
            IL.Terraria.Main.UpdateTime += BloodMoonsRequire200MaxLife;

            // Fix vanilla bugs exposed by Calamity mechanics
            // On.Terraria.Main.InitLifeBytes += BossRushLifeBytes;
            IL.Terraria.NPC.NPCLoot += FixSplittingWormBannerDrops;
            // IL.Terraria.Main.DoUpdate += FixProjectileUpdatePriorityProblems;
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
            IL.Terraria.Main.UpdateTime -= PermitNighttimeTownNPCSpawning;
            On.Terraria.Main.UpdateTime_SpawnTownNPCs -= AlterTownNPCSpawnRate;
            IL.Terraria.Player.Hurt -= RemoveRNGFromBlackBelt;
            IL.Terraria.Player.DashMovement -= FixVanillaShieldSlams;
            IL.Terraria.Player.Update_NPCCollision -= NerfShieldOfCthulhuBonkSafety;
            On.Terraria.WorldGen.OpenDoor -= OpenDoor_LabDoorOverride;
            On.Terraria.WorldGen.CloseDoor -= CloseDoor_LabDoorOverride;
            On.Terraria.Wiring.Teleport -= DisableTeleporters;
            IL.Terraria.Main.DrawInterface_40_InteractItemIcon -= MakeMouseHoverItemsSupportAnimations;
            On.Terraria.Item.AffixName -= IncorporateEnchantmentInAffix;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float -= IncorporateMinionExplodingCountdown;
            On.Terraria.Main.DrawCursor -= UseCoolFireCursorEffect;
            IL.Terraria.Player.QuickHeal -= ApplyManaBurnIfNeeded;
            IL.Terraria.Player.QuickMana -= ApplyManaBurnIfNeeded;
            IL.Terraria.Player.ItemCheck -= ApplyManaBurnIfNeeded;
            IL.Terraria.Player.AddBuff -= AllowBuffTimeStackingForManaBurn;
            On.Terraria.Main.DrawInterface -= DrawGeneralParticles;
            On.Terraria.Main.SortDrawCacheWorms -= DrawFusableParticles;
            On.Terraria.Main.SetDisplayMode -= ResetRenderTargetSizes;
            IL.Terraria.Main.DrawTiles -= DrawCustomLava;
            IL.Terraria.GameContent.Liquid.LiquidRenderer.InternalDraw -= DrawCustomLava2;
            IL.Terraria.Main.oldDrawWater -= DrawCustomLava3;
            IL.Terraria.WaterfallManager.DrawWaterfall -= DrawCustomLavafalls;
            On.Terraria.NPC.Collision_DecideFallThroughPlatforms -= EnableCalamityBossPlatformCollision;
            IL.Terraria.Wiring.HitWireSingle -= AddTwinklersToStatue;

            // Damage and health balance
            IL.Terraria.Main.DamageVar -= AdjustDamageVariance;
            IL.Terraria.NPC.ScaleStats -= RemoveExpertHardmodeScaling;
            IL.Terraria.Projectile.AI_001 -= AdjustChlorophyteBullets;

            // Movement speed balance
            //IL.Terraria.Player.WingMovement -= RemoveSoaringInsigniaInfiniteWingTime;
            IL.Terraria.Player.UpdateJumpHeight -= FixJumpHeightBoosts;
            //IL.Terraria.Player.Update -= NerfSoaringInsigniaRunAcceleration;
            IL.Terraria.Player.Update -= NerfMagiluminescence;
            IL.Terraria.Player.Update -= JumpSpeedAdjustment;
            IL.Terraria.Player.Update -= MaxRunSpeedAdjustment;
            IL.Terraria.Player.Update -= RunSpeedAdjustments;
            IL.Terraria.Player.Update -= ReduceWingHoverVelocities;

            // Mana regen balance
            IL.Terraria.Player.Update -= ManaRegenDelayAdjustment;
            IL.Terraria.Player.UpdateManaRegen -= ManaRegenAdjustment;
            IL.Terraria.Player.ApplyEquipFunctional -= DecreaseMagnetFlowerAndArcaneFlowerManaCost;

            // World generation
            IL.Terraria.WorldGen.Pyramid -= ReplacePharaohSetInPyramids;
            IL.Terraria.WorldGen.MakeDungeon -= PreventDungeonHorizontalCollisions;
            IL.Terraria.WorldGen.DungeonHalls -= PreventDungeonHallCollisions;
            IL.Terraria.WorldGen.GrowLivingTree -= BlockLivingTreesNearOcean;
            On.Terraria.WorldGen.SmashAltar -= PreventSmashAltarCode;
            IL.Terraria.WorldGen.hardUpdateWorld -= AdjustChlorophyteSpawnRate;
            IL.Terraria.WorldGen.Chlorophyte -= AdjustChlorophyteSpawnLimits;

            // Removal of vanilla stupidity
            IL.Terraria.Item.Prefix -= RelaxPrefixRequirements;
            On.Terraria.NPC.SlimeRainSpawns -= PreventBossSlimeRainSpawns;
            IL.Terraria.NPC.SpawnNPC -= MakeVoodooDemonDollWork;
            //IL.Terraria.NPC.VanillaHitEffect -= RemoveLavaDropsFromExpertLavaSlimes;
            IL.Terraria.Main.UpdateTime -= BloodMoonsRequire200MaxLife;

            // Fix vanilla bugs exposed by Calamity mechanics
            // On.Terraria.Main.InitLifeBytes -= BossRushLifeBytes;
            IL.Terraria.NPC.NPCLoot -= FixSplittingWormBannerDrops;
            // IL.Terraria.Main.DoUpdate -= FixProjectileUpdatePriorityProblems;
        }
    }
}
