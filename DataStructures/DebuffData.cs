using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.DataStructures
{
    public class DebuffData
    {
        /// <summary>
        /// Determines the behavior of the debuff.
        /// </summary>
        public enum DebuffBehavior
        {
            Default,
            Electric
        }
        #region DebuffData data
        /// <summary>
        /// UNIMPLEMENTED. WILL BE DONE IN A FUTURE PR
        /// Amount of life regen lost by a player under this debuff
        /// For debuffs with unique scaling, this should usually be the "base" amount.
        /// Defaults to 0
        /// </summary>
        public float PlayerLostRegen = 0;

        /// <summary>
        /// Amount of life regen lost by an enemy under this debuff.
        /// For debuffs with unique scaling, this should usually be the "base" amount.
        /// Defaults to 0
        /// </summary>
        public float EnemyLostRegen = 0;

        /// <summary>
        /// This is used to cancel Vanilla DoT damage on enemies and allow using Calamity's own instead.
        /// </summary>
        public int EnemyVanillaRegenToCancelOut = 0;

        /// <summary>
        /// The minimum size of damage ticks from this debuff on an enemy.
        /// Ticks can be larger than this due to MultiplierDamageTickSize
        /// Defaults to 1
        /// </summary>
        public int MinimumDamageTickSize = 1;

        /// <summary>
        /// The size of damage ticks from this debuff on an enemy
        /// This is based on the applied regen of the debuff, and will cause the tick to scale with amplifiers
        /// Ticks can be larger than this due to MinimumDamageTickSize
        /// Defaults to 0.25f
        /// </summary>
        public float MultiplierDamageTickSize = 0.25f;

        /// <summary>
        /// How much this benefits from heat debuff amplifiers.
        /// Default is 0, which means it does not benefit.
        /// </summary>
        public float HeatDebuffScaling = 0;
        /// <summary>
        /// How much this benefits from sickness debuff amplifiers.
        /// Default is 0, which means it does not benefit.
        /// </summary>
        public float SicknessDebuffScaling = 0;
        /// <summary>
        /// How much this benefits from cold debuff amplifiers.
        /// Default is 0, which means it does not benefit.
        /// </summary>
        public float ColdDebuffScaling = 0;
        /// <summary>
        /// How much this benefits from water debuff amplifiers.
        /// Default is 0, which means it does not benefit.
        /// </summary>
        public float WaterDebuffScaling = 0;
        /// <summary>
        /// How much this benefits from electric debuff amplifiers.
        /// Default is 0, which means it does not benefit.
        /// </summary>
        public float ElectricDebuffScaling = 0;

        /// <summary>
        /// UNIMPLEMENTED. WILL BE DONE IN A FUTURE PR
        /// Whether or not this debuff should draw above inflicted NPCs.
        /// Defaults to TRUE
        /// </summary>
        public bool DrawAboveNPC = true;

        /// <summary>
        /// UNIMPLEMENTED. WILL BE DONE IN A FUTURE PR
        /// Whether or not gear can modify the debuff effects such as duration or damage on the player.
        /// </summary>
        public bool GearCanModifyDebuff = true;

        /// <summary>
        /// How much alcohol this counts as.
        /// Default is 0, most alcohol is 1, and Everclear is 2
        /// </summary>
        public int AlcoholLevel = 0;

        /// <summary>
        /// UNIMPLEMENTED. WILL BE DONE IN A FUTURE PR
        /// The UpdatePlayerLifeRegen code to run. Defaults to just applying DoT
        /// </summary>
        public UpdatePlayerLifeRegen PlayerUpdateMethod;

        /// <summary>
        /// The UpdateNPCLifeRegen code to run. Defaults to just applying DoT
        /// Parameters of the method should be (NPC npc, int buffType, ref int buffIndex, ref int damage)
        /// </summary>
        public UpdateNPCLifeRegen NPCLifeRegenMethod;

        public DebuffData()
        {
            PlayerUpdateMethod = DefaultUpdateOnPlayer;
            NPCLifeRegenMethod = BaseUpdateNPCLifeRegen;
        }
        /// <summary>
        /// Allows using DebuffBehavior to determine preset behavior
        /// "electric" causes debuffs to scale 4x when moving
        /// Uses default behavior if no known key is inputed
        /// </summary>
        /// <param name="behavior">This determines the type of behavior of the debuff</param>
        public DebuffData(DebuffBehavior behavior)
        {

            PlayerUpdateMethod = DefaultUpdateOnPlayer;
            if (behavior == DebuffBehavior.Electric)
                NPCLifeRegenMethod = ElectricDebuffNPCLifeRegen;
            else
                NPCLifeRegenMethod = BaseUpdateNPCLifeRegen;
        }

        /// <summary>
        /// UNIMPLEMENTED. WILL BE IN A FUTURE PR
        /// This is the code that should be run when updating the buff on a player.
        /// Use for DoT damage. Other effects should be in the ModBuff class, or wherever appropriate.
        /// </summary>
        /// <param name="player"></param>
        public delegate void UpdatePlayerLifeRegen(Player player, int buffType, ref int buffIndex, ref int damage);

        /// <summary>
        /// This is the code that should be run when updating life regen on NPC
        /// Use for DoT damage. Other effects should be in the ModBuff class, or wherever appropriate.
        /// </summary>
        /// <param name="npc"></param>
        public delegate void UpdateNPCLifeRegen(NPC npc, int buffType, ref int buffIndex, ref int damage);

        /// <summary>
        /// Applies a scaling amount to a StatModifer
        /// </summary>
        /// <param name="Modifer"></param>
        /// <param name="scaling"></param>
        /// <returns></returns>
        public static StatModifier ApplyScalingToStatModifer(StatModifier Modifer, float scaling)
        {
            StatModifier output = new();
            output += (Modifer.Additive - 1) * scaling;
            output *= 1 + (Modifer.Multiplicative - 1) * scaling;
            output.Base = Modifer.Base * scaling;
            output.Flat = Modifer.Flat * scaling;
            return output;
        }
        /// <summary>
        /// Applies a scaling amount to a StatModifier but ensures all changes end up positive
        /// </summary>
        /// <param name="Modifer"></param>
        /// <param name="scaling"></param>
        /// <returns></returns>
        public static StatModifier ForceModifierPositiveWithScaling(StatModifier Modifer, float scaling)
        {
            StatModifier output = new();
            output += MathHelper.Max((Modifer.Additive - 1) * scaling, 0);
            output *= MathHelper.Max(1 + (Modifer.Multiplicative - 1) * scaling, 1);
            output.Base = MathHelper.Max(Modifer.Base * scaling, 0);
            output.Flat = MathHelper.Max(Modifer.Flat * scaling, 0);
            return output;
        }

        /// <summary>
        /// UNIMPLEMENTED. WILL BE IN A FUTURE PR
        /// The default debuff DoT functionality on a player
        /// </summary>
        public void DefaultUpdateOnPlayer(Player player, int buffType, ref int buffIndex, ref int damage)
        {

        }

        /// <summary>
        /// The default debuff DoT functionality on enemies.
        /// </summary>
        public void BaseUpdateNPCLifeRegen(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            var cnpc = npc.Calamity();
            float totalDPS = EnemyLostRegen;
            StatModifier totalScaling =
                 HeatDebuffScaling + ColdDebuffScaling + SicknessDebuffScaling + WaterDebuffScaling + ElectricDebuffScaling != 0
                 ?
                 ApplyScalingToStatModifer(cnpc.ActiveHeatDebuffMultiplier, HeatDebuffScaling)
                    .CombineWith(ApplyScalingToStatModifer(cnpc.ActiveColdDebuffMultiplier, ColdDebuffScaling)
                    .CombineWith(ApplyScalingToStatModifer(cnpc.ActiveSicknessDebuffMultiplier, SicknessDebuffScaling)
                    .CombineWith(ApplyScalingToStatModifer(cnpc.ActiveWaterDebuffMultiplier, WaterDebuffScaling)
                    .CombineWith(ApplyScalingToStatModifer(cnpc.ActiveElectricDebuffMultiplier, ElectricDebuffScaling)
                 ))))
                 :
                 // Bane doesn't scale with debuff multipliers. This should be implemented as a default feature you can apply to debuffs at some point.
                 buffType == ModContent.BuffType<Bane>() ? StatModifier.Default : cnpc.ActiveTypelessDebuffMultiplier;

            //Ensure at least 25% effectiveness
            if (totalScaling.Multiplicative <= 0.25f)
                totalScaling.Multiplicative = 0.25f;
            if (totalScaling.Additive < 0.25f)
                totalScaling.Additive = 0.25f;

            totalDPS = totalScaling.ApplyTo(totalDPS);
            var totalDPSAdjusted = totalDPS - EnemyVanillaRegenToCancelOut;
            npc.Calamity().ApplyDPSDebuff((int)(totalDPSAdjusted), (int)Math.Max(totalDPS * MultiplierDamageTickSize, MinimumDamageTickSize), ref npc.lifeRegen, ref damage);
        }
        /// <summary>
        /// DoT functionality that takes into account Electric debuff's 4x DPS when moving
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="buffType"></param>
        /// <param name="buffIndex"></param>
        /// <param name="damage"></param>
        public void ElectricDebuffNPCLifeRegen(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            var cnpc = npc.Calamity();
            float totalDPS = EnemyLostRegen;
            StatModifier totalScaling =
                 HeatDebuffScaling + ColdDebuffScaling + SicknessDebuffScaling + WaterDebuffScaling + ElectricDebuffScaling != 0
                 ?
                 ApplyScalingToStatModifer(cnpc.ActiveHeatDebuffMultiplier, HeatDebuffScaling)
                    .CombineWith(ApplyScalingToStatModifer(cnpc.ActiveColdDebuffMultiplier, ColdDebuffScaling)
                    .CombineWith(ApplyScalingToStatModifer(cnpc.ActiveSicknessDebuffMultiplier, SicknessDebuffScaling)
                    .CombineWith(ApplyScalingToStatModifer(cnpc.ActiveWaterDebuffMultiplier, WaterDebuffScaling)
                    .CombineWith(ApplyScalingToStatModifer(cnpc.ActiveElectricDebuffMultiplier, ElectricDebuffScaling)
                 ))))
                 :
                 // Bane doesn't scale with debuff multipliers. This should be implemented as a default feature you can apply to debuffs at some point.
                 buffType == ModContent.BuffType<Bane>() ? cnpc.ActiveTypelessDebuffMultiplier : StatModifier.Default;

            //Ensure at least 25% effectiveness
            if (totalScaling.Multiplicative <= 0.25f)
                totalScaling.Multiplicative = 0.25f;
            if (totalScaling.Additive < 0.25f)
                totalScaling.Additive = 0.25f;

            totalDPS = totalScaling.ApplyTo(totalDPS);
            totalDPS *= (npc.velocity.X == 0 ? 1 : 4);
            totalDPS -= EnemyVanillaRegenToCancelOut * (npc.velocity.X == 0 ? 1 : 5); //Vanilla Electrified is 5x when moving, not 4x
            npc.Calamity().ApplyDPSDebuff((int)(totalDPS), (int)Math.Max(totalDPS * MultiplierDamageTickSize, MinimumDamageTickSize), ref npc.lifeRegen, ref damage);
        }
        #region Special Regen Functions
        /// <summary>
        /// 18OCT2023: Ozzatron: im not gonna sugarcoat it<br/>
        /// Vanilla debuff damage from Daybreak impales scales linearly up to 8 for 800 DPS.<br/>
        /// Instead of allowing this entire 800 DPS to be multiplied by heat weakness + heat DoT bonuses, additional Daybreak spears beyond the first do not contribute to weaknesses or resistances.<br/>
        /// This also stops Daybreak's DPS from being utterly shafted by heat resistance.<br/>
        /// As no other weapon can stack Daybroken, this has no effect on other weapons (they count as "1 Daybreak spear")
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="buffType"></param>
        /// <param name="buffIndex"></param>
        /// <param name="damage"></param>
        public static void DaybrokenRegen(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            var cnpc = npc.Calamity();
            int numImpaledSpears = 0;
            foreach (Projectile k in Main.ActiveProjectiles)
            {
                if (k.type == ProjectileID.Daybreak && k.ai[0] == 1f && k.ai[1] == npc.whoAmI)
                    numImpaledSpears++;
            }

            // If there are no Daybreak impaled spears, Daybroken has 1x potency (it was applied some other way)
            int adjustedSpears = Math.Max(1, numImpaledSpears);
            int baseDaybreakDoTValue = (int)(npc.Calamity().ActiveHeatDebuffMultiplier.ApplyTo(Daybroken.EnemyLostRegen) + (Daybroken.EnemyLostRegen * (adjustedSpears - 1)));
            int totalDPSAdjusted = baseDaybreakDoTValue - Daybroken.EnemyVanillaRegenToCancelOut * numImpaledSpears;
            if (numImpaledSpears == 0)
            {
                totalDPSAdjusted -= Daybroken.EnemyVanillaRegenToCancelOut;
            }
            npc.Calamity().ApplyDPSDebuff(totalDPSAdjusted, (int)Math.Max(baseDaybreakDoTValue * Daybroken.MultiplierDamageTickSize, Daybroken.MinimumDamageTickSize), ref npc.lifeRegen, ref damage);
        }
        /// <summary>
        /// Apply Oiled DoT.
        /// </summary>
        public static void OiledNPCMethod(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            var cnpc = npc.Calamity();
            double totalDPS = ApplyScalingToStatModifer(cnpc.ActiveHeatDebuffMultiplier, Oiled.HeatDebuffScaling).ApplyTo(Oiled.EnemyLostRegen);
            if (totalDPS <= 0)
                return;
            npc.Calamity().ApplyDPSDebuff((int)(totalDPS), damage + (int)Math.Max(totalDPS * Oiled.MultiplierDamageTickSize, Oiled.MinimumDamageTickSize), ref npc.lifeRegen, ref damage);
        }
        /// <summary>
        /// Has Dryad's Bane scale with Calamity's town NPC buffs
        /// </summary>
        public static void DryadsBaneNPCMethod(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            // Buffs match 1:1 with Town NPC buffs
            // See CalamityGlobalTownNPC: BuffTownNPC
            float buffedDryadsBaneMult = 0f +
                (NPC.downedMoonlord ? 0.6f : 0f) +
                (DownedBossSystem.downedProvidence ? 0.2f : 0f) +
                (DownedBossSystem.downedPolterghast ? 0.2f : 0f) +
                (DownedBossSystem.downedDoG ? 0.2f : 0f) +
                (DownedBossSystem.downedYharon ? 0.2f : 0f) +
                (DownedBossSystem.downedExoMechs ? 0.6f : 0f) +
                (DownedBossSystem.downedCalamitas ? 0.6f : 0f);
            if (Main.expertMode)
                buffedDryadsBaneMult *= Main.GameModeInfo.TownNPCDamageMultiplier;
            int buffedDryadsBaneDoTValue = 2 * (int)(4 * buffedDryadsBaneMult);
            npc.lifeRegen -= buffedDryadsBaneDoTValue;

            // Scales damage tick if greater than the vanilla amount
            float vanillaDryadsBaneMult = 1f +
                (NPC.downedBoss1 ? 0.1f : 0f) +
                (NPC.downedBoss2 ? 0.1f : 0f) +
                (NPC.downedBoss3 ? 0.1f : 0f) +
                (NPC.downedQueenBee ? 0.1f : 0f) +
                (Main.hardMode ? 0.4f : 0f) +
                (NPC.downedMechBoss1 ? 0.15f : 0f) +
                (NPC.downedMechBoss1 ? 0.15f : 0f) +
                (NPC.downedMechBoss1 ? 0.15f : 0f) +
                (NPC.downedPlantBoss ? 0.15f : 0f) +
                (NPC.downedGolemBoss ? 0.15f : 0f) +
                (NPC.downedAncientCultist ? 0.15f : 0f);
            if (Main.expertMode)
                vanillaDryadsBaneMult *= Main.GameModeInfo.TownNPCDamageMultiplier;
            int totalDryadsBaneDoTValue = 2 * (int)(4 * vanillaDryadsBaneMult) + buffedDryadsBaneDoTValue;
            if (damage < totalDryadsBaneDoTValue / 6)
                damage = totalDryadsBaneDoTValue / 6;
        }

        #endregion
        #endregion

        #region Vanilla debuff stats
        public static DebuffData OnFire = new DebuffData()
        {
            EnemyLostRegen = 12,
            EnemyVanillaRegenToCancelOut = 12,
            HeatDebuffScaling = 1
        };
        public static DebuffData Hellfire = new DebuffData()
        {
            EnemyLostRegen = 30,
            EnemyVanillaRegenToCancelOut = 30,
            HeatDebuffScaling = 1
        };
        public static DebuffData CursedInferno = new DebuffData()
        {
            EnemyLostRegen = 48,
            EnemyVanillaRegenToCancelOut = 48,
            HeatDebuffScaling = 1
        };
        public static DebuffData Shadowflame = new DebuffData()
        {
            EnemyLostRegen = 60,
            EnemyVanillaRegenToCancelOut = 60,
            HeatDebuffScaling = 1
        };
        public static DebuffData Daybroken = new DebuffData()
        {
            EnemyLostRegen = 200,
            EnemyVanillaRegenToCancelOut = 200,
            HeatDebuffScaling = 1,
            NPCLifeRegenMethod = DaybrokenRegen
        };
        // Provided purely to classify it as a heat debuff
        public static DebuffData Burning = new DebuffData()
        {
            HeatDebuffScaling = 1
        };
        public static DebuffData Frostburn = new DebuffData()
        {
            EnemyLostRegen = 16,
            EnemyVanillaRegenToCancelOut = 16,
            ColdDebuffScaling = 1
        };
        public static DebuffData Frostbite = new DebuffData()
        {
            EnemyLostRegen = 50,
            EnemyVanillaRegenToCancelOut = 50,
            ColdDebuffScaling = 1
        };
        public static DebuffData Poisoned = new DebuffData()
        {
            EnemyLostRegen = 12,
            EnemyVanillaRegenToCancelOut = 12,
            SicknessDebuffScaling = 1
        };
        public static DebuffData AcidVenom = new DebuffData()
        {
            EnemyLostRegen = 60,
            EnemyVanillaRegenToCancelOut = 60,
            SicknessDebuffScaling = 1
        };
        public static DebuffData Electrified = new DebuffData(DebuffBehavior.Electric)
        {
            EnemyLostRegen = 30, // 15 dps stationary, 60 dps moving
            EnemyVanillaRegenToCancelOut = 8,
            ElectricDebuffScaling = 1
        };
        public static DebuffData Oiled = new DebuffData()
        {
            EnemyLostRegen = 50, //This is how much DPS Oiled does when applied alongside a valid debuff
            EnemyVanillaRegenToCancelOut = 50,
            HeatDebuffScaling = 2, //Oiled scales twice as hard with heat debuff scaling to reward debuff builds who use such a rare and difficult to apply debuff.
            NPCLifeRegenMethod = OiledNPCMethod
        };
        public static DebuffData DryadsBane = new DebuffData()
        {
            EnemyLostRegen = 8, //This is not used in the method, serves as a token amount for anything that may need to interface in the future.
            NPCLifeRegenMethod = DryadsBaneNPCMethod
        };
        #endregion

        #region Helpers
        public static int GetDebuffRegenValue(NPC npc, int type)
        {
            var debuffData = CalamityBuffSets.DebuffDataset[type];
            if (debuffData == null)
                return 0;

            var oldRegen = npc.lifeRegen;
            var oldCount = npc.lifeRegenCount;
            int dmg = 1;
            npc.lifeRegenCount = 0;
            npc.lifeRegen = 0;

            int index = npc.FindBuffIndex(type);

            if (debuffData == Oiled)
            {
                bool hasVanillaOil = npc.onFrostBurn || npc.onFrostBurn2 || npc.onFire || npc.onFire2 || npc.onFire3 || npc.shadowFlame;
                if (hasVanillaOil)
                    npc.lifeRegen -= Oiled.EnemyVanillaRegenToCancelOut;
                Oiled.NPCLifeRegenMethod(npc, BuffID.Oiled, ref index, ref dmg);
            }

            debuffData.NPCLifeRegenMethod(npc, type, ref index, ref dmg);

            int done = npc.lifeRegen;
            npc.lifeRegen = oldRegen;
            npc.lifeRegenCount = oldCount;
            return done;
        }

        #endregion
    }
}
