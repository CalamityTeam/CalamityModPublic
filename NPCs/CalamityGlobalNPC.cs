using CalamityMod.Buffs.DoTDebuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles;
using CalamityMod.Utilities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public class CalamityGlobalNPC : GlobalNPC
	{
        #region Variables
        public float DR { get; set; } = 0f;

        /// <summary>
        /// If this is set to true, the NPC's DR cannot be reduced via any means. This applies regardless of whether customDR is true or false.
        /// </summary>
        public bool unbreakableDR = false;

        /// <summary>
        /// Overrides the normal DR math and uses custom DR reductions for each debuff, registered separately.<br></br>
        /// Used primarily by post-Moon Lord bosses.
        /// </summary>
        public bool customDR = false;
        public Dictionary<int, float> flatDRReductions = new Dictionary<int, float>();
        public Dictionary<int, float> multDRReductions = new Dictionary<int, float>();

		// Iron Heart
		private int ironHeartDamage = 0;

		// NewAI
		private const int maxAIMod = 4;
		public float[] newAI = new float[maxAIMod];

		// Town NPC Patreon
		private bool setNewName = true;

		// Draedons Remote
		public static bool DraedonMayhem = false;

		// Debuffs
		public bool timeSlow = false;
		public bool wCleave = false;
		public bool bBlood = false;
		public bool dFlames = false;
		public bool marked = false;
		public bool irradiated = false;
		public bool bFlames = false;
		public bool hFlames = false;
		public bool pFlames = false;
		public bool gState = false;
		public bool aCrunch = false;
		public bool tSad = false;
		public bool pShred = false;
		public bool cDepth = false;
		public bool gsInferno = false;
		public bool astralInfection = false;
		public bool aFlames = false;
		public bool eFreeze = false;
		public bool wDeath = false;
		public bool nightwither = false;
		public bool silvaStun = false;
		public bool enraged = false;
		public bool yellowCandle = false;
		public bool pearlAura = false;
		public bool shellfishVore = false;
		public bool clamDebuff = false;

		// whoAmI Variables
		public static int bobbitWormBottom = -1;
		public static int hiveMind = -1;
		public static int perfHive = -1;
		public static int slimeGodPurple = -1;
		public static int slimeGodRed = -1;
		public static int slimeGod = -1;
		public static int laserEye = -1;
		public static int fireEye = -1;
		public static int primeLaser = -1;
		public static int primeCannon = -1;
		public static int primeVice = -1;
		public static int primeSaw = -1;
		public static int brimstoneElemental = -1;
		public static int cataclysm = -1;
		public static int catastrophe = -1;
		public static int calamitas = -1;
		public static int leviathan = -1;
		public static int siren = -1;
		public static int scavenger = -1;
		public static int astrumDeusHeadMain = -1;
		public static int energyFlame = -1;
		public static int doughnutBoss = -1;
		public static int holyBossAttacker = -1;
		public static int holyBossDefender = -1;
		public static int holyBossHealer = -1;
		public static int holyBoss = -1;
		public static int voidBoss = -1;
		public static int ghostBossClone = -1;
		public static int ghostBoss = -1;
		public static int DoGHead = -1;
		public static int SCalCataclysm = -1;
		public static int SCalCatastrophe = -1;
		public static int SCal = -1;
		public static int SCalWorm = -1;

        // Collections
        public static SortedDictionary<int, int> BossRushHPChanges = new SortedDictionary<int, int>
        {
            // Tier 1
            { NPCID.QueenBee, 420000 },
            { NPCID.BrainofCthulhu, 200000 },
            { NPCID.Creeper, 20000 },
            { NPCID.KingSlime, 500000 },
            { NPCID.BlueSlime, 12000 },
            { NPCID.SlimeSpiked, 24000 },
			{ NPCID.GreenSlime, 9000 },
			{ NPCID.RedSlime, 18000 },
			{ NPCID.PurpleSlime, 24000 },
			{ NPCID.YellowSlime, 21000 },
			{ NPCID.IceSlime, 15000 },
			{ NPCID.UmbrellaSlime, 18000 },
			{ NPCID.RainbowSlime, 100000 },
			{ NPCID.Pinky, 50000 },
			{ NPCID.EyeofCthulhu, 600000 },
            { NPCID.ServantofCthulhu, 60000 },
            { NPCID.SkeletronPrime, 980000 },
            { NPCID.PrimeVice, 540000 },
            { NPCID.PrimeCannon, 450000 },
            { NPCID.PrimeSaw, 450000 },
            { NPCID.PrimeLaser, 380000 },
            { NPCID.Golem, 450000 },
            { NPCID.GolemHead, 300000 },
            { NPCID.GolemHeadFree, 300000 },
            { NPCID.GolemFistLeft, 250000 },
            { NPCID.GolemFistRight, 250000 },
            { NPCID.EaterofWorldsHead, 20000 },
            { NPCID.EaterofWorldsBody, 30000 },
            { NPCID.EaterofWorldsTail, 40000 },

            // Tier 2
            { NPCID.TheDestroyer, 2500000 },
            { NPCID.TheDestroyerBody, 2500000 },
            { NPCID.TheDestroyerTail, 2500000 },
            { NPCID.Probe, 50000 },
            { NPCID.Spazmatism, 1300000 },
            { NPCID.Retinazer, 900000 },
            { NPCID.WallofFlesh, 2400000 },
            { NPCID.WallofFleshEye, 2400000 },
            { NPCID.SkeletronHead, 1300000 },
            { NPCID.SkeletronHand, 500000 },

            // Tier 3
            { NPCID.CultistBoss, 1100000 },
            { NPCID.CultistDragonHead, 400000 },
            { NPCID.CultistDragonBody1, 400000 },
            { NPCID.CultistDragonBody2, 400000 },
            { NPCID.CultistDragonBody3, 400000 },
            { NPCID.CultistDragonBody4, 400000 },
            { NPCID.CultistDragonTail, 400000 },
            { NPCID.AncientCultistSquidhead, 300000 },
            { NPCID.Plantera, 2100000 },
            { NPCID.PlanterasTentacle, 90000 },

            // Tier 4
            { NPCID.DukeFishron, 2500000 },
            { NPCID.MoonLordCore, 1400000 },
            { NPCID.MoonLordHand, 450000 },
            { NPCID.MoonLordHead, 600000 },
			{ NPCID.MoonLordLeechBlob, 8000 }
		};

        public static SortedDictionary<int, int> BossValues = new SortedDictionary<int, int>
        {
            { NPCID.QueenBee, Item.buyPrice(0, 5)},
            { NPCID.SkeletronHead, Item.buyPrice(0, 7) },
            { NPCID.DukeFishron, Item.buyPrice(0, 25) },
            { NPCID.CultistBoss, Item.buyPrice(0, 25) },
            { NPCID.MoonLordCore, Item.buyPrice(0, 30) }
        };
        #endregion

        #region Instance Per Entity
        public override bool InstancePerEntity => true;
        #endregion

        #region Reset Effects
        public override void ResetEffects(NPC npc)
        {
            void ResetSavedIndex(ref int type)
            {
                if (type >= 0 && !Main.npc[type].active)
                {
                    type = -1;
                }
            }

            ResetSavedIndex(ref bobbitWormBottom);
            ResetSavedIndex(ref hiveMind);
            ResetSavedIndex(ref perfHive);
            ResetSavedIndex(ref slimeGodPurple);
            ResetSavedIndex(ref slimeGodRed);
            ResetSavedIndex(ref slimeGod);
            ResetSavedIndex(ref laserEye);
            ResetSavedIndex(ref fireEye);
            ResetSavedIndex(ref primeLaser);
            ResetSavedIndex(ref primeCannon);
            ResetSavedIndex(ref primeVice);
            ResetSavedIndex(ref primeSaw);
            ResetSavedIndex(ref brimstoneElemental);
            ResetSavedIndex(ref cataclysm);
            ResetSavedIndex(ref catastrophe);
            ResetSavedIndex(ref calamitas);
            ResetSavedIndex(ref leviathan);
            ResetSavedIndex(ref siren);
            ResetSavedIndex(ref scavenger);
            ResetSavedIndex(ref astrumDeusHeadMain);
            ResetSavedIndex(ref energyFlame);
            ResetSavedIndex(ref doughnutBoss);
            ResetSavedIndex(ref holyBossAttacker);
            ResetSavedIndex(ref holyBossDefender);
            ResetSavedIndex(ref holyBossHealer);
            ResetSavedIndex(ref holyBoss);
            ResetSavedIndex(ref voidBoss);
            ResetSavedIndex(ref ghostBossClone);
            ResetSavedIndex(ref ghostBoss);
            ResetSavedIndex(ref DoGHead);
            ResetSavedIndex(ref SCalCataclysm);
            ResetSavedIndex(ref SCalCatastrophe);
            ResetSavedIndex(ref SCal);
            ResetSavedIndex(ref SCalWorm);

            timeSlow = false;
            wCleave = false;
            bBlood = false;
            dFlames = false;
            marked = false;
            irradiated = false;
            bFlames = false;
            hFlames = false;
            pFlames = false;
            gState = false;
            aCrunch = false;
            tSad = false;
            pShred = false;
            cDepth = false;
            gsInferno = false;
            astralInfection = false;
            aFlames = false;
            eFreeze = false;
            wDeath = false;
            nightwither = false;
            silvaStun = false;
            enraged = false;
            yellowCandle = false;
            pearlAura = false;
            shellfishVore = false;
            clamDebuff = false;
        }
        #endregion

        #region Life Regen
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            int genLimit = Main.maxTilesX / 2;
            int abyssChasmX = CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135);
            bool abyssPosX = false;

            if (CalamityWorld.abyssSide)
            {
                if ((double)(npc.position.X / 16f) < abyssChasmX + 80)
                {
                    abyssPosX = true;
                }
            }
            else
            {
                if ((double)(npc.position.X / 16f) > abyssChasmX - 80)
                {
                    abyssPosX = true;
                }
            }

            bool inAbyss = ((npc.position.Y / 16f > (Main.rockLayer - Main.maxTilesY * 0.05)) && ((double)(npc.position.Y / 16f) <= Main.maxTilesY - 250) && abyssPosX) || CalamityWorld.abyssTiles > 200;
            bool hurtByAbyss = npc.wet && npc.damage > 0 && !npc.boss && !npc.friendly && !npc.dontTakeDamage && inAbyss && !npc.buffImmune[mod.BuffType("CrushDepth")];
            if (hurtByAbyss)
            {
                npc.AddBuff(mod.BuffType("CrushDepth"), 2);
                npc.DeathSound = null;
                npc.HitSound = null;
            }

            if (npc.damage > 0 && !npc.boss && !npc.friendly && !npc.dontTakeDamage && CalamityWorld.sulphurTiles > 30 &&
                !npc.buffImmune[BuffID.Poisoned] && !npc.buffImmune[mod.BuffType("CrushDepth")])
            {
                if (npc.wet)
                {
                    npc.AddBuff(BuffID.Poisoned, 2);
                }

                if (Main.raining)
                {
                    npc.AddBuff(mod.BuffType("Irradiated"), 2);
                }
            }

            if (npc.venom)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                int projectileCount = 0;
                for (int j = 0; j < 1000; j++)
                {
                    if (Main.projectile[j].active &&
                        (Main.projectile[j].type == mod.ProjectileType("Lionfish") || Main.projectile[j].type == mod.ProjectileType("SulphuricAcidCannon2")) &&
                        Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == npc.whoAmI)
                    {
                        projectileCount++;
                    }
                }
                for (int j = 0; j < 1000; j++)
                {
                    if (Main.projectile[j].active &&
                        (Main.projectile[j].type == mod.ProjectileType("ScourgeoftheSeasStealth")) &&
                        Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == npc.whoAmI)
                    {
                        projectileCount += 6;
                    }
                }

                if (projectileCount > 0)
                {
                    npc.lifeRegen -= projectileCount * 30;

                    if (damage < projectileCount * 6)
                    {
                        damage = projectileCount * 6;
                    }
                }
            }

            if (shellfishVore)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                int projectileCount = 0;
                for (int j = 0; j < 1000; j++)
                {
                    if (Main.projectile[j].active &&
                        (Main.projectile[j].type == mod.ProjectileType("Shellfish")) &&
                        Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == npc.whoAmI &&
                        projectileCount < 5)
                    {
                        projectileCount++;
                    }
                }

                npc.lifeRegen -= projectileCount * 350;

                if (damage < projectileCount * 70)
                {
                    damage = projectileCount * 70;
                }
            }

            if (clamDebuff)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                int projectileCount = 0;
                for (int j = 0; j < 1000; j++)
                {
                    if (Main.projectile[j].active &&
                        (Main.projectile[j].type == mod.ProjectileType("SnapClamProj")) &&
                        Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == npc.whoAmI &&
                        projectileCount < 2)
                    {
                        projectileCount++;
                    }
                }

                npc.lifeRegen -= projectileCount * 35;

                if (damage < projectileCount * 7)
                {
                    damage = projectileCount * 7;
                }
            }

            if (cDepth)
            {
                if (npc.defense < 0)
                {
                    npc.defense = 0;
                }

                int depthDamage = Main.hardMode ? 80 : 12;
                if (hurtByAbyss)
                {
                    depthDamage = 300;
                }

                int calcDepthDamage = depthDamage - npc.defense;
                if (calcDepthDamage < 0)
                {
                    calcDepthDamage = 0;
                }

                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= calcDepthDamage * 5;

                if (damage < calcDepthDamage)
                {
                    damage = calcDepthDamage;
                }
            }

            // Exo Freeze, Glacial State and Temporal Sadness don't work on bosses or other specific enemies.
            if (!npc.boss && !CalamityMod.movementImpairImmuneList.Contains(npc.type))
            {
                if (eFreeze && !CalamityWorld.bossRushActive)
                {
                    npc.velocity.X = 0f;
                    npc.velocity.Y += 0.1f;
                    if (npc.velocity.Y > 15f)
                        npc.velocity.Y = 15f;
                }
                else if (gState)
                {
                    npc.velocity.X = 0f;
                    npc.velocity.Y += 0.05f;
                    if (npc.velocity.Y > 15f)
                        npc.velocity.Y = 15f;
                }
                if (tSad)
                {
                    npc.velocity /= 2f;
                }
            }

            ApplyDPSDebuff(irradiated, 20, 4, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(bFlames, 40, 8, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(hFlames, 50, 10, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(pFlames, 100, 20, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(gsInferno, 250, 50, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(astralInfection, 75, 15, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(aFlames, 125, 25, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(pShred, 1500, 300, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(nightwither, 200, 40, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(dFlames, 2500, 500, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(bBlood, 50, 10, ref npc.lifeRegen, ref damage);
        }

        public void ApplyDPSDebuff(bool debuff, int lifeRegenValue, int damageValue, ref int lifeRegen, ref int damage)
        {
            if (debuff)
            {
                if (lifeRegen > 0)
                {
                    lifeRegen = 0;
                }

                lifeRegen -= lifeRegenValue;

                if (damage < damageValue)
                {
                    damage = damageValue;
                }
            }
        }
        #endregion

        #region Set Defaults
        public override void SetDefaults(NPC npc)
        {
            for (int m = 0; m < maxAIMod; m++)
            {
                newAI[m] = 0f;
            }

            // Apply DR to vanilla NPCs. No vanilla NPCs have DR except in Rev+.
            // This also applies DR to other mods' NPCs who have set up their NPCs to have DR in Rev+.
            if (CalamityWorld.revenge)
            {
                CalamityMod.DRValues.TryGetValue(npc.type, out float revDR);
                DR = revDR;
            }

            if (npc.boss && CalamityWorld.revenge)
            {
                if (npc.type != mod.NPCType("HiveMindP2") && npc.type != mod.NPCType("Leviathan") && npc.type != mod.NPCType("StormWeaverHeadNaked") &&
                    npc.type != mod.NPCType("StormWeaverBodyNaked") && npc.type != mod.NPCType("StormWeaverTailNaked") &&
                    npc.type != mod.NPCType("DevourerofGodsHeadS") && npc.type != mod.NPCType("DevourerofGodsBodyS") &&
                    npc.type != mod.NPCType("DevourerofGodsTailS"))
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (!Main.LocalPlayer.dead && Main.LocalPlayer.active)
                        {
                            Main.LocalPlayer.GetCalamityPlayer().adrenaline = 0;
                        }
                    }
                }
            }

            if (CalamityWorld.bossRushActive)
			{
				BossRushStatChanges(npc, mod);
			}
            else
            {
                if (CalamityMod.enemyImmunityList.Contains(npc.type))
                {
                    npc.buffImmune[mod.BuffType("GlacialState")] = true;
                    npc.buffImmune[mod.BuffType("TemporalSadness")] = true;
                    npc.buffImmune[mod.BuffType("TimeSlow")] = true;
                }

                if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail ||
                    npc.type == NPCID.DD2EterniaCrystal ||
                    npc.townNPC)
                {
                    for (int k = 0; k < npc.buffImmune.Length; k++)
                    {
                        npc.buffImmune[k] = true;
                    }

                    if (npc.townNPC)
                    {
                        npc.buffImmune[BuffID.Wet] = false;
                        npc.buffImmune[BuffID.Slimed] = false;
                        npc.buffImmune[BuffID.Lovestruck] = false;
                        npc.buffImmune[BuffID.Stinky] = false;
                    }
                }
            }

            npc.buffImmune[mod.BuffType("Enraged")] = false;

            BossValueChanges(npc);

            if (CalamityWorld.defiled)
            {
                npc.value = (int)(npc.value * 1.5);
            }

            if (DraedonMayhem)
            {
                DraedonMechaMayhemStatChanges(npc);
            }

            if (CalamityWorld.revenge)
            {
                RevengeanceStatChanges(npc, mod);
            }

            OtherStatChanges(npc);

            if (CalamityWorld.ironHeart)
            {
                IronHeartChanges(npc);
            }
        }
        #endregion

        #region Boss Rush Stat Changes
        private void BossRushStatChanges(NPC npc, Mod mod)
        {
            if (!npc.friendly)
            {
                for (int k = 0; k < npc.buffImmune.Length; k++)
                {
                    npc.buffImmune[k] = true;
                }

                if (npc.type != mod.NPCType("DevourerofGodsHeadS") && npc.type != mod.NPCType("DevourerofGodsBodyS") && npc.type != mod.NPCType("DevourerofGodsTailS"))
                {
                    npc.buffImmune[BuffID.Ichor] = false;
                    npc.buffImmune[BuffID.CursedInferno] = false;
                }
            }

            foreach (KeyValuePair<int, int> BossRushHPChange in BossRushHPChanges)
            {
                if (npc.type == BossRushHPChange.Key)
                {
                    npc.lifeMax = BossRushHPChange.Value;
                    break;
                }
            }
        }
        #endregion

        #region Boss Value Changes
        private void BossValueChanges(NPC npc)
        {
            foreach (KeyValuePair<int, int> BossValue in BossValues)
            {
                if (npc.type == BossValue.Key)
                {
                    npc.value = BossValue.Value;
                    break;
                }
            }
        }
        #endregion

        #region Draedon Mecha Mayhem Stat Changes
        private void DraedonMechaMayhemStatChanges(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.TheDestroyer:
                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                    npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.7) : (int)(npc.lifeMax * 1.8);
                    npc.scale = 1.5f;
                    npc.npcSlots = 10f;
                    break;

                case NPCID.Probe:
                    npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.9) : (int)(npc.lifeMax * 1.6);
                    npc.scale *= CalamityWorld.death ? 1.25f : 1.2f;
                    break;

                case NPCID.SkeletronPrime:
                    npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.5) : (int)(npc.lifeMax * 1.6);
                    npc.npcSlots = 12f;
                    break;

                case NPCID.PrimeVice:
                case NPCID.PrimeCannon:
                case NPCID.PrimeSaw:
                case NPCID.PrimeLaser:
                    npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.15) : (int)(npc.lifeMax * 1.05);
                    break;

                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 3.0) : (int)(npc.lifeMax * 1.8);
                    npc.npcSlots = 10f;
                    break;
            }
        }
        #endregion

        #region Revengeance Stat Changes
        private void RevengeanceStatChanges(NPC npc, Mod mod)
        {
            npc.value = (int)(npc.value * 1.5);

            if (npc.type == NPCID.Mothron)
            {
                npc.scale = 1.25f;
            }
            else if (npc.type == NPCID.MoonLordCore)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.4) : (int)(npc.lifeMax * 1.9);
                npc.npcSlots = 36f;
            }
			else if (npc.type == NPCID.MoonLordHand || npc.type == NPCID.MoonLordHead || npc.type == NPCID.MoonLordLeechBlob)
			{
				npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.4) : (int)(npc.lifeMax * 1.2);
			}
			else if (npc.type >= NPCID.CultistDragonHead && npc.type <= NPCID.CultistDragonTail)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 10.0) : (int)(npc.lifeMax * 5.0);
            }
            else if (npc.type == NPCID.DukeFishron)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 3.525) : (int)(npc.lifeMax * 1.85);
                npc.npcSlots = 20f;
            }
            else if (npc.type == NPCID.Sharkron || npc.type == NPCID.Sharkron2)
            {
                npc.lifeMax = (int)(npc.lifeMax * 5.0);
            }
            else if (npc.type == NPCID.Golem)
			{
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 7.0) : (int)(npc.lifeMax * 4.0);
				npc.npcSlots = 64f;
			}
			else if (npc.type == NPCID.GolemHead)
			{
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.0) : (int)(npc.lifeMax * 1.5);
			}
			else if (npc.type == NPCID.GolemHeadFree)
			{
				npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.5) : (int)(npc.lifeMax * 1.25);
				npc.dontTakeDamage = false;
			}
            else if (npc.type == NPCID.Plantera)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 3.4) : (int)(npc.lifeMax * 2.3);
                npc.npcSlots = 32f;
            }
			else if (npc.type == NPCID.PlanterasHook)
			{
				npc.damage = (npc.defDamage = 0);
			}
			else if (npc.type == NPCID.WallofFlesh || npc.type == NPCID.WallofFleshEye)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.6) : (int)(npc.lifeMax * 1.9);

                if (npc.type == NPCID.WallofFlesh)
                {
                    npc.npcSlots = 20f;
                }
            }
            else if (npc.type == NPCID.TheHungryII || npc.type == NPCID.LeechHead || npc.type == NPCID.LeechBody || npc.type == NPCID.LeechTail)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.15) : (int)(npc.lifeMax * 1.05);
            }
            else if (npc.type == NPCID.SkeletronHead)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.4) : (int)(npc.lifeMax * 1.25);
                npc.npcSlots = 12f;
            }
            else if (npc.type == NPCID.SkeletronHand)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 0.9) : (int)(npc.lifeMax * 0.75);
            }
            else if (npc.type == NPCID.QueenBee)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.65) : (int)(npc.lifeMax * 1.15);
                npc.npcSlots = 14f;
            }
            else if (npc.type == NPCID.BrainofCthulhu)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.1) : (int)(npc.lifeMax * 1.6);
                npc.npcSlots = 12f;
            }
            else if (npc.type == NPCID.Creeper)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.2) : (int)(npc.lifeMax * 1.1);
            }
            else if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.4) : (int)(npc.lifeMax * 1.3);

                if (npc.type == NPCID.EaterofWorldsHead)
                {
                    npc.npcSlots = 10f;
                }
            }
            else if (npc.type == NPCID.EyeofCthulhu)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.3) : (int)(npc.lifeMax * 1.25);
                npc.npcSlots = 10f;
            }
            else if (npc.type == NPCID.KingSlime)
            {
                if (CalamityWorld.death)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 1.85);
                }
            }
            else if (npc.type == NPCID.Wraith || npc.type == NPCID.Mimic || npc.type == NPCID.Reaper || npc.type == NPCID.PresentMimic || npc.type == NPCID.SandElemental)
            {
                npc.knockBackResist = 0f;
            }

            if (!DraedonMayhem)
            {
                if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail)
                {
                    npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.6) : (int)(npc.lifeMax * 1.25);
                    npc.scale = 1.5f;
                    npc.npcSlots = 10f;
                }
                else if (npc.type == NPCID.Probe)
                {
                    if (CalamityWorld.death)
                    {
                        npc.lifeMax = (int)(npc.lifeMax * 1.25);
                        npc.scale *= 1.2f;
                    }
                }
                else if (npc.type == NPCID.SkeletronPrime)
                {
                    npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.45) : (int)(npc.lifeMax * 1.2);
                    npc.npcSlots = 12f;
                }
                else if (npc.type == NPCID.PrimeVice || npc.type == NPCID.PrimeCannon || npc.type == NPCID.PrimeSaw || npc.type == NPCID.PrimeLaser)
                {
                    if (!CalamityWorld.death)
                    {
                        npc.lifeMax = (int)(npc.lifeMax * 0.9);
                    }
                }
                else if (npc.type == NPCID.Retinazer)
                {
                    npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.4) : (int)(npc.lifeMax * 1.25);
                    npc.npcSlots = 10f;
                }
                else if (npc.type == NPCID.Spazmatism)
                {
                    npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.45) : (int)(npc.lifeMax * 1.3);
                    npc.npcSlots = 10f;
                }
            }

            if (CalamityMod.revengeanceLifeStealExceptionList.Contains(npc.type))
            {
                npc.canGhostHeal = false;
            }
        }

        /// <summary>
        /// Sets the DR of this NPC only if Revengeance Mode is enabled. Otherwise sets DR to zero.
        /// </summary>
        /// <param name="dr">The DR to set, assuming Rev+ difficulty.</param>
        /// <returns>Whether Revengeance Mode is currently enabled.</returns>
        public bool RevPlusDR(float dr)
        {
            DR = CalamityWorld.revenge ? dr : 0f;
            return CalamityWorld.revenge;
        }
        #endregion

        #region Other Stat Changes
        private void OtherStatChanges(NPC npc)
        {
            // Fix Sharkron hitboxes
            if (npc.type == NPCID.Sharkron || npc.type == NPCID.Sharkron2)
            {
                npc.width = npc.height = 36;
            }

            if (npc.type == NPCID.CultistBoss)
            {
                npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.5) : (int)(npc.lifeMax * (CalamityWorld.revenge ? 1.7 : 1.2));
                npc.npcSlots = 20f;
            }

            if (npc.type >= NPCID.TombCrawlerHead && npc.type <= NPCID.TombCrawlerTail && !Main.hardMode)
            {
                npc.lifeMax = (int)(npc.lifeMax * 0.6);
            }

            if (Main.bloodMoon && NPC.downedMoonlord && !npc.boss && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax <= 2000)
            {
                npc.lifeMax = (int)(npc.lifeMax * 3.5);
                npc.damage += 100;
                npc.life = npc.lifeMax;
                npc.defDamage = npc.damage;
            }

            if (CalamityWorld.downedDoG)
            {
                if (CalamityMod.pumpkinMoonBuffList.Contains(npc.type))
                {
                    npc.lifeMax = (int)(npc.lifeMax * 7.5);
                    npc.damage += 200;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
                else if (CalamityMod.frostMoonBuffList.Contains(npc.type))
                {
                    npc.lifeMax = (int)(npc.lifeMax * 6.0);
                    npc.damage += 200;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
            }

            if (CalamityMod.eclipseBuffList.Contains(npc.type) && CalamityWorld.buffedEclipse)
            {
                npc.lifeMax = (int)(npc.lifeMax * 32.5);
                npc.damage += 250;
                npc.life = npc.lifeMax;
                npc.defDamage = npc.damage;
            }

            if (NPC.downedMoonlord)
            {
                if (CalamityMod.dungeonEnemyBuffList.Contains(npc.type))
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.5);
                    npc.damage += 150;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
            }

            if (CalamityWorld.revenge)
            {
                if (CalamityMod.revengeanceEnemyBuffList.Contains(npc.type))
                {
                    npc.damage = (int)(npc.damage * 1.25);
                    npc.defDamage = npc.damage;
                }
            }

            if ((npc.boss && npc.type != NPCID.MartianSaucerCore && npc.type < NPCID.Count) || CalamityMod.bossHPScaleList.Contains(npc.type))
            {
                double HPBoost = Config.BossHealthPercentageBoost * 0.01;
                npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            }
        }
        #endregion

        // TODO: Change Iron Heart damage in here for Iron Heart mode
        #region Iron Heart Changes
        private void IronHeartChanges(NPC npc)
		{
			// Iron Heart damage variable will scale with npc.damage
			ironHeartDamage = 0;
		}
        #endregion

        #region Scale Expert Multiplayer Stats
        public override void ScaleExpertStats(NPC npc, int numPlayers, float bossLifeScale)
        {
            if (CalamityWorld.revenge)
            {
                ScaleThoriumBossHealth(npc, mod);
            }

            if (Main.netMode == NetmodeID.SinglePlayer || numPlayers <= 1)
            {
                return;
            }

            if (((npc.boss || CalamityMod.bossScaleList.Contains(npc.type)) && npc.type < NPCID.Count) ||
                (npc.modNPC != null && npc.modNPC.mod.Name.Equals("CalamityMod")))
            {
                double scalar = 1.0;
                switch (numPlayers) //Decrease HP in multiplayer before vanilla scaling
                {
                    case 2:
                        scalar = 0.76;
                        break;

                    case 3:
                        scalar = 0.63;
                        break;

                    case 4:
                        scalar = 0.525;
                        break;

                    case 5:
                        scalar = 0.43;
                        break;

                    case 6:
                        scalar = 0.36;
                        break;

                    default:
                        scalar = 0.295;
                        break;
                }

                npc.lifeMax = (int)(npc.lifeMax * scalar);
            }
        }
        #endregion

        #region Scale Thorium Boss Health
        private void ScaleThoriumBossHealth(NPC npc, Mod mod)
        {
            if (Config.RevengeanceAndDeathThoriumBossBuff)
            {
                Mod thorium = ModLoader.GetMod("ThoriumMod");
                if (thorium != null)
                {
                    if (npc.type == thorium.NPCType("Hatchling") || npc.type == thorium.NPCType("DistractJelly") || npc.type == thorium.NPCType("ViscountBaby") ||
                        npc.type == thorium.NPCType("BoreanHopper") || npc.type == thorium.NPCType("BoreanMyte1") || npc.type == thorium.NPCType("EnemyBeholder") ||
                        npc.type == thorium.NPCType("ThousandSoulPhalactry") || npc.type == thorium.NPCType("AbyssalSpawn"))
                    {
                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.3) : (int)(npc.lifeMax * 1.5);
                    }
                    else if (npc.type == thorium.NPCType("TheGrandThunderBirdv2"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.3) : (int)(npc.lifeMax * 1.5);
                    }
                    else if (npc.type == thorium.NPCType("QueenJelly"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.9) : (int)(npc.lifeMax * 1.35);
                    }
                    else if (npc.type == thorium.NPCType("Viscount"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.65) : (int)(npc.lifeMax * 1.25);
                    }
                    else if (npc.type == thorium.NPCType("GraniteEnergyStorm"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.25) : (int)(npc.lifeMax * 1.1);
                    }
                    else if (npc.type == thorium.NPCType("TheBuriedWarrior") || npc.type == thorium.NPCType("TheBuriedWarrior1") || npc.type == thorium.NPCType("TheBuriedWarrior2"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.5) : (int)(npc.lifeMax * 1.2);
                    }
                    else if (npc.type == thorium.NPCType("ThePrimeScouter") || npc.type == thorium.NPCType("CryoCore") || npc.type == thorium.NPCType("BioCore") ||
                        npc.type == thorium.NPCType("PyroCore"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.75) : (int)(npc.lifeMax * 1.3);
                    }
                    else if (npc.type == thorium.NPCType("BoreanStrider") || npc.type == thorium.NPCType("BoreanStriderPopped"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.0) : (int)(npc.lifeMax * 1.4);
                    }
                    else if (npc.type == thorium.NPCType("FallenDeathBeholder") || npc.type == thorium.NPCType("FallenDeathBeholder2"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.5) : (int)(npc.lifeMax * 1.6);
                    }
                    else if (npc.type == thorium.NPCType("Lich") || npc.type == thorium.NPCType("LichHeadless"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.5) : (int)(npc.lifeMax * 1.6);
                    }
                    else if (npc.type == thorium.NPCType("Abyssion") || npc.type == thorium.NPCType("AbyssionCracked") || npc.type == thorium.NPCType("AbyssionReleased"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 2.3) : (int)(npc.lifeMax * 1.5);
                    }
                    else if (npc.type == thorium.NPCType("SlagFury") || npc.type == thorium.NPCType("Omnicide") || npc.type == thorium.NPCType("RealityBreaker") ||
                        npc.type == thorium.NPCType("Aquaius") || npc.type == thorium.NPCType("Aquaius2"))
                    {
                        npc.buffImmune[mod.BuffType("GlacialState")] = true;
                        npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

                        npc.lifeMax = CalamityWorld.death ? (int)(npc.lifeMax * 1.75) : (int)(npc.lifeMax * 1.3);
                    }
                }
            }
        }
        #endregion

        #region Can Hit Player
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (!npc.boss && !npc.friendly && !npc.dontTakeDamage)
            {
                if (CalamityWorld.downedDoG && (Main.pumpkinMoon || Main.snowMoon))
                {
                    cooldownSlot = 1;
                }

                if(CalamityWorld.buffedEclipse && Main.eclipse)
                {
                    cooldownSlot = 1;
                }
            }

            return true;
        }
        #endregion

        #region Modify Hit Player
        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (tSad)
            {
                damage /= 2;
            }

            if (target.GetCalamityPlayer().beeResist)
            {
                if (CalamityMod.beeEnemyList.Contains(npc.type))
                {
                    damage = (int)(damage * 0.75);
                }
            }
        }
        #endregion

        #region Strike NPC
        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            // TODO -- move this to Destroyer's Rev+ AI; either set his DR or modify npc.takenDamageMultiplier
            if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail)
            {
				if ((newAI[1] < 480f || newAI[2] > 0f) && (CalamityWorld.revenge || CalamityWorld.bossRushActive))
				{
					damage *= 0.01;
				}
			}

			// Override hand/head eye 'death' code and use custom 'death' code instead, this is here just in case the AI code fails
			if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
			{
				if (npc.type == NPCID.MoonLordHand || npc.type == NPCID.MoonLordHead)
				{
					if (npc.life - (int)damage <= 0)
					{
						if (newAI[0] != 1f)
						{
							newAI[0] = 1f;
							npc.life = npc.lifeMax;
							npc.netUpdate = true;
							npc.dontTakeDamage = true;
						}
					}
				}
			}

			// Yellow Candle provides +5% damage which ignores both DR and defense.
			// However, armor penetration bonus damage has already been applied, so it's slightly higher than it should be.
			double yellowCandleDamage = 0.05 * damage;

            // Apply modifications to enemy's current defense based on Calamity debuffs.
            // As with defense and DR, flat reductions apply first, then multiplicative reductions.
            int effectiveDefense = npc.defense -
                    (pFlames ? Plague.DefenseReduction : 0) -
                    (wDeath ? WhisperingDeath.DefenseReduction : 0) -
                    (gsInferno ? GodSlayerInferno.DefenseReduction : 0) -
                    (astralInfection ? AstralInfectionDebuff.DefenseReduction : 0) -
                    (aFlames ? AbyssalFlames.DefenseReduction : 0) -
                    (wCleave ? WarCleave.DefenseReduction : 0);

            if (gState)
                effectiveDefense /= 2;
            if (aCrunch)
                effectiveDefense /= 3;

            // Defense can never be negative and has a minimum value of zero.
            if (effectiveDefense < 0)
                effectiveDefense = 0;

            // Apply vanilla-style defense before DR, using Calamity's reduced defense.
            damage = Main.CalculateDamage((int)damage, effectiveDefense);

            // DR applies after vanilla defense.
            damage = ApplyDR(npc, damage);

            // Add Yellow Candle damage if the NPC isn't supposed to be "near invincible"
            if (yellowCandle && DR < 0.99f && npc.takenDamageMultiplier > 0.05f)
                damage += yellowCandleDamage;

            // Cancel out vanilla defense math by reversing the calculation vanilla is about to perform.
            // While roundabout, this is safer than returning false to stop vanilla damage calculations entirely.
            // Other mods will probably expect the vanilla code to run and may compensate for it themselves.
            damage = Main.CalculateDamage((int)damage, -defense);
            return true;
        }

        /// <summary>
        /// Modifies damage incoming to an NPC based on their DR (damage reduction) stat added by Calamity.<br></br>
        /// This is entirely separate from vanilla's takenDamageMultiplier.
        /// </summary>
        /// <param name="damage">Incoming damage. Has been modified by Main.DamageVar and boosted by armor penetration, but nothing else.</param>
        /// <returns></returns>
        private double ApplyDR(NPC npc, double damage)
        {
            if (DR <= 0f || damage <= 1.0)
                return damage;

            // If the NPC currently has unbreakable DR, it cannot be reduced by any means.
            // If custom DR is enabled, use that instead of normal DR.
            float effectiveDR = unbreakableDR ? DR : (customDR ? CustomDRMath(npc, DR) : DefaultDRMath(npc, DR));

            // DR floor is 0%. Nothing can have negative DR.
            if (effectiveDR <= 0f)
                effectiveDR = 0f;

            double newDamage = (1f - effectiveDR) * damage;
            return newDamage < 1.0 ? 1.0 : newDamage;
        }

        private float DefaultDRMath(NPC npc, float DR)
        {
            float calcDR = DR;
            if (marked)
                calcDR *= 0.5f;
            if (npc.betsysCurse)
                calcDR *= 0.66f;
            if (wCleave)
                calcDR *= 0.75f;

            // Ichor supersedes Cursed Inferno if both are applied.
            if (npc.ichor)
                calcDR *= 0.75f;
            else if (npc.onFire2)
                calcDR *= 0.8f;

            return calcDR;
        }

        private float CustomDRMath(NPC npc, float DR)
        {
            void FlatEditDR(ref float theDR, bool npcHasDebuff, int buffID)
            {
                if (npcHasDebuff && flatDRReductions.TryGetValue(buffID, out float reduction))
                    theDR -= reduction;
            }
            void MultEditDR(ref float theDR, bool npcHasDebuff, int buffID)
            {
                if (npcHasDebuff && multDRReductions.TryGetValue(buffID, out float multiplier))
                    theDR *= multiplier;
            }

            float calcDR = DR;

            // Apply flat reductions first. All vanilla debuffs check their internal booleans.
            FlatEditDR(ref calcDR, npc.poisoned, BuffID.Poisoned);
            FlatEditDR(ref calcDR, npc.onFire, BuffID.OnFire);
            FlatEditDR(ref calcDR, npc.venom, BuffID.Venom);
            FlatEditDR(ref calcDR, npc.onFrostBurn, BuffID.Frostburn);
            FlatEditDR(ref calcDR, npc.shadowFlame, BuffID.ShadowFlame);
            FlatEditDR(ref calcDR, npc.daybreak, BuffID.Daybreak);
            FlatEditDR(ref calcDR, npc.betsysCurse, BuffID.BetsysCurse);

            // Ichor supersedes Cursed Inferno if both are applied.
            FlatEditDR(ref calcDR, npc.ichor, BuffID.Ichor);
            FlatEditDR(ref calcDR, npc.onFire2 && !npc.ichor, BuffID.CursedInferno);

            // Modded debuffs are handled modularly and use HasBuff.
            foreach (KeyValuePair<int, float> entry in flatDRReductions)
            {
                int buffID = entry.Key;
                if (buffID >= BuffID.Count && npc.HasBuff(buffID))
                    calcDR -= entry.Value;
            }

            // Apply multiplicative reductions second. All vanilla debuffs check their internal booleans.
            MultEditDR(ref calcDR, npc.poisoned, BuffID.Poisoned);
            MultEditDR(ref calcDR, npc.onFire, BuffID.OnFire);
            MultEditDR(ref calcDR, npc.venom, BuffID.Venom);
            MultEditDR(ref calcDR, npc.onFrostBurn, BuffID.Frostburn);
            MultEditDR(ref calcDR, npc.shadowFlame, BuffID.ShadowFlame);
            MultEditDR(ref calcDR, npc.daybreak, BuffID.Daybreak);
            MultEditDR(ref calcDR, npc.betsysCurse, BuffID.BetsysCurse);

            // Ichor supersedes Cursed Inferno if both are applied.
            MultEditDR(ref calcDR, npc.ichor, BuffID.Ichor);
            MultEditDR(ref calcDR, npc.onFire2 && !npc.ichor, BuffID.CursedInferno);

            // Modded debuffs are handled modularly and use HasBuff.
            foreach (KeyValuePair<int, float> entry in multDRReductions)
            {
                int buffID = entry.Key;
                if (buffID >= BuffID.Count && npc.HasBuff(buffID))
                    calcDR *= entry.Value;
            }

            return calcDR;
        }
        #endregion

        #region Boss Head Slot
        public override void BossHeadSlot(NPC npc, ref int index)
		{
			if (CalamityWorld.revenge)
			{
				if (npc.type == NPCID.BrainofCthulhu)
				{
					if ((float)npc.life / (float)npc.lifeMax < (CalamityWorld.death ? 0.33f : 0.2f))
						index = -1;
				}

				if (CalamityWorld.death)
				{
					if (npc.type == NPCID.DukeFishron)
					{
						if ((float)npc.life / (float)npc.lifeMax < 0.15f)
							index = -1;
					}
				}
			}
		}
		#endregion

		#region Pre AI
		public override bool PreAI(NPC npc)
        {
            SetPatreonTownNPCName(npc);

            if (npc.type == NPCID.TargetDummy || npc.type == mod.NPCType("SuperDummy"))
            {
                npc.chaseable = !CalamityPlayer.areThereAnyDamnBosses;
                npc.dontTakeDamage = CalamityPlayer.areThereAnyDamnBosses;
            }

            if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail ||
                npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
            {
                npc.buffImmune[mod.BuffType("Enraged")] = false;
            }

            if (npc.type == NPCID.Bee || npc.type == NPCID.BeeSmall || npc.type == NPCID.Hornet || npc.type == NPCID.HornetFatty || npc.type == NPCID.HornetHoney ||
                npc.type == NPCID.HornetLeafy || npc.type == NPCID.HornetSpikey || npc.type == NPCID.HornetStingy || npc.type == NPCID.BigHornetStingy || npc.type == NPCID.LittleHornetStingy ||
                npc.type == NPCID.BigHornetSpikey || npc.type == NPCID.LittleHornetSpikey || npc.type == NPCID.BigHornetLeafy || npc.type == NPCID.LittleHornetLeafy ||
                npc.type == NPCID.BigHornetHoney || npc.type == NPCID.LittleHornetHoney || npc.type == NPCID.BigHornetFatty || npc.type == NPCID.LittleHornetFatty)
            {
                if (Main.player[npc.target].GetCalamityPlayer().queenBeeLore)
                {
                    CalamityGlobalAI.QueenBeeLoreEffect(npc);
                    return false;
                }
            }

			// Always prevent true eye of cthulhu appearance in rev+
			if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
			{
				if (npc.type == NPCID.MoonLordFreeEye)
				{
					npc.active = false;
					npc.netUpdate = true;
				}
			}

			if (CalamityWorld.bossRushActive && !npc.friendly && !npc.townNPC)
            {
                BossRushForceDespawnOtherNPCs(npc, mod);
            }

            if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
            {
                switch (npc.type)
                {
                    case NPCID.KingSlime:
                        return CalamityGlobalAI.BuffedKingSlimeAI(npc, mod);

                    case NPCID.EyeofCthulhu:
                        return CalamityGlobalAI.BuffedEyeofCthulhuAI(npc, enraged, mod);

                    case NPCID.EaterofWorldsHead:
                    case NPCID.EaterofWorldsBody:
                    case NPCID.EaterofWorldsTail:
                        return CalamityGlobalAI.BuffedEaterofWorldsAI(npc, mod);

                    case NPCID.BrainofCthulhu:
                        return CalamityGlobalAI.BuffedBrainofCthulhuAI(npc, enraged, mod);
                    case NPCID.Creeper:
                        return CalamityGlobalAI.BuffedCreeperAI(npc, enraged, mod);

                    case NPCID.QueenBee:
                        return CalamityGlobalAI.BuffedQueenBeeAI(npc, mod);

                    case NPCID.SkeletronHand:
                        return CalamityGlobalAI.BuffedSkeletronHandAI(npc, enraged, mod);
                    case NPCID.SkeletronHead:
                        return CalamityGlobalAI.BuffedSkeletronAI(npc, enraged, mod);

                    case NPCID.WallofFlesh:
                        return CalamityGlobalAI.BuffedWallofFleshAI(npc, enraged, mod);
                    case NPCID.WallofFleshEye:
                        return CalamityGlobalAI.BuffedWallofFleshEyeAI(npc, enraged, mod);

                    case NPCID.TheDestroyer:
                    case NPCID.TheDestroyerBody:
                    case NPCID.TheDestroyerTail:
                        return CalamityGlobalAI.BuffedDestroyerAI(npc, enraged, mod);

                    case NPCID.Retinazer:
                        return CalamityGlobalAI.BuffedRetinazerAI(npc, enraged, mod);
                    case NPCID.Spazmatism:
                        return CalamityGlobalAI.BuffedSpazmatismAI(npc, enraged, mod);

                    case NPCID.SkeletronPrime:
                        return CalamityGlobalAI.BuffedSkeletronPrimeAI(npc, enraged, mod);
                    case NPCID.PrimeLaser:
                        return CalamityGlobalAI.BuffedPrimeLaserAI(npc, mod);
                    case NPCID.PrimeCannon:
                        return CalamityGlobalAI.BuffedPrimeCannonAI(npc, mod);
                    case NPCID.PrimeVice:
                        return CalamityGlobalAI.BuffedPrimeViceAI(npc, mod);
                    case NPCID.PrimeSaw:
                        return CalamityGlobalAI.BuffedPrimeSawAI(npc, mod);

                    case NPCID.Plantera:
                        return CalamityGlobalAI.BuffedPlanteraAI(npc, enraged, mod);
                    case NPCID.PlanterasHook:
                        return CalamityGlobalAI.BuffedPlanterasHookAI(npc, mod);
                    case NPCID.PlanterasTentacle:
                        return CalamityGlobalAI.BuffedPlanterasTentacleAI(npc, mod);

                    case NPCID.Golem:
						return CalamityGlobalAI.BuffedGolemAI(npc, enraged, mod);
					case NPCID.GolemHead:
						return CalamityGlobalAI.BuffedGolemHeadAI(npc, enraged, mod);
					case NPCID.GolemHeadFree:
						return CalamityGlobalAI.BuffedGolemHeadFreeAI(npc, enraged, mod);

                    case NPCID.DukeFishron:
                        return CalamityGlobalAI.BuffedDukeFishronAI(npc, enraged, mod);

                    case NPCID.Pumpking:
                        if (CalamityWorld.downedDoG)
                        {
                            return CalamityGlobalAI.BuffedPumpkingAI(npc);
                        }

                        break;

                    case NPCID.PumpkingBlade:
                        if (CalamityWorld.downedDoG)
                        {
                            return CalamityGlobalAI.BuffedPumpkingBladeAI(npc);
                        }

                        break;

                    case NPCID.IceQueen:
                        if (CalamityWorld.downedDoG)
                        {
                            return CalamityGlobalAI.BuffedIceQueenAI(npc);
                        }

                        break;

                    case NPCID.Mothron:
                        if (CalamityWorld.buffedEclipse)
                        {
                            return CalamityGlobalAI.BuffedMothronAI(npc);
                        }

                        break;

					case NPCID.CultistBoss:
					case NPCID.CultistBossClone:
						return CalamityGlobalAI.BuffedCultistAI(npc, enraged, mod);
					case NPCID.AncientDoom:
						return CalamityGlobalAI.BuffedAncientDoomAI(npc, mod);

					case NPCID.MoonLordCore:
					case NPCID.MoonLordHand:
					case NPCID.MoonLordHead:
					case NPCID.MoonLordLeechBlob:
						return CalamityGlobalAI.BuffedMoonLordAI(npc, enraged, mod);

					default:
                        break;
                }
            }

            return true;
        }
        #endregion

        #region Set Patreon Town NPC Name
        private void SetPatreonTownNPCName(NPC npc)
        {
            if (setNewName)
            {
                setNewName = false;
                switch (npc.type)
                {
                    case NPCID.Guide:
                        switch (Main.rand.Next(36)) // 34 guide names
                        {
                            case 0:
                                npc.GivenName = "Lapp";
                                break;

                            case 1:
                                npc.GivenName = "Ben Shapiro";
                                break;

                            default:
                                break;
                        }

                        break;

                    case NPCID.Wizard:
                        switch (Main.rand.Next(24)) // 23 wizard names
                        {
                            case 0:
                                npc.GivenName = "Mage One-Trick";
                                break;

                            default:
                                break;
                        }

                        break;

                    case NPCID.Steampunker:
                        switch (Main.rand.Next(22)) // 21 steampunker names
                        {
                            case 0:
                                npc.GivenName = "Vorbis";
                                break;

                            default:
                                break;
                        }

                        break;

                    case NPCID.Stylist:
                        switch (Main.rand.Next(21)) // 20 stylist names
                        {
                            case 0:
                                npc.GivenName = "Amber";
                                break;

                            default:
                                break;
                        }

                        break;

                    case NPCID.WitchDoctor:
                        switch (Main.rand.Next(11)) // 10 witch doctor names
                        {
                            case 0:
                                npc.GivenName = "Sok'ar";
                                break;

                            default:
                                break;
                        }

                        break;

                    case NPCID.TaxCollector:
                        switch (Main.rand.Next(21)) // 20 tax collector names
                        {
                            case 0:
                                npc.GivenName = "Emmett";
                                break;

                            default:
                                break;
                        }

                        break;

                    default:
                        break;
                }
            }
        }
        #endregion

        #region Boss Rush Force Despawn Other NPCs
        private void BossRushForceDespawnOtherNPCs(NPC npc, Mod mod)
        {
            switch (CalamityWorld.bossRushStage)
            {
                case 0:
					if (npc.type != NPCID.QueenBee)
					{
						npc.active = false;
						npc.netUpdate = true;
					}

					break;

                case 1:
                    if (npc.type != NPCID.BrainofCthulhu && npc.type != NPCID.Creeper)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 2:
					if (npc.type != NPCID.KingSlime && npc.type != NPCID.BlueSlime && npc.type != NPCID.SlimeSpiked && npc.type != mod.NPCType("KingSlimeJewel") &&
						npc.type != NPCID.YellowSlime && npc.type != NPCID.PurpleSlime && npc.type != NPCID.GreenSlime && npc.type != NPCID.RedSlime &&
						npc.type != NPCID.IceSlime && npc.type != NPCID.UmbrellaSlime && npc.type != NPCID.RainbowSlime && npc.type != NPCID.Pinky)
					{
						npc.active = false;
						npc.netUpdate = true;
					}

					break;

                case 3:
                    if (npc.type != NPCID.EyeofCthulhu && npc.type != NPCID.ServantofCthulhu)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 4:
					if (npc.type != NPCID.SkeletronPrime && npc.type != NPCID.PrimeSaw && npc.type != NPCID.PrimeVice &&
						npc.type != NPCID.PrimeCannon && npc.type != NPCID.PrimeLaser && npc.type != NPCID.Probe)
					{
						npc.active = false;
						npc.netUpdate = true;
					}

					break;

                case 5:
                    if (npc.type != NPCID.Golem && npc.type != NPCID.GolemFistLeft && npc.type != NPCID.GolemFistRight &&
                        npc.type != NPCID.GolemHead && npc.type != NPCID.GolemHeadFree)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 6:
                    if (npc.type != mod.NPCType("ProfanedGuardianBoss") && npc.type != mod.NPCType("ProfanedGuardianBoss2") &&
                        npc.type != mod.NPCType("ProfanedGuardianBoss3"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 7:
                    if (npc.type != NPCID.EaterofWorldsHead && npc.type != NPCID.EaterofWorldsBody && npc.type != NPCID.EaterofWorldsTail &&
                        npc.type != NPCID.VileSpit)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 8:
					if (npc.type != mod.NPCType("Astrageldon") && npc.type != mod.NPCType("AstrageldonSlime"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}

					break;

                case 9:
                    if (npc.type != NPCID.TheDestroyer && npc.type != NPCID.TheDestroyerBody && npc.type != NPCID.TheDestroyerTail &&
                        npc.type != NPCID.Probe)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 10:
                    if (npc.type != NPCID.Spazmatism && npc.type != NPCID.Retinazer)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 11:
                    if (npc.type != mod.NPCType("Bumblefuck") && npc.type != mod.NPCType("Bumblefuck2") &&
                        npc.type != NPCID.Spazmatism && npc.type != NPCID.Retinazer)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 12:
                    if (npc.type != NPCID.WallofFlesh && npc.type != NPCID.WallofFleshEye && npc.type != NPCID.TheHungry &&
                        npc.type != NPCID.TheHungryII && npc.type != NPCID.LeechHead && npc.type != NPCID.LeechBody &&
                        npc.type != NPCID.LeechTail)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 13:
                    if (npc.type != mod.NPCType("HiveMind") && npc.type != mod.NPCType("HiveMindP2") &&
                        npc.type != mod.NPCType("DarkHeart") && npc.type != mod.NPCType("HiveBlob") &&
                        npc.type != mod.NPCType("DankCreeper") && npc.type != mod.NPCType("HiveBlob2"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 14:
                    if (npc.type != NPCID.SkeletronHead && npc.type != NPCID.SkeletronHand)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 15:
                    if (npc.type != mod.NPCType("StormWeaverHead") && npc.type != mod.NPCType("StormWeaverBody") &&
                        npc.type != mod.NPCType("StormWeaverTail") && npc.type != mod.NPCType("StormWeaverHeadNaked") &&
                        npc.type != mod.NPCType("StormWeaverBodyNaked") && npc.type != mod.NPCType("StormWeaverTailNaked") &&
                        npc.type != mod.NPCType("StasisProbe") && npc.type != mod.NPCType("StasisProbeNaked"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 16:
                    if (npc.type != mod.NPCType("AquaticScourgeHead") && npc.type != mod.NPCType("AquaticScourgeBody") &&
                        npc.type != mod.NPCType("AquaticScourgeBodyAlt") && npc.type != mod.NPCType("AquaticScourgeTail") &&
                        npc.type != mod.NPCType("AquaticParasite") && npc.type != mod.NPCType("AquaticUrchin") &&
                        npc.type != mod.NPCType("AquaticSeekerHead") && npc.type != mod.NPCType("AquaticSeekerBody") &&
                        npc.type != mod.NPCType("AquaticSeekerTail"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 17:
                    if (npc.type != mod.NPCType("DesertScourgeHead") && npc.type != mod.NPCType("DesertScourgeBody") &&
                        npc.type != mod.NPCType("DesertScourgeTail") && npc.type != mod.NPCType("DesertScourgeHeadSmall") &&
                        npc.type != mod.NPCType("DesertScourgeBodySmall") && npc.type != mod.NPCType("DesertScourgeTailSmall") &&
                        npc.type != mod.NPCType("DriedSeekerHead") && npc.type != mod.NPCType("DriedSeekerBody") &&
                        npc.type != mod.NPCType("DriedSeekerTail"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 18:
					if (npc.type != NPCID.CultistBoss && npc.type != NPCID.CultistBossClone && npc.type != NPCID.CultistDragonHead &&
						npc.type != NPCID.CultistDragonBody1 && npc.type != NPCID.CultistDragonBody2 && npc.type != NPCID.CultistDragonBody3 &&
						npc.type != NPCID.CultistDragonBody4 && npc.type != NPCID.CultistDragonTail && npc.type != NPCID.AncientCultistSquidhead &&
						npc.type != NPCID.AncientLight && npc.type != NPCID.AncientDoom)
					{
						npc.active = false;
						npc.netUpdate = true;
					}

					break;

                case 19:
                    if (npc.type != mod.NPCType("CrabulonIdle") && npc.type != mod.NPCType("CrabShroom"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 20:
                    if (npc.type != NPCID.Plantera && npc.type != NPCID.PlanterasTentacle && npc.type != NPCID.PlanterasHook &&
                        npc.type != NPCID.Spore)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 21:
                    if (npc.type != mod.NPCType("CeaselessVoid") && npc.type != mod.NPCType("DarkEnergy") &&
                        npc.type != mod.NPCType("DarkEnergy2") && npc.type != mod.NPCType("DarkEnergy3"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 22:
                    if (npc.type != mod.NPCType("PerforatorHive") && npc.type != mod.NPCType("PerforatorHeadLarge") &&
                        npc.type != mod.NPCType("PerforatorBodyLarge") && npc.type != mod.NPCType("PerforatorTailLarge") &&
                        npc.type != mod.NPCType("PerforatorHeadMedium") && npc.type != mod.NPCType("PerforatorBodyMedium") &&
                        npc.type != mod.NPCType("PerforatorTailMedium") && npc.type != mod.NPCType("PerforatorHeadSmall") &&
                        npc.type != mod.NPCType("PerforatorBodySmall") && npc.type != mod.NPCType("PerforatorTailSmall"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 23:
                    if (npc.type != mod.NPCType("Cryogen") && npc.type != mod.NPCType("CryogenIce") &&
                        npc.type != mod.NPCType("IceMass") && npc.type != mod.NPCType("Cryocore") &&
                        npc.type != mod.NPCType("Cryocore2"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 24:
                    if (npc.type != mod.NPCType("BrimstoneElemental") && npc.type != mod.NPCType("Brimling"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 25:
                    if (npc.type != mod.NPCType("CosmicWraith") && npc.type != mod.NPCType("SignusBomb") &&
                        npc.type != mod.NPCType("CosmicLantern"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 26:
                    if (npc.type != mod.NPCType("ScavengerBody") && npc.type != mod.NPCType("ScavengerHead") &&
                        npc.type != mod.NPCType("ScavengerClawLeft") && npc.type != mod.NPCType("ScavengerClawRight") &&
                        npc.type != mod.NPCType("ScavengerLegLeft") && npc.type != mod.NPCType("ScavengerLegRight") &&
                        npc.type != mod.NPCType("ScavengerHead2") && npc.type != mod.NPCType("RockPillar") &&
                        npc.type != mod.NPCType("FlamePillar"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 27:
                    if (npc.type != NPCID.DukeFishron && npc.type != NPCID.DetonatingBubble && npc.type != NPCID.Sharkron &&
                        npc.type != NPCID.Sharkron2)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 28:
					if (npc.type != NPCID.MoonLordCore && npc.type != NPCID.MoonLordHead && npc.type != NPCID.MoonLordHand &&
						npc.type != NPCID.MoonLordLeechBlob)
					{
						npc.active = false;
						npc.netUpdate = true;
					}

					break;

                case 29:
                    if (npc.type != mod.NPCType("AstrumDeusHead") && npc.type != mod.NPCType("AstrumDeusBody") &&
                        npc.type != mod.NPCType("AstrumDeusTail") && npc.type != mod.NPCType("AstrumDeusHeadSpectral") &&
                        npc.type != mod.NPCType("AstrumDeusBodySpectral") && npc.type != mod.NPCType("AstrumDeusTailSpectral") &&
                        npc.type != mod.NPCType("AstrumDeusProbe") && npc.type != mod.NPCType("AstrumDeusProbe2") &&
                        npc.type != mod.NPCType("AstrumDeusProbe3"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 30:
                    if (npc.type != mod.NPCType("Polterghast") && npc.type != mod.NPCType("PhantomFuckYou") &&
                        npc.type != mod.NPCType("PolterghastHook") && npc.type != mod.NPCType("PolterPhantom"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 31:
                    if (npc.type != mod.NPCType("PlaguebringerGoliath") && npc.type != mod.NPCType("PlagueBeeG") &&
                        npc.type != mod.NPCType("PlagueBeeLargeG") && npc.type != mod.NPCType("PlagueHomingMissile") &&
                        npc.type != mod.NPCType("PlagueMine") && npc.type != mod.NPCType("PlaguebringerShade"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 32:
                    if (npc.type != mod.NPCType("Calamitas") && npc.type != mod.NPCType("CalamitasRun") &&
                        npc.type != mod.NPCType("CalamitasRun2") && npc.type != mod.NPCType("CalamitasRun3") &&
                        npc.type != mod.NPCType("LifeSeeker") && npc.type != mod.NPCType("SoulSeeker"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 33:
                    if (npc.type != mod.NPCType("Siren") && npc.type != mod.NPCType("Leviathan") &&
                        npc.type != mod.NPCType("AquaticAberration") && npc.type != mod.NPCType("Parasea") &&
                        npc.type != mod.NPCType("SirenClone") && npc.type != mod.NPCType("SirenIce"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 34:
                    if (npc.type != mod.NPCType("SlimeGod") && npc.type != mod.NPCType("SlimeGodRun") &&
                        npc.type != mod.NPCType("SlimeGodCore") && npc.type != mod.NPCType("SlimeGodSplit") &&
                        npc.type != mod.NPCType("SlimeGodRunSplit") && npc.type != mod.NPCType("SlimeSpawnCorrupt") &&
                        npc.type != mod.NPCType("SlimeSpawnCorrupt2") && npc.type != mod.NPCType("SlimeSpawnCrimson") &&
                        npc.type != mod.NPCType("SlimeSpawnCrimson2"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 35:
                    if (npc.type != mod.NPCType("Providence") && npc.type != mod.NPCType("ProvSpawnDefense") &&
                        npc.type != mod.NPCType("ProvSpawnOffense") && npc.type != mod.NPCType("ProvSpawnHealer"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 36:
                    if (npc.type != mod.NPCType("SupremeCalamitas") && npc.type != mod.NPCType("SCalWormBody") &&
                        npc.type != mod.NPCType("SCalWormBodyWeak") && npc.type != mod.NPCType("SCalWormHead") &&
                        npc.type != mod.NPCType("SCalWormTail") && npc.type != mod.NPCType("SoulSeekerSupreme") &&
                        npc.type != mod.NPCType("SCalWormHeart") && npc.type != mod.NPCType("SupremeCataclysm") &&
                        npc.type != mod.NPCType("SupremeCatastrophe"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;

                case 37:
					if (npc.type != mod.NPCType("Yharon") && npc.type != mod.NPCType("DetonatingFlare") &&
						npc.type != mod.NPCType("DetonatingFlare2"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}

					break;

                case 38:
                    if (npc.type != mod.NPCType("DevourerofGodsHeadS") && npc.type != mod.NPCType("DevourerofGodsBodyS") &&
                        npc.type != mod.NPCType("DevourerofGodsTailS"))
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    break;
            }
        }
        #endregion

        #region AI
        public override void AI(NPC npc)
		{
			if (!CalamityWorld.spawnedHardBoss)
			{
				if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer || npc.type == NPCID.SkeletronPrime ||
					npc.type == NPCID.Plantera || npc.type == mod.NPCType("Cryogen") || npc.type == mod.NPCType("AquaticScourgeHead") ||
					npc.type == mod.NPCType("BrimstoneElemental") || npc.type == mod.NPCType("Astrageldon") || npc.type == mod.NPCType("AstrumDeusHeadSpectral") ||
					npc.type == mod.NPCType("Calamitas") || npc.type == mod.NPCType("Siren") || npc.type == mod.NPCType("PlaguebringerGoliath") ||
					npc.type == mod.NPCType("ScavengerBody") || npc.type == NPCID.DukeFishron || npc.type == NPCID.CultistBoss || npc.type == NPCID.Golem)
				{
					CalamityWorld.spawnedHardBoss = true;
					CalamityMod.UpdateServerBoolean();
				}
			}

			if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
			{
				bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

				switch (npc.type)
				{
					case NPCID.DungeonGuardian:
						CalamityGlobalAI.RevengeanceDungeonGuardianAI(npc, configBossRushBoost, enraged);
						break;

					case NPCID.Lihzahrd:
						CalamityGlobalAI.RevengeanceLihzahrdAI(npc);
						break;

					case NPCID.IceGolem:
						CalamityGlobalAI.RevengeanceIceGolemAI(npc);
						break;

					default:
						break;
				}
			}
		}
		#endregion

		#region Post AI
		public override void PostAI(NPC npc)
		{
            // Bosses and any specific other NPCs are completely immune to having their movement impaired.
            if (npc.boss || CalamityMod.movementImpairImmuneList.Contains(npc.type))
                return;

            if (pearlAura && !CalamityPlayer.areThereAnyDamnBosses)
                npc.velocity *= 0.95f;

            if (!CalamityWorld.bossRushActive)
			{
				if (silvaStun)
                    npc.velocity = Vector2.Zero;
				else if (timeSlow)
                    npc.velocity *= 0.85f;
			}
		}
        #endregion

        #region On Hit Player
        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            if (target.GetCalamityPlayer().snowman)
            {
                if (npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon || npc.type == NPCID.RedDevil)
                {
                    target.AddBuff(mod.BuffType("PopoNoseless"), 36000);
                }
            }

            switch (npc.type)
            {
                case NPCID.Golem:
                    target.AddBuff(mod.BuffType("ArmorCrunch"), 300);
                    break;

                case NPCID.GolemHead:
                case NPCID.GolemFistRight:
                case NPCID.GolemFistLeft:
                    target.AddBuff(mod.BuffType("ArmorCrunch"), 180);
                    break;

                default:
                    break;
            }

            if (CalamityWorld.revenge)
			{
				if (Config.RevengeanceAndDeathThoriumBossBuff)
				{
					Mod thorium = ModLoader.GetMod("ThoriumMod");
					if (thorium != null)
					{
						if (npc.type == thorium.NPCType("GraniteEnergyStorm") || npc.type == thorium.NPCType("TheBuriedWarrior") || npc.type == thorium.NPCType("TheBuriedWarrior1") ||
							npc.type == thorium.NPCType("TheBuriedWarrior2") || npc.type == thorium.NPCType("ThePrimeScouter") || npc.type == thorium.NPCType("CryoCore") ||
							npc.type == thorium.NPCType("BioCore") || npc.type == thorium.NPCType("PyroCore") || npc.type == thorium.NPCType("SlagFury") ||
							npc.type == thorium.NPCType("Omnicide") || npc.type == thorium.NPCType("RealityBreaker") || npc.type == thorium.NPCType("Aquaius") ||
							npc.type == thorium.NPCType("Aquaius2"))
						{
							target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						}
						else if (npc.type == thorium.NPCType("ViscountBaby") || npc.type == thorium.NPCType("EnemyBeholder") || npc.type == thorium.NPCType("AbyssalSpawn") ||
							npc.type == thorium.NPCType("Viscount") || npc.type == thorium.NPCType("FallenDeathBeholder") || npc.type == thorium.NPCType("FallenDeathBeholder2") ||
							npc.type == thorium.NPCType("Lich") || npc.type == thorium.NPCType("LichHeadless") || npc.type == thorium.NPCType("Abyssion") ||
							npc.type == thorium.NPCType("AbyssionCracked") || npc.type == thorium.NPCType("AbyssionReleased"))
						{
							target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
							target.AddBuff(mod.BuffType("Horror"), 180);
						}
					}
				}

                switch (npc.type)
                {
                    case NPCID.DemonEye:
                    case NPCID.EaterofSouls:
                    case NPCID.EaterofWorldsTail:
                    case NPCID.ChaosBall:
                    case NPCID.VileSpit:
                    case NPCID.LeechHead:
                    case NPCID.Crimera:
                    case NPCID.FaceMonster:
                    case NPCID.BloodCrawler:
                    case NPCID.BloodCrawlerWall:
                        target.AddBuff(mod.BuffType("Horror"), 60);
                        break;

                    case NPCID.DevourerHead:
                    case NPCID.EaterofWorldsBody:
                    case NPCID.CorruptBunny:
                    case NPCID.CorruptGoldfish:
                    case NPCID.CorruptSlime:
                    case NPCID.Corruptor:
                    case NPCID.Clinger:
                    case NPCID.Slimer:
                    case NPCID.WanderingEye:
                    case NPCID.Frankenstein:
                    case NPCID.WallCreeper:
                    case NPCID.WallCreeperWall:
                    case NPCID.SwampThing:
                    case NPCID.CorruptPenguin:
                    case NPCID.Herpling:
                    case NPCID.FloatyGross:
                    case NPCID.Crimslime:
                    case NPCID.BloodFeeder:
                    case NPCID.BloodJelly:
                    case NPCID.IchorSticker:
                    case NPCID.Ghost:
                    case NPCID.CreatureFromTheDeep:
                    case NPCID.Fritz:
                    case NPCID.CrimsonBunny:
                    case NPCID.CrimsonGoldfish:
                    case NPCID.CrimsonPenguin:
                    case NPCID.Drippler:
                        target.AddBuff(mod.BuffType("Horror"), 120);
                        break;

                    case NPCID.EyeofCthulhu:
                    case NPCID.ServantofCthulhu:
                    case NPCID.SkeletronHead:
                    case NPCID.Demon:
                    case NPCID.VoodooDemon:
                    case NPCID.CursedHammer:
                    case NPCID.SeekerHead:
                    case NPCID.RedDevil:
                    case NPCID.BlackRecluse:
                    case NPCID.CrimsonAxe:
                    case NPCID.Nymph:
                    case NPCID.BlackRecluseWall:
                    case NPCID.Eyezor:
                    case NPCID.BrainofCthulhu:
                    case NPCID.HeadlessHorseman:
                    case NPCID.Splinterling:
                    case NPCID.Hellhound:
                    case NPCID.Poltergeist:
                    case NPCID.Nailhead:
                    case NPCID.DrManFly:
                    case NPCID.ThePossessed:
                    case NPCID.Mothron:
                    case NPCID.Medusa:
                    case NPCID.SandsharkCorrupt:
                    case NPCID.SandsharkCrimson:
                        target.AddBuff(mod.BuffType("Horror"), 180);
                        break;

                    case NPCID.MourningWood:
                    case NPCID.Pumpking:
                        target.AddBuff(mod.BuffType("Horror"), 240);
                        break;

                    case NPCID.WallofFlesh:
                    case NPCID.Krampus:
                        target.AddBuff(mod.BuffType("Horror"), 300);
                        break;

                    case NPCID.DungeonGuardian:
                        target.AddBuff(mod.BuffType("Horror"), 1200);
                        break;

                    case NPCID.Creeper:
                        target.AddBuff(mod.BuffType("Horror"), 60);
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
                        break;

                    case NPCID.CursedSkull:
                    case NPCID.SkeletronHand:
                    case NPCID.TheHungry:
                    case NPCID.TheHungryII:
                    case NPCID.PumpkingBlade:
                    case NPCID.BloodZombie:
                        target.AddBuff(mod.BuffType("Horror"), 120);
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
                        break;

                    case NPCID.DarkMummy:
                        target.AddBuff(mod.BuffType("Horror"), 180);
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
                        break;

                    case NPCID.EaterofWorldsHead:
                    case NPCID.GiantCursedSkull:
                    case NPCID.Butcher:
                    case NPCID.Psycho:
                        target.AddBuff(mod.BuffType("Horror"), 180);
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
                        break;

                    case NPCID.Wraith:
                    case NPCID.VampireBat:
                    case NPCID.Vampire:
                    case NPCID.Reaper:
                        target.AddBuff(mod.BuffType("Horror"), 240);
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
                        break;

                    case NPCID.AncientCultistSquidhead:
                        target.AddBuff(mod.BuffType("Horror"), 240);
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 240);
                        break;

                    case NPCID.DungeonSpirit:
                        target.AddBuff(mod.BuffType("Horror"), 300);
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
                        break;

                    case NPCID.AncientDoom:
                        target.AddBuff(mod.BuffType("Horror"), 300);
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 300);
                        break;

                    case NPCID.Snatcher:
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 60);
                        break;

                    case NPCID.BurningSphere:
                    case NPCID.ManEater:
                    case NPCID.Mummy:
                    case NPCID.EnchantedSword:
                    case NPCID.Werewolf:
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
                        break;

                    case NPCID.Spazmatism:
                    case NPCID.PrimeSaw:
                    case NPCID.PrimeVice:
                    case NPCID.Probe:
                    case NPCID.QueenBee:
                    case NPCID.GolemFistLeft:
                    case NPCID.GolemFistRight:
                    case NPCID.Spore:
                    case NPCID.BoneLee:
                    case NPCID.Flocko:
                    case NPCID.DukeFishron:
                    case NPCID.Sharkron:
                    case NPCID.Sharkron2:
                    case NPCID.ShadowFlameApparition:
                    case NPCID.MartianDrone:
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
                        break;

                    case NPCID.Mimic:
                    case NPCID.AngryTrapper:
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 240);
                        break;

                    case NPCID.DetonatingBubble:
                        target.AddBuff(mod.BuffType("MarkedforDeath"), 360);
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion

        #region Modify Hit By Projectile
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (npc.type == NPCID.TheDestroyerBody)
            {
                if (((projectile.penetrate == -1 || projectile.penetrate > 1) && !projectile.minion) || projectile.type == mod.ProjectileType("KelvinCatalystStar"))
                {
                    damage = (int)(damage * 0.5);
                }
                else if (projectile.type == mod.ProjectileType("FossilShardThrown") || projectile.type == mod.ProjectileType("FrostShardFriendly"))
                {
                    damage = (int)(damage * 0.75);
                }
            }
            else if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail || npc.type == NPCID.Creeper)
            {
                if ((projectile.penetrate == -1 || projectile.penetrate > 1) && !projectile.minion)
                {
                    damage = (int)(damage * 0.6);
                }
            }

            if (Main.player[projectile.owner].GetCalamityPlayer().eGauntlet)
            {
                if (projectile.melee && ShouldAffectNPC(npc) && Main.rand.NextBool(15))
                {
                    if (!CalamityPlayer.areThereAnyDamnBosses)
                    {
                        damage = npc.lifeMax * 3;
                    }
                }
            }

            if (Main.player[projectile.owner].GetCalamityPlayer().eTalisman)
            {
                if (projectile.magic && ShouldAffectNPC(npc) && Main.rand.NextBool(15))
                {
                    if (!CalamityPlayer.areThereAnyDamnBosses)
                    {
                        damage = npc.lifeMax * 3;
                    }
                }
            }

            if (Main.player[projectile.owner].GetCalamityPlayer().nanotech)
            {
                if (projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue && ShouldAffectNPC(npc) && Main.rand.NextBool(15))
                {
                    if (!CalamityPlayer.areThereAnyDamnBosses)
                    {
                        damage = npc.lifeMax * 3;
                    }
                }
            }

            if (Main.player[projectile.owner].GetCalamityPlayer().eQuiver)
            {
                if (projectile.ranged && ShouldAffectNPC(npc) && Main.rand.NextBool(15))
                {
                    if (!CalamityPlayer.areThereAnyDamnBosses)
                    {
                        damage = npc.lifeMax * 3;
                    }
                }
            }

            if (Main.player[projectile.owner].GetCalamityPlayer().statisBeltOfCurses)
            {
                if ((projectile.minion || CalamityMod.projectileMinionList.Contains(projectile.type)) && ShouldAffectNPC(npc) && Main.rand.NextBool(15))
                {
                    if (!CalamityPlayer.areThereAnyDamnBosses)
                    {
                        damage = npc.lifeMax * 3;
                    }
                }
            }
        }
        #endregion

        #region On Hit By Item
        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            if (player.GetCalamityPlayer().bloodflareSet)
            {
                if (!npc.SpawnedFromStatue && npc.damage > 0 && (npc.life < npc.lifeMax * 0.5) &&
                    player.GetCalamityPlayer().bloodflareHeartTimer <= 0)
                {
                    player.GetCalamityPlayer().bloodflareHeartTimer = 180;
                    DropHelper.DropItem(npc, ItemID.Heart);
                }
                else if (!npc.SpawnedFromStatue && npc.damage > 0 && (npc.life > npc.lifeMax * 0.5) &&
                    player.GetCalamityPlayer().bloodflareManaTimer <= 0)
                {
                    player.GetCalamityPlayer().bloodflareManaTimer = 180;
                    DropHelper.DropItem(npc, ItemID.Star);
                }
            }
        }
        #endregion

        #region On Hit By Projectile
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
			bool isSummon = projectile.minion || projectile.sentry || CalamityMod.projectileMinionList.Contains(projectile.type);

            if (Main.player[projectile.owner].GetCalamityPlayer().sGenerator)
            {
                if (isSummon && npc.damage > 0)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            Main.player[projectile.owner].AddBuff(mod.BuffType("SpiritGeneratorAtkBuff"), 120);
                            break;

                        case 1:
                            Main.player[projectile.owner].AddBuff(mod.BuffType("SpiritGeneratorRegenBuff"), 120);
                            break;

                        case 2:
                            Main.player[projectile.owner].AddBuff(mod.BuffType("SpiritGeneratorDefBuff"), 120);
                            break;

                        default:
                            break;
                    }
                }
            }

            if (Main.player[projectile.owner].GetCalamityPlayer().bloodflareSet)
            {
                if (!npc.SpawnedFromStatue && npc.damage > 0 && (npc.life < npc.lifeMax * 0.5) &&
                    Main.player[projectile.owner].GetCalamityPlayer().bloodflareHeartTimer <= 0)
                {
                    Main.player[projectile.owner].GetCalamityPlayer().bloodflareHeartTimer = 180;
                    DropHelper.DropItem(npc, ItemID.Heart);
                }
                else if (!npc.SpawnedFromStatue && npc.damage > 0 && (npc.life > npc.lifeMax * 0.5) &&
                    Main.player[projectile.owner].GetCalamityPlayer().bloodflareManaTimer <= 0)
                {
                    Main.player[projectile.owner].GetCalamityPlayer().bloodflareManaTimer = 180;
                    DropHelper.DropItem(npc, ItemID.Star);
                }
            }
        }
        #endregion

        #region Check Dead
        public override bool CheckDead(NPC npc)
        {
            if (npc.lifeMax > 1000 && npc.type != 288 &&
                npc.type != mod.NPCType("PhantomSpirit") &&
                npc.type != mod.NPCType("PhantomSpiritS") &&
                npc.type != mod.NPCType("PhantomSpiritM") &&
                npc.type != mod.NPCType("PhantomSpiritL") &&
                npc.value > 0f && npc.HasPlayerTarget &&
                NPC.downedMoonlord &&
                Main.player[npc.target].ZoneDungeon)
            {
                int maxValue = Main.expertMode ? 4 : 6;

                if (Main.rand.NextBool(maxValue) && Main.wallDungeon[Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].wall])
                {
                    int randomType = Main.rand.Next(4);
                    switch (randomType)
                    {
                        case 0:
                            randomType = mod.NPCType("PhantomSpirit");
                            break;

                        case 1:
                            randomType = mod.NPCType("PhantomSpiritS");
                            break;

                        case 2:
                            randomType = mod.NPCType("PhantomSpiritM");
                            break;

                        case 3:
                            randomType = mod.NPCType("PhantomSpiritL");
                            break;

                        default:
                            break;
                    }

                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, randomType, 0, 0f, 0f, 0f, 0f, 255);
                }
            }

            return true;
        }
        #endregion

        #region Hit Effect
        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            if (CalamityWorld.revenge)
            {
                switch (npc.type)
                {
                    case NPCID.MotherSlime:
                        if (npc.life <= 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int num261 = Main.rand.Next(2) + 2;
                                int num;
                                for (int num262 = 0; num262 < num261; num262 = num + 1)
                                {
                                    int num263 = NPC.NewNPC((int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), 1, 0, 0f, 0f, 0f, 0f, 255);
                                    Main.npc[num263].SetDefaults(-5, -1f);
                                    Main.npc[num263].velocity.X = npc.velocity.X * 2f;
                                    Main.npc[num263].velocity.Y = npc.velocity.Y;
                                    NPC var_324_BB1A_cp_0_cp_0 = Main.npc[num263];
                                    var_324_BB1A_cp_0_cp_0.velocity.X = var_324_BB1A_cp_0_cp_0.velocity.X + (Main.rand.Next(-20, 20) * 0.1f + num262 * npc.direction * 0.3f);
                                    NPC var_324_BB6F_cp_0_cp_0 = Main.npc[num263];
                                    var_324_BB6F_cp_0_cp_0.velocity.Y = var_324_BB6F_cp_0_cp_0.velocity.Y - (Main.rand.Next(0, 10) * 0.1f + num262);
                                    Main.npc[num263].ai[0] = -1000 * Main.rand.Next(3);

                                    if (Main.netMode == NetmodeID.Server && num263 < 200)
                                    {
                                        NetMessage.SendData(23, -1, -1, null, num263, 0f, 0f, 0f, 0, 0, 0);
                                    }

                                    num = num262;
                                }
                            }
                        }
                        break;

                    case NPCID.Demon:
                    case NPCID.VoodooDemon:
                        npc.ai[0] += 1f;
                        break;

                    case NPCID.CursedHammer:
                    case NPCID.EnchantedSword:
                    case NPCID.CrimsonAxe:
                        if (npc.life <= npc.lifeMax * 0.5)
                        {
                            npc.justHit = false;
                        }

                        break;

                    case NPCID.Clinger:
                    case NPCID.Gastropod:
                    case NPCID.GiantTortoise:
                    case NPCID.IceTortoise:
                    case NPCID.BlackRecluse:
                    case NPCID.BlackRecluseWall:
                        if (npc.life <= npc.lifeMax * 0.25)
                        {
                            npc.justHit = false;
                        }

                        break;

                    case NPCID.Paladin:
                        if (npc.life <= npc.lifeMax * 0.15)
                        {
                            npc.justHit = false;
                        }

                        break;

                    case NPCID.Clown:
                        if (Main.netMode != NetmodeID.MultiplayerClient && !Main.player[npc.target].dead)
                        {
                            npc.ai[2] += 29f;
                        }

                        break;
                }

                if (npc.type == mod.NPCType("SandTortoise") || npc.type == mod.NPCType("PlaguedTortoise"))
                {
                    if (npc.life <= npc.lifeMax * 0.25)
                    {
                        npc.justHit = false;
                    }
                }
            }
        }
        #endregion

        #region Edit Spawn Rate
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.GetCalamityPlayer().ZoneSulphur)
            {
                spawnRate = (int)(spawnRate * 1.1);
                maxSpawns = (int)(maxSpawns * 0.8f);
                if (Main.raining)
                {
                    spawnRate = (int)(spawnRate * 0.7);
                    maxSpawns = (int)(maxSpawns * 1.2f);
                }
            }
            else if (player.GetCalamityPlayer().ZoneAbyss)
            {
                spawnRate = (int)(spawnRate * 0.7);
                maxSpawns = (int)(maxSpawns * 1.1f);
            }
            else if (player.GetCalamityPlayer().ZoneCalamity)
            {
                spawnRate = (int)(spawnRate * 0.9);
                maxSpawns = (int)(maxSpawns * 1.1f);
            }
            else if (player.GetCalamityPlayer().ZoneAstral)
            {
                spawnRate = (int)(spawnRate * 0.6);
                maxSpawns = (int)(maxSpawns * 1.2f);
            }
            else if (player.GetCalamityPlayer().ZoneSunkenSea)
            {
                spawnRate = (int)(spawnRate * 0.9);
                maxSpawns = (int)(maxSpawns * 1.1f);
            }

            if (player.GetCalamityPlayer().clamity)
            {
                spawnRate = (int)(spawnRate * 0.02);
                maxSpawns = (int)(maxSpawns * 1.5f);
            }

            if (CalamityWorld.death)
            {
                spawnRate = (int)(spawnRate * 0.75);
            }
            else if (CalamityWorld.revenge)
            {
                spawnRate = (int)(spawnRate * 0.85);
            }

            if (CalamityWorld.demonMode)
            {
                spawnRate = (int)(spawnRate * 0.75);
            }

            if (player.GetCalamityPlayer().zerg && player.GetCalamityPlayer().chaosCandle)
            {
                spawnRate = (int)(spawnRate * 0.005);
                maxSpawns = (int)(maxSpawns * 7.5f);
            }
            else if (player.GetCalamityPlayer().zerg)
            {
                spawnRate = (int)(spawnRate * 0.01);
                maxSpawns = (int)(maxSpawns * 5f);
            }
            else if (player.GetCalamityPlayer().chaosCandle)
            {
                spawnRate = (int)(spawnRate * 0.02);
                maxSpawns = (int)(maxSpawns * 2.5f);
            }

            if (player.GetCalamityPlayer().zen && player.GetCalamityPlayer().tranquilityCandle)
            {
                spawnRate = (int)(spawnRate * (player.GetCalamityPlayer().ZoneAbyss ? 1.75 : 75));
                maxSpawns = (int)(maxSpawns * (player.GetCalamityPlayer().ZoneAbyss ? 0.625f : 0.005f));
            }
            else if (CalamityPlayer.areThereAnyDamnBosses || player.GetCalamityPlayer().zen || (Config.DisableExpertEnemySpawnsNearHouse && player.townNPCs > 1f && Main.expertMode))
            {
                spawnRate = (int)(spawnRate * (player.GetCalamityPlayer().ZoneAbyss ? 1.5 : 50));
                maxSpawns = (int)(maxSpawns * (player.GetCalamityPlayer().ZoneAbyss ? 0.75f : 0.01f));
            }
            else if (player.GetCalamityPlayer().tranquilityCandle)
            {
                spawnRate = (int)(spawnRate * (player.GetCalamityPlayer().ZoneAbyss ? 1.25 : 25));
                maxSpawns = (int)(maxSpawns * (player.GetCalamityPlayer().ZoneAbyss ? 0.875f : 0.02f));
            }
        }
        #endregion

        #region Edit Spawn Range
        public override void EditSpawnRange(Player player, ref int spawnRangeX, ref int spawnRangeY, ref int safeRangeX, ref int safeRangeY)
        {
            if (player.GetCalamityPlayer().ZoneAbyss)
            {
                spawnRangeX = (int)(1920 / 16 * 0.5); //0.7
                safeRangeX = (int)(1920 / 16 * 0.32); //0.52
            }
        }
        #endregion

        #region Edit Spawn Pool
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.player.GetCalamityPlayer().ZoneAbyss ||
				spawnInfo.player.GetCalamityPlayer().ZoneCalamity ||
				spawnInfo.player.GetCalamityPlayer().ZoneSulphur ||
				spawnInfo.player.GetCalamityPlayer().ZoneSunkenSea ||
				(spawnInfo.player.GetCalamityPlayer().ZoneAstral && !NPC.LunarApocalypseIsUp))
			{
				pool[0] = 0f;
			}
		}
        #endregion

        #region Drawing
        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
            {
                if (npc.type == NPCID.SkeletronPrime)
                {
                    npc.frameCounter = 0.0;
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (bBlood)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 5, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
                Lighting.AddLight(npc.position, 0.08f, 0f, 0f);
            }
            if (bFlames || enraged)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, mod.DustType("BrimstoneFlame"), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
                Lighting.AddLight(npc.position, 0.05f, 0.01f, 0.01f);
            }
            if (aFlames)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, mod.DustType("BrimstoneFlame"), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.25f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.35f;
                    }
                }
                Lighting.AddLight(npc.position, 0.025f, 0f, 0f);
            }
            if (pShred)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 5, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.1f;
                    Main.dust[dust].velocity.Y += 0.25f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }
            if (hFlames)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, mod.DustType("HolyFlame"), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
                Lighting.AddLight(npc.position, 0.25f, 0.25f, 0.1f);
            }
            if (pFlames)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 89, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
                Lighting.AddLight(npc.position, 0.07f, 0.15f, 0.01f);
            }
            if (gsInferno)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 173, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
                Lighting.AddLight(npc.position, 0.1f, 0f, 0.135f);
            }
            if (astralInfection)
            {
                if (Main.rand.Next(5) < 3)
                {
                    int dustType = (Main.rand.NextBool(2) ? mod.DustType("AstralOrange") : mod.DustType("AstralBlue"));
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, dustType, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 0.6f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                    Main.dust[dust].color = new Color(255, 255, 255, 0);
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }
            if (nightwither)
            {
                Rectangle hitbox = npc.Hitbox;
                if (Main.rand.Next(5) < 4)
                {
                    int num3 = Utils.SelectRandom(Main.rand, new int[]
                    {
                        173,
                        27,
                        234
                    });
                    int num4 = Dust.NewDust(hitbox.TopLeft(), npc.width, npc.height, num3, 0f, -2.5f, 0, default, 1f);
                    Main.dust[num4].noGravity = true;
                    Main.dust[num4].alpha = 200;
                    Main.dust[num4].velocity.Y -= 0.2f;
                    Dust dust = Main.dust[num4];
                    dust.velocity *= 1.2f;
                    dust = Main.dust[num4];
                    dust.scale += Main.rand.NextFloat();
                }
            }
            if (tSad || cDepth)
            {
                if (Main.rand.Next(6) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 33, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y += 0.15f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }
            if (dFlames)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 173, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
                Lighting.AddLight(npc.position, 0.1f, 0f, 0.135f);
            }

            if (gState || eFreeze)
            {
                drawColor = Color.Cyan;
            }

            if (marked)
            {
                drawColor = Color.Fuchsia;
            }

            if (pearlAura)
            {
                drawColor = Color.White;
            }

            if (timeSlow)
            {
                drawColor = Color.Aquamarine;
            }
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (Main.LocalPlayer.GetCalamityPlayer().trippy)
            {
                return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, npc.alpha);
            }

            if (enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
            {
                return new Color(200, 50, 50, npc.alpha);
            }

            return null;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
            {
                if (npc.type == NPCID.SkeletronPrime)
                {
                    return false;
                }
            }

            if (Main.LocalPlayer.GetCalamityPlayer().trippy)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (npc.spriteDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }

                float num66 = 0f;
                Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
                Microsoft.Xna.Framework.Color color9 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
                Microsoft.Xna.Framework.Color alpha15 = npc.GetAlpha(color9);
                float num212 = 0.99f;
                alpha15.R = (byte)(alpha15.R * num212);
                alpha15.G = (byte)(alpha15.G * num212);
                alpha15.B = (byte)(alpha15.B * num212);
                alpha15.A = (byte)(alpha15.A * num212);
                for (int num213 = 0; num213 < 4; num213++)
                {
                    Vector2 position9 = npc.position;
                    float num214 = Math.Abs(npc.Center.X - Main.LocalPlayer.Center.X);
                    float num215 = Math.Abs(npc.Center.Y - Main.LocalPlayer.Center.Y);

                    if (num213 == 0 || num213 == 2)
                    {
                        position9.X = Main.LocalPlayer.Center.X + num214;
                    }
                    else
                    {
                        position9.X = Main.LocalPlayer.Center.X - num214;
                    }

                    position9.X -= npc.width / 2;

                    if (num213 == 0 || num213 == 1)
                    {
                        position9.Y = Main.LocalPlayer.Center.Y + num215;
                    }
                    else
                    {
                        position9.Y = Main.LocalPlayer.Center.Y - num215;
                    }

                    position9.Y -= npc.height / 2;

                    Main.spriteBatch.Draw(Main.npcTexture[npc.type], new Vector2(position9.X - Main.screenPosition.X + npc.width / 2 - Main.npcTexture[npc.type].Width * npc.scale / 2f + vector11.X * npc.scale, position9.Y - Main.screenPosition.Y + npc.height - Main.npcTexture[npc.type].Height * npc.scale / Main.npcFrameCount[npc.type] + 4f + vector11.Y * npc.scale + num66 + npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(npc.frame), alpha15, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }
            return true;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
            {
                // His afterimages I can't get to work, so fuck it
                if (npc.type == NPCID.SkeletronPrime)
                {
                    Texture2D texture2D3 = Main.npcTexture[npc.type];
                    int frameHeight = texture2D3.Height / Main.npcFrameCount[npc.type];

                    npc.frame.Y = (int)newAI[3];

                    // Floating phase
                    if (npc.ai[1] == 0f || npc.ai[1] == 4f)
                    {
                        newAI[2] += 1f;
                        if (newAI[2] >= 12f)
                        {
                            newAI[2] = 0f;
                            newAI[3] = newAI[3] + frameHeight;

                            if (newAI[3] / frameHeight >= 2f)
                            {
                                newAI[3] = 0f;
                            }
                        }
                    }

                    // Spinning probe spawn or fly over phase
                    else if (npc.ai[1] == 5f || npc.ai[1] == 6f)
                    {
                        newAI[2] = 0f;
                        newAI[3] = frameHeight;
                    }

                    // Spinning phase
                    else
                    {
                        newAI[2] = 0f;
                        newAI[3] = frameHeight * 2;
                    }

                    npc.frame.Y = (int)newAI[3];

                    SpriteEffects spriteEffects = SpriteEffects.None;
                    if (npc.spriteDirection == 1)
                    {
                        spriteEffects = SpriteEffects.FlipHorizontally;
                    }

                    spriteBatch.Draw(texture2D3, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, npc.GetAlpha(drawColor), npc.rotation, npc.frame.Size() / 2, npc.scale, spriteEffects, 0);

                    spriteBatch.Draw(Main.BoneEyesTexture, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY),
                        npc.frame, new Microsoft.Xna.Framework.Color(200, 200, 200, 0), npc.rotation, npc.frame.Size() / 2, npc.scale, spriteEffects, 0);
                }
            }
        }
        #endregion

        #region Get Chat
        public override void GetChat(NPC npc, ref string chat)
        {
            int fapsol = NPC.FindFirstNPC(mod.NPCType("FAP"));
            int permadong = NPC.FindFirstNPC(mod.NPCType("DILF"));

            switch (npc.type)
            {
                case NPCID.Guide:
                    if (Main.hardMode)
                    {
                        if (Main.rand.NextBool(20))
                        {
                            chat = "Could you be so kind as to, ah...check hell for me...? I left someone I kind of care about down there.";
                        }

                        if (Main.rand.NextBool(20))
                        {
                            chat = "I have this sudden shiver up my spine, like a meteor just fell and thousands of innocent creatures turned into monsters from the stars.";
                        }
                    }

                    if (Main.rand.NextBool(20) && NPC.downedMoonlord)
                    {
                        chat = "The dungeon seems even more restless than usual, watch out for the powerful abominations stirring up in there.";
                    }

                    if (CalamityWorld.downedProvidence)
                    {
                        if (Main.rand.NextBool(20))
                        {
                            chat = "Seems like extinguishing that butterfly caused its life to seep into the hallowed areas, try taking a peek there and see what you can find!";
                        }

                        if (Main.rand.NextBool(20))
                        {
                            chat = "I've heard there is a portal of antimatter absorbing everything it can see in the dungeon, try using the Rune of Kos there!";
                        }
                    }

                    break;

                case NPCID.PartyGirl:
                    if (Main.rand.NextBool(10) && fapsol != -1)
                    {
                        chat = "I have a feeling we're going to have absolutely fantastic parties with " + Main.npc[fapsol].GivenName + " around!";
                    }

                    if (Main.eclipse)
                    {
                        if (Main.rand.NextBool(5))
                        {
                            chat = "I think my light display is turning into an accidental bug zapper. At least the monsters are enjoying it.";
                        }

                        if (Main.rand.NextBool(5))
                        {
                            chat = "Ooh! I love parties where everyone wears a scary costume!";
                        }
                    }

                    break;

                case NPCID.Wizard:
                    if (Main.rand.NextBool(10) && permadong != -1)
                    {
                        chat = "I'd let " + Main.npc[permadong].GivenName + " coldheart MY icicle.";
                    }

                    if (Main.rand.NextBool(10) && Main.hardMode)
                    {
                        chat = "Space just got way too close for comfort.";
                    }

                    break;

                case NPCID.Dryad:
                    if (Main.rand.NextBool(5) && CalamityWorld.buffedEclipse && Main.eclipse)
                    {
                        chat = "There's a dark solar energy emanating from the moths that appear during this time. Ah, the moths as you progress further get more powerful...hmm...what power was Yharon holding back?";
                    }

                    if (Main.rand.NextBool(10) && Main.hardMode)
                    {
                        chat = "That starborne illness sits upon this land like a blister. Do even more vile forces of corruption exist in worlds beyond?";
                    }

                    break;

                case NPCID.Stylist:
                    string worldEvil = WorldGen.crimson ? "Crimson" : "Corruption";
                    if (Main.rand.NextBool(15) && Main.hardMode)
                    {
                        chat = "Please don't catch space lice. Or " + worldEvil + " lice. Or just lice in general.";
                    }

                    if (fapsol != -1)
                    {
                        if (Main.rand.NextBool(15))
                        {
                            chat = "Sometimes I catch " + Main.npc[fapsol].GivenName + " sneaking up from behind me.";
                        }

                        if (Main.rand.NextBool(15))
                        {
                            chat = Main.npc[fapsol].GivenName + " is always trying to brighten my mood...even if, deep down, I know she's sad.";
                        }
                    }

                    break;

                case NPCID.GoblinTinkerer:
                    int banditIndex = NPC.FindFirstNPC(mod.NPCType("Bandit"));
                    if (Main.rand.NextBool(10) && banditIndex != -1 && Main.LocalPlayer.GetModPlayer<CalamityPlayer>().reforges >= 10)
                    {
                        var thief = Main.npc[banditIndex];
                        chat = $"Hey, is it just me or have my pockets gotten lighter ever since {thief.GivenName} arrived?";
                    }
                    if (Main.rand.NextBool(10) && NPC.downedMoonlord)
                    {
                        chat = "You know...we haven't had an invasion in a while...";
                    }

                    break;

                case NPCID.ArmsDealer:
                    if (Main.rand.NextBool(5) && Main.eclipse)
                    {
                        chat = "That's the biggest moth I've ever seen for sure. You'd need one big gun to take one of those down.";
                    }

                    if (Main.rand.NextBool(10) && CalamityWorld.downedDoG)
                    {
                        chat = "Is it me or are your weapons getting bigger and bigger?";
                    }

                    break;

                case NPCID.Merchant:
                    if (Main.rand.NextBool(5) && NPC.downedMoonlord)
                    {
                        chat = "Each night seems only more foreboding than the last. I feel unthinkable terrors are watching your every move.";
                    }

                    if (Main.rand.NextBool(5) && Main.eclipse)
                    {
                        chat = "Are you daft?! Turn off those lamps!";
                    }

                    if (Main.rand.NextBool(5) && Main.raining && CalamityWorld.sulphurTiles > 30)
                    {
                        chat = "If this acid rain keeps up, there'll be a shortage of Dirt Blocks soon enough!";
                    }

                    break;

                case NPCID.Mechanic:
                    if (Main.rand.NextBool(5) && NPC.downedMoonlord)
                    {
                        chat = "What do you mean your traps aren't making the cut? Don't look at me!";
                    }

                    if (Main.rand.NextBool(5) && Main.eclipse)
                    {
                        chat = "Um...should my nightlight be on?";
                    }

                    if (Main.rand.NextBool(5) && fapsol != -1)
                    {
                        chat = "Well, I like " + Main.npc[fapsol].GivenName + ", but I, ah...I have my eyes on someone else.";
                    }

                    break;

                case NPCID.DD2Bartender:
                    if (Main.rand.NextBool(5) && !Main.dayTime && Main.moonPhase == 0)
                    {
                        chat = "Care for a little Moonshine?";
                    }

                    if (Main.rand.NextBool(10) && fapsol != -1)
                    {
                        chat = "Sheesh, " + Main.npc[fapsol].GivenName + " is a little cruel, isn't she? I never claimed to be an expert on anything but ale!";
                    }

                    break;

                case NPCID.Pirate:
                    if (Main.rand.NextBool(5) && !CalamityWorld.downedLeviathan)
                    {
                        chat = "Aye, I’ve heard of a mythical creature in the oceans, singing with an alluring voice. Careful when yer fishin out there.";
                    }

                    if (Main.rand.NextBool(5) && CalamityWorld.downedAquaticScourge)
                    {
                        chat = "I have to thank ye again for takin' care of that sea serpent. Or was that another one...";
                    }

                    break;

                case NPCID.Cyborg:
                    if (Main.rand.NextBool(10) && Main.raining)
                    {
                        chat = "All these moments will be lost in time. Like tears...in the rain.";
                    }

                    if (NPC.downedMoonlord)
                    {
                        if (Main.rand.NextBool(10))
                        {
                            chat = "Always shoot for the moon! It has clearly worked before.";
                        }

                        if (Main.rand.NextBool(10))
                        {
                            chat = "Draedon? He's...a little 'high octane' if you know what I mean.";
                        }
                    }

                    if (Main.rand.NextBool(10) && !CalamityWorld.downedPlaguebringer && NPC.downedGolemBoss)
                    {
                        chat = "Those oversized bugs terrorizing the jungle... Surely you of all people could shut them down!";
                    }

                    break;

                case NPCID.Clothier:
                    if (NPC.downedMoonlord)
                    {
                        if (Main.rand.NextBool(10))
                        {
                            chat = "Who you gonna call?";
                        }

                        if (Main.rand.NextBool(10))
                        {
                            chat = "Those screams...I’m not sure why, but I feel like a nameless fear has awoken in my heart.";
                        }

                        if (Main.rand.NextBool(10))
                        {
                            chat = "I can faintly hear ghostly shrieks from the dungeon...and not ones I’m familiar with at all. Just what is going on in there?";
                        }
                    }

                    if (Main.rand.NextBool(10) && CalamityWorld.downedPolterghast)
                    {
                        chat = "Whatever that thing was, I’m glad it’s gone now.";
                    }

                    if (Main.rand.NextBool(5) && NPC.AnyNPCs(NPCID.MoonLordCore))
                    {
                        chat = "Houston, we've had a problem.";
                    }

                    break;

                case NPCID.Steampunker:
                    bool hasPortalGun = false;
                    for (int k = 0; k < 255; k++)
                    {
                        Player player = Main.player[k];
                        if (player.active && player.HasItem(ItemID.PortalGun))
                        {
                            hasPortalGun = true;
                        }
                    }

                    if (Main.rand.NextBool(5) && hasPortalGun)
                    {
                        chat = "Just what is that contraption? It makes my Teleporters look like child's play!";
                    }

                    if (Main.rand.NextBool(5) && NPC.downedMoonlord)
                    {
                        chat = "Yep! I'm also considering being a space pirate now.";
                    }

                    break;

                case NPCID.DyeTrader:
                    if (Main.rand.NextBool(5))
                    {
                        chat = "Have you seen those gemstone creatures in the caverns? Their colors are simply breathtaking!";
                    }

                    if (Main.rand.NextBool(5) && permadong != -1)
                    {
                        chat = "Do you think " + Main.npc[permadong].GivenName + " knows how to 'let it go?'";
                    }

                    break;

                case NPCID.TaxCollector:
                    int platinumCoins = 0;
                    for (int k = 0; k < 255; k++)
                    {
                        Player player = Main.player[k];
                        if (player.active)
                        {
                            for (int j = 0; j < player.inventory.Length; j++)
                            {
                                if (player.inventory[j].type == ItemID.PlatinumCoin)
                                {
                                    platinumCoins += player.inventory[j].stack;
                                }
                            }
                        }
                    }

                    if (Main.rand.NextBool(5) && platinumCoins >= 100)
                    {
                        chat = "BAH! Doesn't seem like I'll ever be able to quarrel with the debts of the town again!";
                    }

                    if (Main.rand.NextBool(5) && platinumCoins >= 500)
                    {
                        chat = "Where and how are you getting all of this money?";
                    }

                    if (Main.rand.NextBool(5) && !CalamityWorld.downedBrimstoneElemental)
                    {
                        chat = "Perhaps with all that time you've got you could check those old ruins? Certainly something of value in it for you!";
                    }

                    if (Main.rand.NextBool(10) && CalamityWorld.downedDoG)
                    {
                        chat = "Devourer of what, you said? Devourer of Funds, if its payroll is anything to go by!";
                    }

                    break;

                case NPCID.Demolitionist:
                    if (Main.rand.NextBool(5) && CalamityWorld.downedDoG)
                    {
                        chat = "God Slayer Dynamite? Boy do I like the sound of that!";
                    }

                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Shop Stuff
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.Merchant)
            {
                SetShopItem(ref shop, ref nextSlot, ItemID.Flare, Main.LocalPlayer.HasItem(mod.ItemType("FirestormCannon")) || Main.LocalPlayer.HasItem(mod.ItemType("SpectralstormCannon")));
                SetShopItem(ref shop, ref nextSlot, ItemID.ApprenticeBait, NPC.downedBoss1);
                SetShopItem(ref shop, ref nextSlot, ItemID.JourneymanBait, NPC.downedBoss2);
                SetShopItem(ref shop, ref nextSlot, WorldGen.crimson ? ItemID.Vilethorn : ItemID.TheRottedFork, NPC.downedBoss2);
                SetShopItem(ref shop, ref nextSlot, ItemID.MasterBait, NPC.downedBoss3);
            }

            if (type == NPCID.ArmsDealer)
            {
                SetShopItem(ref shop, ref nextSlot, ItemID.Stake, Main.LocalPlayer.HasItem(mod.ItemType("Impaler")));
                SetShopItem(ref shop, ref nextSlot, WorldGen.crimson ? ItemID.Musket : ItemID.TheUndertaker, NPC.downedBoss2);
                SetShopItem(ref shop, ref nextSlot, ItemID.Boomstick, NPC.downedQueenBee, price: Item.buyPrice(0, 20, 0, 0));
            }

            if (type == NPCID.Dryad)
            {
                SetShopItem(ref shop, ref nextSlot, ItemID.JungleRose, price: Item.buyPrice(0, 2));
                SetShopItem(ref shop, ref nextSlot, ItemID.NaturesGift, price: Item.buyPrice(0, 10));
                SetShopItem(ref shop, ref nextSlot, ItemID.GoblinBattleStandard, price: Item.buyPrice(0, 1));
                SetShopItem(ref shop, ref nextSlot, ItemID.SlimeCrown, NPC.downedSlimeKing, Item.buyPrice(0, 2));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("DriedSeafood"), CalamityWorld.downedDesertScourge, Item.buyPrice(0, 2));
                SetShopItem(ref shop, ref nextSlot, ItemID.SuspiciousLookingEye, NPC.downedBoss1, Item.buyPrice(0, 3));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("DecapoditaSprout"), CalamityWorld.downedCrabulon, Item.buyPrice(0, 4));
                SetShopItem(ref shop, ref nextSlot, ItemID.BloodySpine, NPC.downedBoss2, Item.buyPrice(0, 6));
                SetShopItem(ref shop, ref nextSlot, ItemID.WormFood, NPC.downedBoss2, Item.buyPrice(0, 6));
                SetShopItem(ref shop, ref nextSlot, WorldGen.crimson ? ItemID.BandofStarpower : ItemID.PanicNecklace, NPC.downedBoss2);
                SetShopItem(ref shop, ref nextSlot, WorldGen.crimson ? ItemID.WormScarf : ItemID.BrainOfConfusion, Main.expertMode && NPC.downedBoss2);
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("BloodyWormFood"), CalamityWorld.downedPerforator, Item.buyPrice(0, 10));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("RottenBrain"), CalamityWorld.downedPerforator && Main.expertMode);
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("Teratoma"), CalamityWorld.downedHiveMind, Item.buyPrice(0, 10));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("BloodyWormTooth"), CalamityWorld.downedHiveMind && Main.expertMode);
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("OverloadedSludge"), CalamityWorld.downedSlimeGod, Item.buyPrice(0, 15));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("Seafood"), CalamityWorld.downedAquaticScourge, Item.buyPrice(0, 20));
                SetShopItem(ref shop, ref nextSlot, ItemID.PumpkinMoonMedallion, NPC.downedHalloweenKing, Item.buyPrice(0, 25));
                SetShopItem(ref shop, ref nextSlot, ItemID.NaughtyPresent, NPC.downedChristmasIceQueen, Item.buyPrice(0, 25));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("RomajedaOrchid"));
            }

            if (type == NPCID.GoblinTinkerer)
            {
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("MeleeLevelMeter"), price: Item.buyPrice(0, 5));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("RangedLevelMeter"), price: Item.buyPrice(0, 5));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("MagicLevelMeter"), price: Item.buyPrice(0, 5));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("SummonLevelMeter"), price: Item.buyPrice(0, 5));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("RogueLevelMeter"), price: Item.buyPrice(0, 5));
            }

            if (type == NPCID.Clothier)
            {
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("BlueBrickWallUnsafe"), price: Item.buyPrice(copper: 10));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("BlueSlabWallUnsafe"), price: Item.buyPrice(copper: 10));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("BlueTiledWallUnsafe"), price: Item.buyPrice(copper: 10));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("GreenBrickWallUnsafe"), price: Item.buyPrice(copper: 10));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("GreenSlabWallUnsafe"), price: Item.buyPrice(copper: 10));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("GreenTiledWallUnsafe"), price: Item.buyPrice(copper: 10));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("PinkBrickWallUnsafe"), price: Item.buyPrice(copper: 10));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("PinkSlabWallUnsafe"), price: Item.buyPrice(copper: 10));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("PinkTiledWallUnsafe"), price: Item.buyPrice(copper: 10));
                SetShopItem(ref shop, ref nextSlot, ItemID.GoldenKey, Main.hardMode, Item.buyPrice(0, 5));
            }

            if (type == NPCID.Painter)
            {
                SetShopItem(ref shop, ref nextSlot, ItemID.PainterPaintballGun, price: Item.buyPrice(0, 15));
            }

            if (type == NPCID.Steampunker)
            {
                SetShopItem(ref shop, ref nextSlot, ItemID.MechanicalWorm, NPC.downedMechBoss1, Item.buyPrice(0, 20));
                SetShopItem(ref shop, ref nextSlot, ItemID.MechanicalEye, NPC.downedMechBoss2, Item.buyPrice(0, 20));
                SetShopItem(ref shop, ref nextSlot, ItemID.MechanicalSkull, NPC.downedMechBoss3, Item.buyPrice(0, 20));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("AstralSolution"), Main.LocalPlayer.GetCalamityPlayer().ZoneAstral && Main.hardMode, Item.buyPrice(0, 0, 5));
            }

            if (type == NPCID.Wizard)
            {
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("CryoKey"), CalamityWorld.downedCryogen, Item.buyPrice(0, 15));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("CharredIdol"), CalamityWorld.downedBrimstoneElemental, Item.buyPrice(0, 20));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("AstralChunk"), CalamityWorld.downedAstrageldon, Item.buyPrice(0, 25));
                SetShopItem(ref shop, ref nextSlot, ItemID.MagicMissile, price: Item.buyPrice(0, 5));
                SetShopItem(ref shop, ref nextSlot, ItemID.SpectreStaff, NPC.downedGolemBoss, Item.buyPrice(0, 25));
                SetShopItem(ref shop, ref nextSlot, ItemID.InfernoFork, NPC.downedGolemBoss, Item.buyPrice(0, 25));
                SetShopItem(ref shop, ref nextSlot, ItemID.ShadowbeamStaff, NPC.downedGolemBoss, Item.buyPrice(0, 25));
                SetShopItem(ref shop, ref nextSlot, ItemID.CelestialSigil, NPC.downedMoonlord, Item.buyPrice(3));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("ProfanedShard"), CalamityWorld.downedGuardians, Item.buyPrice(10));
            }

            if (type == NPCID.WitchDoctor)
            {
                SetShopItem(ref shop, ref nextSlot, ItemID.Abeemination, price: Item.buyPrice(0, 8));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("BulbofDoom"), NPC.downedPlantBoss, Item.buyPrice(0, 20));
                SetShopItem(ref shop, ref nextSlot, ItemID.SolarTablet, NPC.downedGolemBoss, Item.buyPrice(0, 25));
                SetShopItem(ref shop, ref nextSlot, ItemID.LihzahrdPowerCell, NPC.downedGolemBoss, Item.buyPrice(0, 30));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("AncientMedallion"), CalamityWorld.downedScavenger, Item.buyPrice(0, 50));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("Abomination"), CalamityWorld.downedPlaguebringer, Item.buyPrice(0, 50));
                SetShopItem(ref shop, ref nextSlot, mod.ItemType("BirbPheromones"), CalamityWorld.downedBumble, Item.buyPrice(5));
            }
        }

        public override void SetupTravelShop(int[] shop, ref int nextSlot)
        {
            if (Main.moonPhase == 0)
            {
                shop[nextSlot] = mod.ItemType("FrostBarrier");
                nextSlot++;
            }
        }

        public void SetShopItem(ref Chest shop, ref int nextSlot, int itemID, bool condition = true, int? price = null)
        {
			if (condition)
			{
				shop.item[nextSlot].SetDefaults(itemID);
				if (price != null)
				{
					shop.item[nextSlot].shopCustomPrice = price;
				}

				nextSlot++;
			}
        }
        #endregion

        #region Any Boss NPCs
        public static bool AnyBossNPCS()
		{
			Mod mod = ModLoader.GetMod("CalamityMod");

			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type != NPCID.MartianSaucerCore &&
					(Main.npc[i].boss || Main.npc[i].type == NPCID.EaterofWorldsHead || Main.npc[i].type == NPCID.EaterofWorldsTail || Main.npc[i].type == mod.NPCType("SlimeGodRun") ||
					Main.npc[i].type == mod.NPCType("SlimeGodRunSplit") || Main.npc[i].type == mod.NPCType("SlimeGod") || Main.npc[i].type == mod.NPCType("SlimeGodSplit")))
				{
					return true;
				}
			}
			return false;
		}
        #endregion

        #region Any Living Players
        public static bool AnyLivingPlayers()
        {
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Should Affect NPC
        public static bool ShouldAffectNPC(NPC target)
		{
			Mod mod = ModLoader.GetMod("CalamityMod");

			if (target.damage > 0 && !target.boss && !target.friendly && !target.dontTakeDamage && target.type != NPCID.Mothron &&
				target.type != NPCID.Pumpking && target.type != NPCID.TheDestroyerBody && target.type != NPCID.TheDestroyerTail &&
				target.type != NPCID.MourningWood && target.type != NPCID.Everscream && target.type != NPCID.SantaNK1 && target.type != NPCID.IceQueen &&
				target.type != mod.NPCType("Reaper") && target.type != mod.NPCType("Mauler") && target.type != mod.NPCType("EidolonWyrmHead") &&
				target.type != mod.NPCType("EidolonWyrmHeadHuge") && target.type != mod.NPCType("ColossalSquid") && target.type != NPCID.DD2Betsy)
			{
				return true;
			}
			return false;
		}
        #endregion

        #region Old Duke Spawn
        public static void OldDukeSpawn(int plr, int Type)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            Player player = Main.player[plr];

            if (!player.active || player.dead)
            {
                return;
            }

            int m = 0;
            while (m < 1000)
            {
                Projectile projectile = Main.projectile[m];
                if (projectile.active && projectile.bobber && projectile.owner == plr)
                {
                    int num8 = NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y + 100, mod.NPCType("OldDuke"));
                    string typeName2 = Main.npc[num8].TypeName;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue("Announcement.HasAwoken", typeName2), new Color(175, 75, 255));
                        return;
                    }

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", new object[]
                        {
                            Main.npc[num8].GetTypeNetName()
                        }), new Color(175, 75, 255));
                        return;
                    }

                    break;
                }
                else
                {
                    m++;
                }
            }
        }
        #endregion

        #region Astral things
        public static void DoHitDust(NPC npc, int hitDirection, int dustType = 5, float xSpeedMult = 1f, int numHitDust = 5, int numDeathDust = 20)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection * xSpeedMult, -1f);
            }

            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection * xSpeedMult, -1f);
                }
            }
        }

        public static void DoFlyingAI(NPC npc, float maxSpeed, float acceleration, float circleTime, float minDistanceTarget = 150f, bool shouldAttackTarget = true)
        {
            //Pick a new target.
            if (npc.target < 0 || npc.target >= 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }

            Player myTarget = Main.player[npc.target];
            Vector2 toTarget = (myTarget.Center - npc.Center);
            float distanceToTarget = toTarget.Length();
            Vector2 maxVelocity = toTarget;

            if (distanceToTarget < 3f)
            {
                maxVelocity = npc.velocity;
            }
            else
            {
                float magnitude = maxSpeed / distanceToTarget;
                maxVelocity *= magnitude;
            }

            //Circular motion
            npc.ai[0]++;

            //y motion
            if (npc.ai[0] > circleTime * 0.5f)
            {
                npc.velocity.Y += acceleration;
            }
            else
            {
                npc.velocity.Y -= acceleration;
            }

            //x motion
            if (npc.ai[0] < circleTime * 0.25f || npc.ai[0] > circleTime * 0.75f)
            {
                npc.velocity.X += acceleration;
            }
            else
            {
                npc.velocity.X -= acceleration;
            }

            //reset
            if (npc.ai[0] > circleTime)
            {
                npc.ai[0] = 0f;
            }

            //if close enough
            if (shouldAttackTarget && distanceToTarget < minDistanceTarget)
            {
                npc.velocity += maxVelocity * 0.007f;
            }

            if (myTarget.dead)
            {
                maxVelocity.X = npc.direction * maxSpeed / 2f;
                maxVelocity.Y = -maxSpeed / 2f;
            }

            //maximise velocity
            if (npc.velocity.X < maxVelocity.X)
            {
                npc.velocity.X += acceleration;
            }

            if (npc.velocity.X > maxVelocity.X)
            {
                npc.velocity.X -= acceleration;
            }

            if (npc.velocity.Y < maxVelocity.Y)
            {
                npc.velocity.Y += acceleration;
            }

            if (npc.velocity.Y > maxVelocity.Y)
            {
                npc.velocity.Y -= acceleration;
            }

            //rotate towards player if alive
            if (!myTarget.dead)
            {
                npc.rotation = toTarget.ToRotation();
            }
            else //don't, do velocity instead
            {
                npc.rotation = npc.velocity.ToRotation();
            }

            npc.rotation += MathHelper.Pi;

            //tile collision
            float collisionDamp = 0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -collisionDamp;

                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }

                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -collisionDamp;

                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5f)
                {
                    npc.velocity.Y = 1.5f;
                }

                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5f)
                {
                    npc.velocity.Y = -1.5f;
                }
            }

            //water collision
            if (npc.wet)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y *= 0.95f;
                }

                npc.velocity.Y -= 0.3f;

                if (npc.velocity.Y < -2f)
                {
                    npc.velocity.Y = -2f;
                }
            }

            //Taken from source. Important for net?
            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }
        }

        public static void DoSpiderWallAI(NPC npc, int transformType, float chaseMaxSpeed = 2f, float chaseAcceleration = 0.08f)
        {
            //GET NEW TARGET
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest();
            }

            Vector2 between = Main.player[npc.target].Center - npc.Center;
            float distance = between.Length();

            //modify vector depending on distance and speed.
            if (distance == 0f)
            {
                between.X = npc.velocity.X;
                between.Y = npc.velocity.Y;
            }
            else
            {
                distance = chaseMaxSpeed / distance;
                between.X *= distance;
                between.Y *= distance;
            }

            //update if target dead.
            if (Main.player[npc.target].dead)
            {
                between.X = npc.direction * chaseMaxSpeed / 2f;
                between.Y = -chaseMaxSpeed / 2f;
            }
            npc.spriteDirection = -1;

            //If spider can't see target, circle around to attempt to find the target.
            if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
            {
                //CIRCULAR MOTION, SIMILAR TO FLYING AI (Eater of Souls etc.)
                npc.ai[0]++;

                if (npc.ai[0] > 0f)
                {
                    npc.velocity.Y += 0.023f;
                }
                else
                {
                    npc.velocity.Y -= 0.023f;
                }

                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                {
                    npc.velocity.X += 0.023f;
                }
                else
                {
                    npc.velocity.X -= 0.023f;
                }

                if (npc.ai[0] > 200f)
                {
                    npc.ai[0] = -200f;
                }

                npc.velocity.X += between.X * 0.007f;
                npc.velocity.Y += between.Y * 0.007f;
                npc.rotation = npc.velocity.ToRotation();

                if (npc.velocity.X > 1.5f)
                {
                    npc.velocity.X *= 0.9f;
                }

                if (npc.velocity.X < -1.5f)
                {
                    npc.velocity.X *= 0.9f;
                }

                if (npc.velocity.Y > 1.5f)
                {
                    npc.velocity.Y *= 0.9f;
                }

                if (npc.velocity.Y < -1.5f)
                {
                    npc.velocity.Y *= 0.9f;
                }

                npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -3f, 3f);
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -3f, 3f);
            }
            else //CHASE TARGET
            {
                if (npc.velocity.X < between.X)
                {
                    npc.velocity.X = npc.velocity.X + chaseAcceleration;

                    if (npc.velocity.X < 0f && between.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X + chaseAcceleration;
                    }
                }
                else if (npc.velocity.X > between.X)
                {
                    npc.velocity.X = npc.velocity.X - chaseAcceleration;

                    if (npc.velocity.X > 0f && between.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - chaseAcceleration;
                    }
                }
                if (npc.velocity.Y < between.Y)
                {
                    npc.velocity.Y = npc.velocity.Y + chaseAcceleration;

                    if (npc.velocity.Y < 0f && between.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + chaseAcceleration;
                    }
                }
                else if (npc.velocity.Y > between.Y)
                {
                    npc.velocity.Y = npc.velocity.Y - chaseAcceleration;

                    if (npc.velocity.Y > 0f && between.Y < 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y - chaseAcceleration;
                    }
                }
                npc.rotation = between.ToRotation();
            }

            //DAMP COLLISIONS OFF OF WALLS
            float collisionDamp = 0.5f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -collisionDamp;

                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }

                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -collisionDamp;

                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5f)
                {
                    npc.velocity.Y = 2f;
                }

                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5f)
                {
                    npc.velocity.Y = -2f;
                }
            }

            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int x = (int)npc.Center.X / 16;
                int y = (int)npc.Center.Y / 16;
                bool flag = false;

                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (Main.tile[i, j] == null)
                        {
                            return;
                        }

                        if (Main.tile[i, j].wall > 0)
                        {
                            flag = true;
                        }
                    }
                }
                if (!flag)
                {
                    npc.Transform(transformType);
                    return;
                }
            }
        }

        public static void DoVultureAI(NPC npc, float acceleration = 0.1f, float maxSpeed = 3f, int sitWidth = 30, int flyWidth = 50, int rangeX = 100, int rangeY = 100)
        {
            npc.localAI[0]++;
            npc.noGravity = true;
            npc.TargetClosest(true);

            if (npc.ai[0] == 0f)
            {
                npc.width = sitWidth;
                npc.noGravity = false;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.velocity.X != 0f || npc.velocity.Y < 0f || npc.velocity.Y > 0.3)
                    {
                        npc.ai[0] = 1f;
                        npc.netUpdate = true;
                    }
                    else
                    {
                        Rectangle playerRect = Main.player[npc.target].getRect();
                        Rectangle rangeRect = new Rectangle((int)npc.Center.X - rangeX, (int)npc.Center.Y - rangeY, rangeX * 2, rangeY * 2);
                        if (npc.localAI[0] > 20f && (rangeRect.Intersects(playerRect) || npc.life < npc.lifeMax))
                        {
                            npc.ai[0] = 1f;
                            npc.velocity.Y -= 6f;
                            npc.netUpdate = true;
                        }
                    }
                }
            }
            else if (!Main.player[npc.target].dead)
            {
                npc.width = flyWidth;

                //Collision damping
                if (npc.collideX)
                {
                    npc.velocity.X = npc.oldVelocity.X * -0.5f;

                    if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    {
                        npc.velocity.X = 2f;
                    }

                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }

                if (npc.collideY)
                {
                    npc.velocity.Y = npc.oldVelocity.Y * -0.5f;

                    if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                    {
                        npc.velocity.Y = 1f;
                    }

                    if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                    {
                        npc.velocity.Y = -1f;
                    }
                }

                if (npc.direction == -1 && npc.velocity.X > -maxSpeed)
                {
                    npc.velocity.X -= acceleration;

                    if (npc.velocity.X > maxSpeed)
                    {
                        npc.velocity.X = npc.velocity.X - acceleration;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X - acceleration * 0.5f;
                    }

                    if (npc.velocity.X < -maxSpeed)
                    {
                        npc.velocity.X = -maxSpeed;
                    }
                }
                else if (npc.direction == 1 && npc.velocity.X < maxSpeed)
                {
                    npc.velocity.X = npc.velocity.X + acceleration;

                    if (npc.velocity.X < -maxSpeed)
                    {
                        npc.velocity.X = npc.velocity.X + acceleration;
                    }
                    else if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X + acceleration * 0.5f;
                    }

                    if (npc.velocity.X > maxSpeed)
                    {
                        npc.velocity.X = maxSpeed;
                    }
                }

                float xDistance = Math.Abs(npc.Center.X - Main.player[npc.target].Center.X);
                float yLimiter = Main.player[npc.target].position.Y - (npc.height / 2f);
                if (xDistance > 50f)
                {
                    yLimiter -= 100f;
                }

                if (npc.position.Y < yLimiter)
                {
                    npc.velocity.Y = npc.velocity.Y + acceleration * 0.5f;

                    if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + acceleration * 0.1f;
                    }
                }
                else
                {
                    npc.velocity.Y = npc.velocity.Y - acceleration * 0.5f;

                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y - acceleration * 0.1f;
                    }
                }

                if (npc.velocity.Y < -maxSpeed)
                {
                    npc.velocity.Y = -maxSpeed;
                }

                if (npc.velocity.Y > maxSpeed)
                {
                    npc.velocity.Y = maxSpeed;
                }
            }
            //Change velocity if wet.
            if (npc.wet)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.95f;
                }

                npc.velocity.Y = npc.velocity.Y - 0.5f;

                if (npc.velocity.Y < -4f)
                {
                    npc.velocity.Y = -4f;
                }
            }
        }

        /// <summary>
        /// Allows you to spawn dust on the NPC in a certain place. Uses the npc.position value as the base point for the rectangle.
        /// Takes direction and rotation into account.
        /// </summary>
        /// <param name="frameWidth">The width of the sheet for the NPC.</param>
        /// <param name="rect">The place to put a dust.</param>
        /// <param name="chance">The chance to spawn a dust (0.3 = 30%)</param>
        public static Dust SpawnDustOnNPC(NPC npc, int frameWidth, int frameHeight, int dustType, Rectangle rect, Vector2 velocity = default, float chance = 0.5f, bool useSpriteDirection = false)
        {
            Vector2 half = new Vector2(frameWidth / 2f, frameHeight / 2f);

            //"flip" the rectangle's position x-wise.
            if ((!useSpriteDirection && npc.direction == 1) || (useSpriteDirection && npc.spriteDirection == 1))
            {
                rect.X = frameWidth - rect.Right;
            }

            if (Main.rand.NextFloat(1f) < chance)
            {
                Vector2 offset = (npc.Center - half + new Vector2(Main.rand.NextFloat(rect.Left, rect.Right), Main.rand.NextFloat(rect.Top, rect.Bottom))) - npc.Center;
                offset = offset.RotatedBy(npc.rotation);
                Dust d = Dust.NewDustPerfect(npc.Center + offset, dustType, velocity);
                return d;
            }
            return null;
        }
        #endregion
    }
}
