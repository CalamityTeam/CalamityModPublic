using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    [AutoloadBossHead]
    public class PlaguebringerGoliath : ModNPC
    {
		private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
		private const float MissileAngleSpread = 60;
        private const int MissileProjectiles = 8;
        private int MissileCountdown = 0;
        private int despawnTimer = 120;
        private int chargeDistance = 0;
        private bool charging = false;
        private bool halfLife = false;
        private bool canDespawn = false;
        private bool flyingFrame2 = false;
        private int curTex = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Plaguebringer Goliath");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.npcSlots = 64f;
            npc.width = 198;
            npc.height = 198;
            npc.defense = 50;
			npc.DR_NERD(0.3f);
			npc.LifeMaxNERB(88750, 106500, 370000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.boss = true;
            npc.value = Item.buyPrice(0, 25, 0, 0);
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            music = CalamityMod.Instance.GetMusicFromMusicMod("PlaguebringerGoliath") ?? MusicID.Boss3;
            bossBag = ModContent.ItemType<PlaguebringerGoliathBag>();
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToElectricity = true;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(biomeEnrageTimer);
			writer.Write(halfLife);
            writer.Write(canDespawn);
            writer.Write(flyingFrame2);
            writer.Write(MissileCountdown);
            writer.Write(despawnTimer);
            writer.Write(chargeDistance);
            writer.Write(charging);
            for (int i = 0; i < 4; i++)
                writer.Write(npc.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			biomeEnrageTimer = reader.ReadInt32();
			halfLife = reader.ReadBoolean();
            canDespawn = reader.ReadBoolean();
            flyingFrame2 = reader.ReadBoolean();
            MissileCountdown = reader.ReadInt32();
            despawnTimer = reader.ReadInt32();
            chargeDistance = reader.ReadInt32();
            charging = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                npc.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            //drawcode adjustments for the new sprite
            npc.gfxOffY = charging ? -40 : -50;
            npc.width = npc.frame.Width / 2;
            npc.height = (int)(npc.frame.Height * (charging ? 1.5f : 1.8f));

			// Mode variables
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases
			bool phase2 = lifeRatio < 0.75f;
			bool phase3 = lifeRatio < 0.5f;
			bool phase4 = lifeRatio < 0.25f;
			bool phase5 = lifeRatio < 0.1f;

			// Adjusts how 'challenging' the projectiles and enemies are to deal with
			float challengeAmt = (1f - lifeRatio) * 100f;
			float nukeBarrageChallengeAmt = (0.5f - lifeRatio) * 200f;

			// Adjust slowing debuff immunity
			bool immuneToSlowingDebuffs = npc.ai[0] == 0f || npc.ai[0] == 4f;
			npc.buffImmune[ModContent.BuffType<ExoFreeze>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<KamiDebuff>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TimeSlow>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TeslaFreeze>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
			npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
			npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

			// Light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.3f, 0.7f, 0f);

            // Show message
            if (!halfLife && phase3 && expertMode)
            {
                string key = "Mods.CalamityMod.PlagueBossText";
                Color messageColor = Color.Lime;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, $"Sounds/Custom/PlagueSounds/PBGNukeWarning"), npc.Center);

                halfLife = true;
            }

            // Missile countdown
            if (halfLife && MissileCountdown == 0)
                MissileCountdown = 600;
            if (MissileCountdown > 1)
                MissileCountdown--;

			Vector2 vectorCenter = npc.Center;

            // Count nearby players
            int num1038 = 0;
            for (int num1039 = 0; num1039 < Main.maxPlayers; num1039++)
            {
                if (Main.player[num1039].active && !Main.player[num1039].dead && (vectorCenter - Main.player[num1039].Center).Length() < 1000f)
                    num1038++;
            }

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

            // Distance from target
            Vector2 distFromPlayer = player.Center - vectorCenter;

			// Enrage
			if (!player.ZoneJungle && !BossRushEvent.BossRushActive)
			{
				if (biomeEnrageTimer > 0)
					biomeEnrageTimer--;
			}
			else
				biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

			bool biomeEnraged = biomeEnrageTimer <= 0 || malice;

			float enrageScale = death ? 0.5f : 0f;
            if (biomeEnraged)
            {
                npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
                enrageScale += 1.5f;
            }

			if (enrageScale > 1.5f)
				enrageScale = 1.5f;

			if (BossRushEvent.BossRushActive)
				enrageScale = 2f;

			bool diagonalDash = (revenge && phase2) || malice;

			if (npc.ai[0] != 0f && npc.ai[0] != 4f)
				npc.rotation = npc.velocity.X * 0.02f;

			// Despawn
			if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 5600f)
            {
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 5600f)
				{
					if (despawnTimer > 0)
						despawnTimer--;
				}
            }
            else
                despawnTimer = 120;

            canDespawn = despawnTimer <= 0;
            if (canDespawn)
            {
				if (npc.velocity.Y > 3f)
					npc.velocity.Y = 3f;
				npc.velocity.Y -= 0.2f;
				if (npc.velocity.Y < -16f)
					npc.velocity.Y = -16f;

				if (npc.timeLeft > 60)
                    npc.timeLeft = 60;

				if (npc.ai[0] != -1f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					MissileCountdown = 0;
					chargeDistance = 0;
					npc.netUpdate = true;
				}
				return;
            }

			// Always start in enemy spawning phase
			if (calamityGlobalNPC.newAI[3] == 0f)
			{
				calamityGlobalNPC.newAI[3] = 1f;
				npc.ai[0] = 2f;
				npc.netUpdate = true;
			}

            // Phase switch
            if (npc.ai[0] == -1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num596;
                    do num596 = MissileCountdown == 1 ? 4 : Main.rand.Next(4);
                    while (num596 == npc.ai[1] || num596 == 1);

					if (num596 == 0 && diagonalDash && distFromPlayer.Length() < 1800f)
					{
						do
						{
							switch (Main.rand.Next(3))
							{
								case 0:
									chargeDistance = 0;
									break;
								case 1:
									chargeDistance = 400;
									break;
								case 2:
									chargeDistance = -400;
									break;
							}
						}
						while (chargeDistance == npc.ai[3]);

						npc.ai[3] = -chargeDistance;
					}
                    npc.ai[0] = num596;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
					npc.TargetClosest();
                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, $"Sounds/Custom/PlagueSounds/PBGAttackSwitch{Main.rand.Next(1, 3)}"), npc.Center);
                }
            }

            // Charge phase
            else if (npc.ai[0] == 0f)
            {
				float num1044 = revenge ? 28f : 26f;
				if (phase2)
					num1044 += 1f;
				if (phase3)
					num1044 += 1f;
				if (phase4)
					num1044 += 1f;
				if (phase5)
					num1044 += 1f;

				num1044 += 2f * enrageScale;

				int num1043 = (int)Math.Ceiling(2f + enrageScale);
                if ((npc.ai[1] > (2 * num1043) && npc.ai[1] % 2f == 0f) || distFromPlayer.Length() > 1800f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, $"Sounds/Custom/PlagueSounds/PBGAttackSwitch{Main.rand.Next(1, 3)}"), npc.Center);

                    return;
                }

                // Charge
                if (npc.ai[1] % 2f == 0f)
                {
					float playerLocation = vectorCenter.X - player.Center.X;

					float num620 = 20f;
					num620 += 20f * enrageScale;

					if (Math.Abs(npc.Center.Y - (player.Center.Y - chargeDistance)) < num620)
                    {
						if (diagonalDash)
						{
							switch (Main.rand.Next(3))
							{
								case 0:
									chargeDistance = 0;
									break;
								case 1:
									chargeDistance = 400;
									break;
								case 2:
									chargeDistance = -400;
									break;
							}
						}

                        charging = true;
                        npc.frameCounter = 4;

                        npc.ai[1] += 1f;
                        npc.ai[2] = 0f;

                        float num1045 = player.position.X + (player.width / 2) - vectorCenter.X;
                        float num1046 = player.position.Y + (player.height / 2) - vectorCenter.Y;
                        float num1047 = (float)Math.Sqrt(num1045 * num1045 + num1046 * num1046);

                        num1047 = num1044 / num1047;
                        npc.velocity.X = num1045 * num1047;
                        npc.velocity.Y = num1046 * num1047;
						npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

						calamityGlobalNPC.newAI[1] = npc.velocity.X;
						calamityGlobalNPC.newAI[2] = npc.velocity.Y;

						npc.direction = playerLocation < 0 ? 1 : -1;
                        npc.spriteDirection = npc.direction;
						if (npc.spriteDirection != 1)
							npc.rotation += (float)Math.PI;

                        npc.netUpdate = true;
                        npc.netSpam -= 5;

                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/PlagueSounds/PBGDash"), npc.Center);
                        return;
                    }

					npc.rotation = npc.velocity.X * 0.02f;
					charging = false;

                    float num1048 = revenge ? 14f : 12f;
                    float num1049 = revenge ? 0.25f : 0.22f;
                    if (phase2)
                    {
                        num1048 += 1f;
                        num1049 += 0.05f;
                    }
                    if (phase4)
                    {
                        num1048 += 1f;
                        num1049 += 0.05f;
                    }
					num1048 += 1.5f * enrageScale;
					num1049 += 0.25f * enrageScale;

					if (vectorCenter.Y < (player.Center.Y - chargeDistance))
                        npc.velocity.Y += num1049;
                    else
                        npc.velocity.Y -= num1049;

                    if (npc.velocity.Y < -num1048)
                        npc.velocity.Y = -num1048;
                    if (npc.velocity.Y > num1048)
                        npc.velocity.Y = num1048;

                    if (Math.Abs(vectorCenter.X - player.Center.X) > 650f)
                        npc.velocity.X += num1049 * npc.direction;
                    else if (Math.Abs(vectorCenter.X - player.Center.X) < 500f)
                        npc.velocity.X -= num1049 * npc.direction;
                    else
                        npc.velocity.X *= 0.8f;

                    if (npc.velocity.X < -num1048)
                        npc.velocity.X = -num1048;
                    if (npc.velocity.X > num1048)
                        npc.velocity.X = num1048;

                    npc.direction = playerLocation < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;

                    npc.netUpdate = true;
                    npc.netSpam -= 5;
                }

                // Slow down after charge
                else
                {
                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;

                    int num1050 = revenge ? 525 : 550;
                    if (phase4)
						num1050 = revenge ? 450 : 475;
					else if (phase3)
                        num1050 = revenge ? 475 : 500;
                    else if (phase2)
                        num1050 = revenge ? 500 : 525;
					num1050 -= (int)(25f * enrageScale);

					int num1051 = 1;
                    if (vectorCenter.X < player.Center.X)
                        num1051 = -1;

					if (npc.direction == num1051 && (Math.Abs(vectorCenter.X - player.Center.X) > num1050 || Math.Abs(vectorCenter.Y - player.Center.Y) > num1050))
						npc.ai[2] = 1f;

					if (enrageScale > 0 && npc.ai[2] == 1f)
						npc.velocity *= 0.95f;

					if (npc.ai[2] != 1f)
                    {
                        charging = true;
                        npc.frameCounter = 4;

                        // Velocity fix if PBG slowed
                        if (npc.velocity.Length() < num1044)
							npc.velocity = new Vector2(calamityGlobalNPC.newAI[1], calamityGlobalNPC.newAI[2]);

						calamityGlobalNPC.newAI[0] += 1f;
						if (calamityGlobalNPC.newAI[0] > 90f)
							npc.velocity *= 1.01f;

                        npc.netUpdate = true;
						return;
                    }

					float playerLocation = vectorCenter.X - player.Center.X;
					npc.direction = playerLocation < 0 ? 1 : -1;
					npc.spriteDirection = npc.direction;

					npc.rotation = npc.velocity.X * 0.02f;
					charging = false;

                    npc.velocity *= 0.9f;
                    float num1052 = revenge ? 0.12f : 0.1f;
                    if (phase2)
                    {
                        npc.velocity *= 0.98f;
                        num1052 += 0.05f;
                    }
                    if (phase3)
                    {
                        npc.velocity *= 0.98f;
                        num1052 += 0.05f;
                    }
                    if (phase4)
                    {
                        npc.velocity *= 0.98f;
                        num1052 += 0.05f;
                    }
					if (enrageScale > 0)
						npc.velocity *= 0.95f;

					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num1052)
                    {
                        npc.ai[2] = 0f;
                        npc.ai[1] += 1f;
						calamityGlobalNPC.newAI[0] = 0f;
                    }
				}
            }

            // Move closer if too far away
            else if (npc.ai[0] == 2f)
            {
                float playerLocation = vectorCenter.X - player.Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

                float num1055 = player.position.X + (player.width / 2) - vectorCenter.X;
                float num1056 = player.position.Y + (player.height / 2) - 200f - vectorCenter.Y;
                float num1057 = (float)Math.Sqrt(num1055 * num1055 + num1056 * num1056);

				calamityGlobalNPC.newAI[0] += 1f;
				if (num1057 < 600f || calamityGlobalNPC.newAI[0] >= 180f)
                {
                    npc.ai[0] = (phase3 || malice) ? 5f : 1f;
                    npc.ai[1] = 0f;
					calamityGlobalNPC.newAI[0] = 0f;
					npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, $"Sounds/Custom/PlagueSounds/PBGAttackSwitch{Main.rand.Next(1, 3)}"), npc.Center);

                    return;
                }

                // Move closer
                Movement(100f, 350f, 450f, player, enrageScale);
            }

            // Spawn less missiles
            else if (npc.ai[0] == 1f)
            {
                charging = false;
                Vector2 vector119 = new Vector2(npc.direction == 1 ? npc.getRect().BottomLeft().X : npc.getRect().BottomRight().X, npc.getRect().Bottom().Y + 20f);
                vector119.X += npc.direction * 120;
                float num1058 = player.position.X + (player.width / 2) - vectorCenter.X;
                float num1059 = player.position.Y + (player.height / 2) - vectorCenter.Y;
                float num1060 = (float)Math.Sqrt(num1058 * num1058 + num1059 * num1059);

                npc.ai[1] += 1f;
                npc.ai[1] += num1038 / 2;
                if (phase2)
                    npc.ai[1] += 0.5f;

                bool flag103 = false;
                if (npc.ai[1] > 40f - 12f * enrageScale)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] += 1f;
                    flag103 = true;
                }

                if (flag103)
                {
                    Main.PlaySound(SoundID.NPCHit, (int)npc.position.X, (int)npc.position.Y, 8);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (expertMode && NPC.CountNPCS(ModContent.NPCType<PlagueMine>()) < 2)
							NPC.NewNPC((int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueMine>(), 0, 0f, 0f, 0f, challengeAmt);

						float npcSpeed = (revenge ? 9f : 7f) + enrageScale * 2f;

						float num1071 = player.position.X + player.width * 0.5f - vector119.X;
						float num1072 = player.position.Y + player.height * 0.5f - vector119.Y;
						float num1073 = (float)Math.Sqrt(num1071 * num1071 + num1072 * num1072);

						num1073 = npcSpeed / num1073;
						num1071 *= num1073;
						num1072 *= num1073;

						int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueHomingMissile>(), 0, 0f, 0f, 0f, challengeAmt);
						Main.npc[num1062].velocity.X = num1071;
						Main.npc[num1062].velocity.Y = num1072;
						Main.npc[num1062].netUpdate = true;
					}
				}

                // Move closer if too far away
                if (num1060 > 600f)
                    Movement(100f, 350f, 450f, player, enrageScale);
                else
                    npc.velocity *= 0.9f;

                float playerLocation = vectorCenter.X - player.Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

                if (npc.ai[2] > 3f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 2f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, $"Sounds/Custom/PlagueSounds/PBGAttackSwitch{Main.rand.Next(1, 3)}"), npc.Center);
                }
            }

            // Missile spawn
            else if (npc.ai[0] == 5f)
            {
                charging = false;
                Vector2 vector119 = new Vector2(npc.direction == 1 ? npc.getRect().BottomLeft().X : npc.getRect().BottomRight().X, npc.getRect().Bottom().Y + 20f);
                vector119.X += npc.direction * 120;
                float num1058 = player.position.X + (player.width / 2) - vectorCenter.X;
                float num1059 = player.position.Y + (player.height / 2) - vectorCenter.Y;
                float num1060 = (float)Math.Sqrt(num1058 * num1058 + num1059 * num1059);

                npc.ai[1] += 1f;
                npc.ai[1] += num1038 / 2;
                bool flag103 = false;
                if (phase4)
                    npc.ai[1] += 0.5f;
                if (phase5)
                    npc.ai[1] += 0.5f;

                if (npc.ai[1] % 20f == 19f)
                    npc.netUpdate = true;

                if (npc.ai[1] > 30f - 12f * enrageScale)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] += 1f;
                    flag103 = true;
                }

                if (flag103)
                {
                    Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 88);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (expertMode && NPC.CountNPCS(ModContent.NPCType<PlagueMine>()) < 3)
							NPC.NewNPC((int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueMine>(), 0, 0f, 0f, 0f, challengeAmt);

						float npcSpeed = (revenge ? 11f : 9f) + enrageScale * 2f;

						float num1071 = player.position.X + player.width * 0.5f - vector119.X;
						float num1072 = player.position.Y + player.height * 0.5f - vector119.Y;
						float num1073 = (float)Math.Sqrt(num1071 * num1071 + num1072 * num1072);

						num1073 = npcSpeed / num1073;
						num1071 *= num1073;
						num1072 *= num1073;
						num1071 += Main.rand.Next(-20, 21) * 0.05f;
						num1072 += Main.rand.Next(-20, 21) * 0.05f;

						int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueHomingMissile>(), 0, 0f, 0f, 0f, challengeAmt);
						Main.npc[num1062].velocity.X = num1071;
						Main.npc[num1062].velocity.Y = num1072;
						Main.npc[num1062].netUpdate = true;
					}
                }

                // Move closer if too far away
                if (num1060 > 600f)
                    Movement(100f, 350f, 450f, player, enrageScale);
                else
                    npc.velocity *= 0.9f;

                float playerLocation = vectorCenter.X - player.Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

                if (npc.ai[2] > 5f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 2f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, $"Sounds/Custom/PlagueSounds/PBGAttackSwitch{Main.rand.Next(1, 3)}"), npc.Center);
                }
            }

            // Stinger phase
            else if (npc.ai[0] == 3f)
            {
                Vector2 vector121 = new Vector2(npc.direction == 1 ? npc.getRect().BottomLeft().X : npc.getRect().BottomRight().X, npc.getRect().Bottom().Y + 20f);
                vector121.X += npc.direction * 120;

				npc.ai[1] += 1f;
				int num650 = phase5 ? 20 : (phase3 ? 25 : 30);
				num650 -= (int)Math.Ceiling(5f * enrageScale);

				if (npc.ai[1] % num650 == (num650 - 1) && vectorCenter.Y < player.position.Y)
                {
                    Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 42);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float projectileSpeed = revenge ? 6f : 5f;
						projectileSpeed += 2f * enrageScale;

                        float num1071 = player.position.X + player.width * 0.5f - vector121.X;
                        float num1072 = player.position.Y + player.height * 0.5f - vector121.Y;
                        float num1073 = (float)Math.Sqrt(num1071 * num1071 + num1072 * num1072);
                        num1073 = projectileSpeed / num1073;
                        num1071 *= num1073;
                        num1072 *= num1073;

                        int type = ModContent.ProjectileType<PlagueStingerGoliathV2>();
						switch ((int)npc.ai[2])
						{
							case 0:
							case 1:
								break;
							case 2:
							case 3:
								if (expertMode)
									type = ModContent.ProjectileType<PlagueStingerGoliath>();
								break;
							case 4:
								type = ModContent.ProjectileType<HiveBombGoliath>();
								break;
						}

						int damage = npc.GetProjectileDamage(type);
						Projectile.NewProjectile(vector121.X, vector121.Y, num1071, num1072, type, damage, 0f, Main.myPlayer, challengeAmt, player.position.Y);
                        npc.netUpdate = true;
                    }

					npc.ai[2] += 1f;
					if (npc.ai[2] > 4f)
						npc.ai[2] = 0f;
				}

                Movement(100f, 400f, 500f, player, enrageScale);

                float playerLocation = vectorCenter.X - player.Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

				if (npc.ai[1] > num650 * 10f)
				{
                    npc.ai[0] = -1f;
                    npc.ai[1] = 3f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, $"Sounds/Custom/PlagueSounds/PBGAttackSwitch{Main.rand.Next(1, 3)}"), npc.Center);
                }
            }

            // Missile charge
            else if (npc.ai[0] == 4f)
            {
				float num1044 = revenge ? 28f : 26f;

				num1044 += 3f * enrageScale;

				int num1043 = (int)Math.Ceiling(2f + enrageScale);
                if (npc.ai[1] > (2 * num1043) && npc.ai[1] % 2f == 0f)
                {
                    MissileCountdown = 0;
                    npc.ai[0] = -1f;
                    npc.ai[1] = -1f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, $"Sounds/Custom/PlagueSounds/PBGAttackSwitch{Main.rand.Next(1, 3)}"), npc.Center);
                    return;
                }

                // Charge
                if (npc.ai[1] % 2f == 0f)
                {
                    float playerLocation = vectorCenter.X - player.Center.X;

					float num620 = 20f;
					num620 += 20 * enrageScale;

					if (Math.Abs(vectorCenter.Y - (player.Center.Y - 500f)) < num620)
                    {
                        if (MissileCountdown == 1)
                        {
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/PlagueSounds/PBGBarrageLaunch"), npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = revenge ? 6f : 5f;
								speed += 2f * enrageScale;

								int type = ModContent.ProjectileType<HiveBombGoliath>();
								int damage = npc.GetProjectileDamage(type);

								Vector2 baseVelocity = player.Center - vectorCenter;
                                baseVelocity.Normalize();
                                baseVelocity *= speed;

								int missiles = malice ? 16 : MissileProjectiles;
								int spread = malice ? 18 : 24;
                                for (int i = 0; i < missiles; i++)
                                {
                                    Vector2 spawn = vectorCenter; // Normal = 96, Malice = 144
                                    spawn.X += i * (int)(spread * 1.125) - (missiles * (spread / 2)); // Normal = -96 to 93, Malice = -144 to 156
                                    Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-MissileAngleSpread / 2 + (MissileAngleSpread * i / missiles)));
                                    Projectile.NewProjectile(spawn, velocity, type, damage, 0f, Main.myPlayer, nukeBarrageChallengeAmt, player.position.Y);
                                }
                            }
                        }

                        charging = true;

                        npc.ai[1] += 1f;
                        npc.ai[2] = 0f;

                        float num1045 = player.position.X + (player.width / 2) - vectorCenter.X;
                        float num1046 = player.position.Y - 500f + (player.height / 2) - vectorCenter.Y;
                        float num1047 = (float)Math.Sqrt(num1045 * num1045 + num1046 * num1046);

                        num1047 = num1044 / num1047;
                        npc.velocity.X = num1045 * num1047;
                        npc.velocity.Y = num1046 * num1047;
						npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

						npc.direction = playerLocation < 0 ? 1 : -1;
                        npc.spriteDirection = npc.direction;
						if (npc.spriteDirection != 1)
							npc.rotation += (float)Math.PI;

                        npc.netUpdate = true;

						return;
                    }

					npc.rotation = npc.velocity.X * 0.02f;
					charging = false;

                    float num1048 = revenge ? 16f : 14f;
                    float num1049 = revenge ? 0.2f : 0.18f;
					num1048 += 1.5f * enrageScale;
					num1049 += 0.25f * enrageScale;

                    if (vectorCenter.Y < player.Center.Y - 500f)
                        npc.velocity.Y += num1049;
                    else
                        npc.velocity.Y -= num1049;

                    if (npc.velocity.Y < -num1048)
                        npc.velocity.Y = -num1048;
                    if (npc.velocity.Y > num1048)
                        npc.velocity.Y = num1048;

                    if (Math.Abs(vectorCenter.X - player.Center.X) > 600f)
                        npc.velocity.X += num1049 * npc.direction;
                    else if (Math.Abs(vectorCenter.X - player.Center.X) < 300f)
                        npc.velocity.X -= num1049 * npc.direction;
                    else
                        npc.velocity.X *= 0.8f;

                    if (npc.velocity.X < -num1048)
                        npc.velocity.X = -num1048;
                    if (npc.velocity.X > num1048)
                        npc.velocity.X = num1048;

                    npc.direction = playerLocation < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;
				}

                // Slow down after charge
                else
                {
                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;

                    int num1050 = 600;
                    int num1051 = 1;

                    if (vectorCenter.X < player.Center.X)
                        num1051 = -1;
                    if (npc.direction == num1051 && Math.Abs(vectorCenter.X - player.Center.X) > num1050)
                        npc.ai[2] = 1f;
					if (enrageScale > 0 && npc.ai[2] == 1f)
						npc.velocity *= 0.95f;

					if (npc.ai[2] != 1f)
                    {
                        charging = true;

                        // Velocity fix if PBG slowed
                        if (npc.velocity.Length() < num1044)
							npc.velocity.X = num1044 * npc.direction;

						calamityGlobalNPC.newAI[0] += 1f;
						if (calamityGlobalNPC.newAI[0] > 90f)
							npc.velocity.X *= 1.01f;

						return;
                    }

					npc.rotation = npc.velocity.X * 0.02f;
					charging = false;

                    npc.velocity *= 0.9f;
                    float num1052 = revenge ? 0.12f : 0.1f;
                    if (phase3)
                    {
                        npc.velocity *= 0.9f;
                        num1052 += 0.05f;
                    }
                    if (phase4)
                    {
                        npc.velocity *= 0.9f;
                        num1052 += 0.05f;
                    }
                    if (phase5)
                    {
                        npc.velocity *= 0.9f;
                        num1052 += 0.05f;
                    }
					if (enrageScale > 0)
						npc.velocity *= 0.95f;

					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num1052)
                    {
                        npc.ai[2] = 0f;
                        npc.ai[1] += 1f;
						calamityGlobalNPC.newAI[0] = 0f;
                    }
				}
            }
        }

        private void Movement(float xPos, float yPos, float yPos2, Player player, float enrageScale)
        {
			Vector2 acceleration = new Vector2(0.1f, 0.15f);
			Vector2 velocity = new Vector2(8f, 5f);
			float deceleration = 0.98f;

			acceleration *= 0.1f * enrageScale + 1f;
			velocity *= 1f - enrageScale * 0.1f;
			if (CalamityWorld.malice || BossRushEvent.BossRushActive)
				velocity *= 0.5f;
			deceleration *= 1f - enrageScale * 0.1f;

			if (npc.position.Y > player.position.Y - yPos)
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= deceleration;
                npc.velocity.Y -= acceleration.Y;
                if (npc.velocity.Y > velocity.Y)
                    npc.velocity.Y = velocity.Y;
            }
            else if (npc.position.Y < player.position.Y - yPos2)
            {
                if (npc.velocity.Y < 0f)
                    npc.velocity.Y *= deceleration;
                npc.velocity.Y += acceleration.Y;
                if (npc.velocity.Y < -velocity.Y)
                    npc.velocity.Y = -velocity.Y;
            }

            if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + xPos)
            {
                if (npc.velocity.X > 0f)
                    npc.velocity.X *= deceleration;
                npc.velocity.X -= acceleration.X;
                if (npc.velocity.X > velocity.X)
                    npc.velocity.X = velocity.X;
            }
            if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - xPos)
            {
                if (npc.velocity.X < 0f)
                    npc.velocity.X *= deceleration;
                npc.velocity.X += acceleration.X;
                if (npc.velocity.X < -velocity.X)
                    npc.velocity.X = -velocity.X;
            }
        }

        public override bool CheckActive() => canDespawn;

		// Can only hit the target if within certain distance
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= 100f;
		}

		public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int i = 1; i < 7; i++)
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("PlaguebringerGoliathGore" + i), 2f);
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 200;
                npc.height = 200;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
			Texture2D glowTexture = ModContent.GetTexture("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliathGlow");
			if (curTex != (charging ? 2 : 1))
            {
                npc.frame.X = 0;
                npc.frame.Y = 0;
            }
            if (charging)
            {
                curTex = 2;
                texture = ModContent.GetTexture("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliathChargeTex");
				glowTexture = ModContent.GetTexture("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliathChargeTexGlow");
			}
            else
            {
                curTex = 1;
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

			int frameCount = 3;
			Rectangle rectangle = new Rectangle(npc.frame.X, npc.frame.Y, texture.Width / 2, texture.Height / frameCount);
			Vector2 vector11 = rectangle.Size() / 2f;
			Vector2 posOffset = new Vector2(charging ? 175 : 125, 0);
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 10;
			if (npc.ai[0] != 0f && npc.ai[0] != 4f)
				num153 = 7;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture.Width, texture.Height / frameCount) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + posOffset;
					spriteBatch.Draw(texture, vector41, new Rectangle?(rectangle), color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture.Width, texture.Height / frameCount) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + posOffset;
			spriteBatch.Draw(texture, vector43, new Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 *= (num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2(glowTexture.Width, glowTexture.Height / frameCount) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + posOffset;
					spriteBatch.Draw(glowTexture, vector44, new Rectangle?(rectangle), color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(glowTexture, vector43, new Rectangle?(rectangle), color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
        }

        public override void FindFrame(int frameHeight)
        {
            int width = !charging ? (532 / 2) : (644 / 2);
            int height = !charging ? (768 / 3) : (636 / 3);
            npc.frameCounter += 1.0;

            if (npc.frameCounter > 4.0)
            {
                npc.frame.Y = npc.frame.Y + height;
                npc.frameCounter = 0.0;
            }
            if (npc.frame.Y >= height * 3)
            {
                npc.frame.Y = 0;
                npc.frame.X = npc.frame.X == 0 ? width : 0;
                if (charging)
                {
                   flyingFrame2 = !flyingFrame2;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			DropHelper.DropItemChance(npc, ModContent.ItemType<PlaguebringerGoliathTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgePlaguebringerGoliath>(), true, !CalamityWorld.downedPlaguebringer);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<PlagueCellCluster>(), 10, 14);
                DropHelper.DropItemSpray(npc, ModContent.ItemType<InfectedArmorPlating>(), 13, 17);
                DropHelper.DropItemSpray(npc, ItemID.Stinger, 3, 5);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<VirulentKatana>(w), // Virulence
                    DropHelper.WeightStack<DiseasedPike>(w),
                    DropHelper.WeightStack<ThePlaguebringer>(w), // Pandemic
                    DropHelper.WeightStack<Malevolence>(w),
                    DropHelper.WeightStack<PestilentDefiler>(w),
                    DropHelper.WeightStack<TheHive>(w),
                    DropHelper.WeightStack<MepheticSprayer>(w), // Blight Spewer
                    DropHelper.WeightStack<PlagueStaff>(w),
                    DropHelper.WeightStack<FuelCellBundle>(w),
                    DropHelper.WeightStack<InfectedRemote>(w),
                    DropHelper.WeightStack<TheSyringe>(w)
				);

				// Equipment
				DropHelper.DropItem(npc, ModContent.ItemType<ToxicHeart>(), true);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
                DropHelper.DropItemChance(npc, ModContent.ItemType<PlagueCaller>(), 10);
            }

            DropHelper.DropItemCondition(npc, ModContent.ItemType<Malachite>(), !Main.expertMode, 0.1f);

            // Mark PBG as dead
            CalamityWorld.downedPlaguebringer = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 300, true);
        }
    }
}
