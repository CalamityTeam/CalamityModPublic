using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.FurnitureExo;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Graphics.CameraModifiers;
using Terraria.Map;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges : ILoadable
    {
        void ILoadable.Load(Mod mod)
        {
            ManipulatorManager.ApplyEdits += ApplyEdits;
        }

        void ILoadable.Unload()
        {
            // (2023/07) Manually unloading IL hooks is no longer necessary as TML should automatically do it for you.
            // https://discord.com/channels/103110554649894912/445276626352209920/1099856743820959855 (links to TML server)
        }

        /// <summary>
        /// Loads all IL Editing changes in the mod.
        /// </summary>
        private static void ApplyEdits(ManipulatorContext ctx)
        {
            // Cache the six lab door tile types for efficiency.
            labDoorOpen = ModContent.TileType<LaboratoryDoorOpen>();
            labDoorClosed = ModContent.TileType<LaboratoryDoorClosed>();
            aLabDoorOpen = ModContent.TileType<AgedLaboratoryDoorOpen>();
            aLabDoorClosed = ModContent.TileType<AgedLaboratoryDoorClosed>();
            exoDoorOpen = ModContent.TileType<ExoDoorOpen>();
            exoDoorClosed = ModContent.TileType<ExoDoorClosed>();

            // Graphics
            IL_Main.DoDraw += CustomDoDrawChanges;
            On_Main.DrawCursor += UseCoolFireCursorEffect;
            On_Main.SortDrawCacheWorms += DrawFusableParticles;
            On_TileDrawing.Draw += ClearTilePings;
            On_CommonCode.ModifyItemDropFromNPC += ColorBlightedGel;
            On_MoonlordDeathDrama.RequestLight += DisableFlashesWithPhotosensitivityConfig;

            // Graphics (ModPlant stuff)
            IL_TileDrawing.DrawSingleTile += DisableCullingForTreeAndCactus;
            IL_TileDrawing.DrawTrees += DrawTreeGlowMask;
            On_TileDrawing.DrawBasicTile += DrawTreeTrunkAndCactusGlowMask;

            // NPC behavior
            IL_Main.UpdateTime += PermitNighttimeTownNPCSpawning;
            On_Main.UpdateTime_SpawnTownNPCs += AlterTownNPCSpawnRate;
            IL_NPC.DoDeathEvents += PreventVanillaBossDeathsInBossRush;
            On_NPC.NPCLoot += PreventDiabolistLootLogic;
            IL_Player.CollectTaxes += MakeTaxCollectorUseful;

            // Mechanics / features
            On_NPC.ApplyTileCollision += AllowFusionFeederToDigThroughSand;
            IL_Player.ApplyEquipFunctional += ScopesRequireVisibilityToZoom;
            IL_Player.Hurt_PlayerDeathReason_int_int_refHurtInfo_bool_bool_int_bool_float_float_float += DodgeMechanicAdjustments;
            On_Player.PutHallowedArmorSetBonusOnCooldown += AddHolyProtectionCooldown;
            IL_Player.DashMovement += FixAllDashMechanics;
            On_Player.DashMovement += DashMovementEdits;
            On_Player.DoCommonDashHandle += ApplyDashKeybind;
            On_Player.KeyDoubleTap += DisableDoubleTapOnConfig;
            IL_Player.GiveImmuneTimeForCollisionAttack += MakeShieldSlamIFramesConsistent;
            IL_Player.Update_NPCCollision += NerfShieldOfCthulhuBonkSafety;
            On_WorldGen.OpenDoor += OpenDoor_LabDoorOverride;
            On_WorldGen.CloseDoor += CloseDoor_LabDoorOverride;
            On_Item.AffixName += IncorporateEnchantmentInAffix;
            On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += IncorporateExtraProjectileVariables;
            On_Player.ApplyDamageToNPC += ApplyOldFashionedDamageToMiscHits;
            IL_Wiring.HitWireSingle += AddTwinklersToStatue;
            On_AWorldListItem.GetDifficulty += GetDifficultyOverride;
            On_Item.GetShimmered += ShimmerEffectEdits;
            On_Player.Teleport += TPOverride;
            On_TileDrawing.DrawSingleTile += GlowMaskTileRender;
            On_Player.PlaceThing_CannonBall += AllowCannonJellyfishUse;
            On_Player.IsItemSlotUnlockedAndUsable += MasterModeCelestialOnionCheck;
            On_Projectile.AI_007_GrapplingHooks += AllowHooksToGrabArenabox;
            On_Collision.SolidCollision_Vector2_int_int += ArenaCollision_Vector2_int_int;
            On_Collision.SolidCollision_Vector2_int_int_bool += ArenaCollision_Vector2_int_int_bool;
            On_Collision.TileCollision += ArenaCollision_TileCollision;

            // Mana Burn (Chaos Stone) and Chalice of the Blood God
            IL_Player.ApplyLifeAndOrMana += ChaliceBufferHeal;
            On_Player.CheckMana_int_bool_bool += AllowNegativeCheckMana;
            On_Player.CheckMana_Item_int_bool_bool += AllowNegativeCheckMana;

            // Custom grappling
            On_Player.GrappleMovement += CustomGrappleMovementCheck;
            On_Player.UpdatePettingAnimal += CustomGrapplePreDefaultMovement;
            On_Player.PlayerFrame += CustomGrapplePostFrame;
            On_Player.SlopeDownMovement += CustomGrapplePreStepUp;

            // Damage and health balance
            On_Main.DamageVar_float_int_float += AdjustDamageVariance;
            IL_NPC.ScaleStats_ApplyExpertTweaks += RemoveExpertHardmodeScaling;
            IL_Projectile.Damage += PreventVanillaWhipTagCrits;
            IL_Projectile.Damage += VanillaBossResistChanges;
            IL_Projectile.AI_026 += PygmyAggroOnClosestPointInHitbox;
            IL_Projectile.AI_099_2 += LimitTerrarianProjectiles;
            On_Projectile.AI_015_Flails += FlailsNoLongerAffectedByPlayerVelocity;
            IL_Projectile.AI_015_Flails += IncreaseFlowerPowRetSpeed;
            IL_Projectile.AI_120_StardustGuardian += StardustGuardianAttackBuffs;
            On_Player.ConsumeSolarFlare += SolarWingsDashChange;
            On_Projectile.IsDamageDodgable += GFBNurseMeteorUndodgeable;
            IL_Player.UpdateBuffs += UpdateBuffsBalancingChanges;
            IL_Player.ApplyVanillaHurtEffectModifiers += RemoveBeetleAndSolarFlareMultiplicativeDR;
            On_Projectile.ghostHeal += AdjustSpectreHealing;
            On_Projectile.vampireHeal += AdjustVampireHealing;

            // Movement speed balance
            IL_Player.UpdateJumpHeight += FixJumpHeightBoosts;
            ctx.PlayerUpdate.Add(BaseJumpSpeedAdjustment);
            ctx.PlayerUpdate.Add(RunSpeedAdjustments);
            ctx.PlayerUpdate.Add(NerfOverpoweredRunAccelerationSources); // Soaring Insignia, Magiluminescence, and Shadow Armor
            IL_Player.WingMovement += RemoveSoaringInsigniaInfiniteWingTime;

            // Life regen and mana regen balance
            IL_Player.UpdateLifeRegen += UpdateLifeRegenBalancingChanges;
            IL_Player.UpdateManaRegen += UpdateManaRegenBalancingChanges;
            ctx.PlayerUpdate.Add(ManaRegenDelayAdjustment);

            // Debuff balancing
            IL_Projectile.StatusPlayer += RemoveFrozenInflictionFromDeerclopsIceSpikes;

            // World generation
            IL_WorldGen.GrowLivingTree += BlockLivingTreesNearOcean;
            On_WorldGen.SmashAltar += PreventSmashAltarCode;
            IL_WorldGen.hardUpdateWorld += AdjustChlorophyteSpawnRate;
            IL_WorldGen.Chlorophyte += AdjustChlorophyteSpawnLimits;
            IL_UIWorldCreation.SetDefaultOptions += ChangeDefaultWorldSize;
            IL_UIWorldCreation.AddWorldSizeOptions += SwapSmallDescriptionKey;
            On_WorldGen.MakeDungeon += LimitDungeonEntranceXPosition;
            IL_WorldGen.DungeonHalls += LimitDungeonHallsXPosition;

            // Removal of vanilla stupidity
            IL_Player.StatusFromNPC += RemoveExpertBrainRandomDebuffs;
            On_NPC.HitEffect_HitInfo += PreventLavaSlimeLavaDrop;
            IL_NPC.StrikeNPC_HitInfo_bool_bool += LetDetonatingBubblesTakeDamage;
            IL_PunchCameraModifier.Update += PunchCameraUsesScreenshakeConfig;
            IL_Player.ItemCheck_EmitUseVisuals += MakeMagmaStoneFireGauntletDustToggleable;
            IL_Projectile.EmitEnchantmentVisualsAt += MakeMagmaStoneFireGauntletProjectileDustToggleable;
            IL_Sandstorm.HasSufficientWind += DecreaseSandstormWindSpeedRequirement;
            IL_Item.TryGetPrefixStatMultipliersForItem += RelaxPrefixRequirements;
            On_NPC.SlimeRainSpawns += PreventBossSlimeRainSpawns;
            On_ShimmerTransforms.IsItemTransformLocked += AdjustShimmerRequirements;

            IL_Projectile.CanExplodeTile += MakeMeteoriteExplodable;
            IL_Main.UpdateTime_StartNight += BloodMoonsRequire200MaxLife;
            IL_WorldGen.AttemptFossilShattering += PreventFossilShattering;
            On_Player.GetPickaxeDamage += RemoveHellforgePickaxeRequirement;
            ctx.PlayerUpdate.Add(PreventUFODismountInWater);

            On_Player.GetAnglerReward_MainReward += AddMoreGuaranteedAnglerRewards;
            On_Player.GetAnglerReward_Bait += ImproveAnglerBaitReward;
            On_Player.GetAnglerReward_Money += ImproveAnglerMoneyReward;

            IL_Player.TileInteractionsUse += RemovePowerCellPlanteraLock;
            On_Player.ItemCheck_CheckCanUse += RemoveUseLocks;
            On_Player.ItemCheck_UseEventItems += ApplyCelestialSigilChanges;
            IL_Main.DrawInfoAccs += RemoveDamageConditionFromRadar;
            //On_ShopHelper.ApplyNpcRelationshipEffect += AllowMultipleLikedNPCs;

            On_Player.UpdateControlHolds += DelayGravity;
            On_PlayerInput.SetZoom_MouseInWorld += GravityMouse;
            On_Main.DrawPlayerChatBubbles += UI_Unflip_Start;
            On_Main.DrawInterface += UI_Unflip_End;

            // Fix vanilla not accounting for spritebatch modification in held projectile drawing
            On_PlayerDrawLayers.DrawHeldProj += FixHeldProjectileBlendState;

            // Fix vanilla not accounting for multiple bobbers when fishing with truffle worm
            IL_Player.ItemCheck_CheckFishingBobbers += FixTruffleWormFishing;

            // Allow specified walls to be visible through water on the map
            IL_MapHelper.CreateMapTile += UseVisibleThroughWaterMapTile;

            // Fix vanilla behaviour of not calling CheckDead for NPCs that realLife is set
            IL_NPC.StrikeNPC_HitInfo_bool_bool += EnsureCheckDeadOnSegments;

            //Additional detours that are in their own item files given they are only relevant to these specific items:
            //Rover drive detours on Player.DrawInfernoRings to draw its shield
            //Wulfrum armor hooks on Player.KeyDoubleTap and DrawPendingMouseText to activate its set bonus and spoof the mouse text to display the stats of the activated weapon if shift is held
            //HeldOnlyItem detours Player.dropItemCheck, ItemSlot.Draw (Sb, itemarray, int, int, vector2, color) and ItemSlot.LeftClick_ItemArray to make its stuff work
        }
    }
}
