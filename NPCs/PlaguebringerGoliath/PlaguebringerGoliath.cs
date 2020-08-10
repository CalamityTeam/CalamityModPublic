using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
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
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    [AutoloadBossHead]
    public class PlaguebringerGoliath : ModNPC
    {
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
            npc.damage = 100;
            npc.npcSlots = 64f;
            npc.width = 198;
            npc.height = 198;
            npc.defense = 40;
			npc.DR_NERD(0.4f);
			npc.LifeMaxNERB(81000, 106500, 3700000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.boss = true;
            npc.value = Item.buyPrice(0, 25, 0, 0);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[BuffID.Frostburn] = false;
			npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[BuffID.BetsysCurse] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.Oiled] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WarCleave>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/PlaguebringerGoliath");
            else
                music = MusicID.Boss3;
            bossBag = ModContent.ItemType<PlaguebringerGoliathBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(halfLife);
            writer.Write(canDespawn);
            writer.Write(flyingFrame2);
            writer.Write(MissileCountdown);
            writer.Write(despawnTimer);
            writer.Write(chargeDistance);
            writer.Write(charging);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            halfLife = reader.ReadBoolean();
            canDespawn = reader.ReadBoolean();
            flyingFrame2 = reader.ReadBoolean();
            MissileCountdown = reader.ReadInt32();
            despawnTimer = reader.ReadInt32();
            chargeDistance = reader.ReadInt32();
            charging = reader.ReadBoolean();
        }

        public override void AI()
        {
            //drawcode adjustments for the new sprite
            npc.gfxOffY = charging ? -40 : -50;
            npc.width = npc.frame.Width / 2;
            npc.height = (int)(npc.frame.Height * (charging ? 1.5f : 1.8f));

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Mode variables
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;

            // Light
            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.3f, 0.7f, 0f);

            // Show message
            if (!halfLife && ((lifeRatio < 0.5f && expertMode) || death))
            {
                string key = "Mods.CalamityMod.PlagueBossText";
                Color messageColor = Color.Lime;
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(key), messageColor);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

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
            for (int num1039 = 0; num1039 < 255; num1039++)
            {
                if (Main.player[num1039].active && !Main.player[num1039].dead && (vectorCenter - Main.player[num1039].Center).Length() < 1000f)
                    num1038++;
            }

            // Defense gain
            if (expertMode)
            {
                int num1040 = death ? 20 : (int)(20f * (1f - lifeRatio));
                npc.defense = npc.defDefense + num1040;
            }

            // Target
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest(true);

			Player player = Main.player[npc.target];

            // Distance from target
            Vector2 distFromPlayer = player.Center - vectorCenter;

			// Enrage
			int enrageScale = 0;
			if ((npc.position.Y / 16f) < Main.worldSurface || (npc.position.Y / 16f) > (Main.maxTilesY - 200))
				enrageScale++;
			if (!player.ZoneJungle)
				enrageScale++;

			if (CalamityWorld.bossRushActive)
				enrageScale = 0;

			bool diagonalDash = revenge && (lifeRatio < 0.8f || death);

			if (npc.ai[0] != 0f && npc.ai[0] != 4f)
			{
				npc.rotation = npc.velocity.X * 0.02f;
			}

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
                    npc.ai[0] = num596;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }
            }

            // Charge phase
            else if (npc.ai[0] == 0f)
            {
				float num1044 = revenge ? 28f : 26f;
				if (lifeRatio < 0.66f || death)
					num1044 += 2f;
				if (lifeRatio < 0.33f || death)
					num1044 += 2f;
				if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
					num1044 += 2f;

				num1044 += 6f * enrageScale;

				int num1043 = 2 + enrageScale;
                if ((npc.ai[1] > (2 * num1043) && npc.ai[1] % 2f == 0f) || distFromPlayer.Length() > 1800f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                    return;
                }

                // Charge
                if (npc.ai[1] % 2f == 0f)
                {
                    npc.TargetClosest(true);

                    float playerLocation = vectorCenter.X - player.Center.X;

					float num620 = 20f;
					num620 += 20 * enrageScale;

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

						npc.Calamity().newAI[1] = npc.velocity.X;
						npc.Calamity().newAI[2] = npc.velocity.Y;

						npc.direction = playerLocation < 0 ? 1 : -1;
                        npc.spriteDirection = npc.direction;
						if (npc.spriteDirection != 1)
							npc.rotation += (float)Math.PI;

						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0);
                        return;
                    }

					npc.rotation = npc.velocity.X * 0.02f;
					charging = false;

                    float num1048 = revenge ? 14f : 12f;
                    float num1049 = revenge ? 0.25f : 0.22f;
                    if (lifeRatio < 0.66f || death)
                    {
                        num1048 += 1f;
                        num1049 += 0.05f;
                    }
                    if (lifeRatio < 0.33f || death)
                    {
                        num1048 += 1f;
                        num1049 += 0.05f;
                    }
                    if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
                    {
                        num1048 += 2f;
                        num1049 += 0.1f;
                    }
					num1048 += 3 * enrageScale;
					num1049 += 0.5f * enrageScale;

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

					if (npc.netSpam > 10)
						npc.netSpam = 10;
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
                    if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
                        num1050 = 300;
                    else if (CalamityWorld.bossRushActive)
                        num1050 = 400;
                    else if (lifeRatio < 0.33f || death)
                        num1050 = revenge ? 450 : 475;
                    else if (lifeRatio < 0.66f)
                        num1050 = revenge ? 475 : 500;
					num1050 -= 100 * enrageScale;

					int num1051 = 1;
                    if (vectorCenter.X < player.Center.X)
                        num1051 = -1;

					if (npc.direction == num1051 && (Math.Abs(vectorCenter.X - player.Center.X) > num1050 || Math.Abs(vectorCenter.Y - player.Center.Y) > num1050))
					{
						npc.ai[2] = 1f;
					}
					if (enrageScale > 0 && npc.ai[2] == 1f)
						npc.velocity *= 0.5f;

					if (npc.ai[2] != 1f)
                    {
                        charging = true;
                        npc.frameCounter = 4;

                        // Velocity fix if PBG slowed
                        if (npc.velocity.Length() < num1044)
							npc.velocity = new Vector2(npc.Calamity().newAI[1], npc.Calamity().newAI[2]);

						npc.Calamity().newAI[0] += 1f;
						if (npc.Calamity().newAI[0] > 90f)
							npc.velocity *= 1.01f;

						return;
                    }

                    npc.TargetClosest(true);

                    npc.spriteDirection = npc.direction;

					npc.rotation = npc.velocity.X * 0.02f;
					charging = false;

                    npc.velocity *= 0.9f;
                    float num1052 = revenge ? 0.12f : 0.1f;
                    if (lifeRatio < 0.8f || death)
                    {
                        npc.velocity *= 0.98f;
                        num1052 += 0.05f;
                    }
                    if (lifeRatio < 0.6f || death)
                    {
                        npc.velocity *= 0.98f;
                        num1052 += 0.05f;
                    }
                    if (lifeRatio < 0.4f || death)
                    {
                        npc.velocity *= 0.98f;
                        num1052 += 0.05f;
                    }
					if (enrageScale > 0)
						npc.velocity *= 0.7f;

					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num1052)
                    {
                        npc.ai[2] = 0f;
                        npc.ai[1] += 1f;
						npc.Calamity().newAI[0] = 0f;
                    }

					npc.netUpdate = true;

					if (npc.netSpam > 10)
						npc.netSpam = 10;
				}
            }

            // Move closer if too far away
            else if (npc.ai[0] == 2f)
            {
                npc.TargetClosest(true);

                float playerLocation = vectorCenter.X - player.Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

                float num1055 = player.position.X + (player.width / 2) - vectorCenter.X;
                float num1056 = player.position.Y + (player.height / 2) - 200f - vectorCenter.Y;
                float num1057 = (float)Math.Sqrt(num1055 * num1055 + num1056 * num1056);
                if (num1057 < 600f)
                {
                    npc.ai[0] = (lifeRatio < 0.66f || death) ? 5f : 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                    return;
                }

                // Move closer
                Movement(100f, 350f, 450f, player, enrageScale);
            }

            // Spawn bees
            else if (npc.ai[0] == 1f)
            {
                charging = false;

                npc.TargetClosest(true);

                Vector2 vector119 = new Vector2(npc.direction == 1 ? npc.getRect().BottomLeft().X : npc.getRect().BottomRight().X, npc.getRect().Bottom().Y - 40f);
                vector119.X += npc.direction * 120;
                float num1058 = player.position.X + (player.width / 2) - vectorCenter.X;
                float num1059 = player.position.Y + (player.height / 2) - vectorCenter.Y;
                float num1060 = (float)Math.Sqrt(num1058 * num1058 + num1059 * num1059);

                npc.ai[1] += 1f;
                npc.ai[1] += num1038 / 2;
                if (lifeRatio < 0.75f)
                    npc.ai[1] += 0.25f;
                if (lifeRatio < 0.5f)
                    npc.ai[1] += 0.25f;

                bool flag103 = false;
                if (npc.ai[1] > 40f - 12f * enrageScale)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] += 1f;
                    flag103 = true;
                }

                if (flag103)
                {
                    Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 8);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int randomAmt = expertMode ? 2 : 4;
                        if (Main.rand.Next(randomAmt) == 0)
                            randomAmt = ModContent.NPCType<PlagueBeeLargeG>();
                        else
                            randomAmt = ModContent.NPCType<PlagueBeeG>();

                        if (expertMode && NPC.CountNPCS(ModContent.NPCType<PlagueMine>()) < 2)
                            NPC.NewNPC((int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueMine>(), 0, 0f, 0f, 0f, 0f, 255);

                        if (revenge && !NPC.AnyNPCs(ModContent.NPCType<PlaguebringerShade>()))
                            NPC.NewNPC((int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlaguebringerShade>(), 0, 0f, 0f, 0f, 0f, 255);

                        if (NPC.CountNPCS(ModContent.NPCType<PlagueBeeLargeG>()) < 2)
                        {
                            int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y, randomAmt, 0, 0f, 0f, 0f, 0f, 255);

							Main.npc[num1062].velocity = player.Center - npc.Center;
							Main.npc[num1062].velocity.Normalize();
							Main.npc[num1062].velocity *= CalamityWorld.bossRushActive ? 12f : 6f;

							Main.npc[num1062].localAI[0] = 60f;
                            Main.npc[num1062].netUpdate = true;
                        }
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
                }
            }

            // Missile spawn
            else if (npc.ai[0] == 5f)
            {
                charging = false;

                npc.TargetClosest(true);

                Vector2 vector119 = new Vector2(npc.direction == 1 ? npc.getRect().BottomLeft().X : npc.getRect().BottomRight().X, npc.getRect().Bottom().Y - 40f);
                vector119.X += npc.direction * 120;
                float num1058 = player.position.X + (player.width / 2) - vectorCenter.X;
                float num1059 = player.position.Y + (player.height / 2) - vectorCenter.Y;
                float num1060 = (float)Math.Sqrt(num1058 * num1058 + num1059 * num1059);

                npc.ai[1] += 1f;
                npc.ai[1] += num1038 / 2;
                bool flag103 = false;
                if (lifeRatio < 0.25f || death)
                    npc.ai[1] += 0.25f;
                if (lifeRatio < 0.1f || death)
                    npc.ai[1] += 0.25f;

                if (npc.ai[1] > 40f - 12f * enrageScale)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] += 1f;
                    flag103 = true;
                }

                if (flag103)
                {
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 88);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (expertMode && NPC.CountNPCS(ModContent.NPCType<PlagueMine>()) < 3)
                            NPC.NewNPC((int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueMine>(), 0, 0f, 0f, 0f, 0f, 255);

                        if (revenge && !NPC.AnyNPCs(ModContent.NPCType<PlaguebringerShade>()))
                            NPC.NewNPC((int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlaguebringerShade>(), 0, 0f, 0f, 0f, 0f, 255);

                        float projectileSpeed = revenge ? 6f : 5f;
						if (CalamityWorld.bossRushActive)
							projectileSpeed *= 2f;

                        float num1071 = player.position.X + player.width * 0.5f - vector119.X;
                        float num1072 = player.position.Y + player.height * 0.5f - vector119.Y;
                        float num1073 = (float)Math.Sqrt(num1071 * num1071 + num1072 * num1072);

                        num1073 = projectileSpeed / num1073;
                        num1071 *= num1073;
                        num1072 *= num1073;

                        if (NPC.CountNPCS(ModContent.NPCType<PlagueHomingMissile>()) < 5)
                        {
                            int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y, ModContent.NPCType<PlagueHomingMissile>(), 0, 0f, 0f, 0f, 0f, 255);
                            Main.npc[num1062].velocity.X = num1071;
                            Main.npc[num1062].velocity.Y = num1072;
                            Main.npc[num1062].netUpdate = true;
                        }
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
                }
            }

            // Stinger phase
            else if (npc.ai[0] == 3f)
            {
                Vector2 vector121 = new Vector2(npc.direction == 1 ? npc.getRect().BottomLeft().X : npc.getRect().BottomRight().X, npc.getRect().Bottom().Y - 40f);
                vector121.X += npc.direction * 120;

				npc.ai[1] += 1f;
				int num650 = (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 10 : ((lifeRatio < 0.1f || death) ? 20 : ((lifeRatio < 0.5f) ? 25 : 30));
				num650 -= 5 * enrageScale;

				if (npc.ai[1] % num650 == (num650 - 1) && vectorCenter.Y < player.position.Y)
                {
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 42);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float projectileSpeed = revenge ? 6.5f : 6f;
						projectileSpeed += 7 * enrageScale;

						if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            projectileSpeed += 10f;

						if (CalamityWorld.bossRushActive)
                            projectileSpeed *= 1.5f;

                        float num1071 = player.position.X + player.width * 0.5f - vector121.X;
                        float num1072 = player.position.Y + player.height * 0.5f - vector121.Y;
                        float num1073 = (float)Math.Sqrt(num1071 * num1071 + num1072 * num1072);
                        num1073 = projectileSpeed / num1073;
                        num1071 *= num1073;
                        num1072 *= num1073;

                        int num1074 = 40;
                        int num1075 = Main.rand.NextBool(2) ? ModContent.ProjectileType<PlagueStingerGoliath>() : ModContent.ProjectileType<PlagueStingerGoliathV2>();
                        if (expertMode)
                        {
                            num1074 = 32;
                            int damageBoost = death ? 5 : (int)(6f * (1f - lifeRatio));
                            num1074 += damageBoost;

                            if (Main.rand.NextBool(6))
                            {
                                num1074 += 8;
                                num1075 = ModContent.ProjectileType<HiveBombGoliath>();
                            }
                        }
                        else
                        {
                            if (Main.rand.NextBool(9))
                            {
                                num1074 = 50;
                                num1075 = ModContent.ProjectileType<HiveBombGoliath>();
                            }
                        }
                        Projectile.NewProjectile(vector121.X, vector121.Y, num1071, num1072, num1075, num1074, 0f, Main.myPlayer, 0f, player.position.Y);
                    }
                }

                Movement(100f, 400f, 500f, player, enrageScale);

                float playerLocation = vectorCenter.X - player.Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

				if (npc.ai[1] > num650 * 10f)
				{
                    npc.ai[0] = -1f;
                    npc.ai[1] = 3f;
                    npc.netUpdate = true;
                }
            }

            // Missile charge
            else if (npc.ai[0] == 4f)
            {
				float num1044 = revenge ? 28f : 26f;
				if (CalamityWorld.bossRushActive)
					num1044 = 32f;

				num1044 += 6f * enrageScale;

				int num1043 = 2 + enrageScale;
                if (npc.ai[1] > (2 * num1043) && npc.ai[1] % 2f == 0f)
                {
                    MissileCountdown = 0;
                    npc.ai[0] = -1f;
                    npc.ai[1] = -1f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                    return;
                }

                // Charge
                if (npc.ai[1] % 2f == 0f)
                {
                    npc.TargetClosest(true);

                    float playerLocation = vectorCenter.X - player.Center.X;

					float num620 = 20f;
					num620 += 20 * enrageScale;

					if (Math.Abs(vectorCenter.Y - (player.Center.Y - 500f)) < num620)
                    {
                        if (MissileCountdown == 1)
                        {
                            Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 116);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int speed = revenge ? 6 : 5;
								speed += 7 * enrageScale;

								if (CalamityWorld.bossRushActive)
                                    speed = 12;

                                int damage = expertMode ? 48 : 60;

                                Vector2 baseVelocity = player.Center - vectorCenter;
                                baseVelocity.Normalize();
                                baseVelocity *= speed;

                                for (int i = 0; i < MissileProjectiles; i++)
                                {
                                    Vector2 spawn = vectorCenter;
                                    spawn.X += i * 27 - (MissileProjectiles * 12); // -96 to 93
                                    Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-MissileAngleSpread / 2 + (MissileAngleSpread * i / MissileProjectiles)));
                                    Projectile.NewProjectile(spawn.X, spawn.Y, velocity.X, velocity.Y, ModContent.ProjectileType<HiveBombGoliath>(), damage, 0f, Main.myPlayer, 0f, player.position.Y);
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

						return;
                    }

					npc.rotation = npc.velocity.X * 0.02f;
					charging = false;

                    float num1048 = revenge ? 16f : 14f;
                    float num1049 = revenge ? 0.2f : 0.18f;
					num1048 += 3 * enrageScale;
					num1049 += 0.5f * enrageScale;
					if (CalamityWorld.bossRushActive)
                    {
                        num1048 *= 1.5f;
                        num1049 *= 1.5f;
                    }

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

					npc.netUpdate = true;

					if (npc.netSpam > 10)
						npc.netSpam = 10;
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
						npc.velocity *= 0.5f;

					if (npc.ai[2] != 1f)
                    {
                        charging = true;

                        // Velocity fix if PBG slowed
                        if (npc.velocity.Length() < num1044)
							npc.velocity.X = num1044 * npc.direction;

						npc.Calamity().newAI[0] += 1f;
						if (npc.Calamity().newAI[0] > 90f)
							npc.velocity.X *= 1.01f;

						return;
                    }

                    npc.TargetClosest(true);

                    npc.spriteDirection = npc.direction;

					npc.rotation = npc.velocity.X * 0.02f;
					charging = false;

                    npc.velocity *= 0.9f;
                    float num1052 = revenge ? 0.12f : 0.1f;
                    if (lifeRatio < 0.5f || death)
                    {
                        npc.velocity *= 0.9f;
                        num1052 += 0.05f;
                    }
                    if (lifeRatio < 0.3f || death)
                    {
                        npc.velocity *= 0.9f;
                        num1052 += 0.05f;
                    }
                    if (lifeRatio < 0.1f || death)
                    {
                        npc.velocity *= 0.9f;
                        num1052 += 0.05f;
                    }
					if (enrageScale > 0)
						npc.velocity *= 0.7f;

					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num1052)
                    {
                        npc.ai[2] = 0f;
                        npc.ai[1] += 1f;
						npc.Calamity().newAI[0] = 0f;
                    }

					npc.netUpdate = true;

					if (npc.netSpam > 10)
						npc.netSpam = 10;
				}
            }

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
            }
        }

        private void Movement(float xPos, float yPos, float yPos2, Player player, int enrageScale)
        {
			Vector2 acceleration = new Vector2(CalamityWorld.bossRushActive ? 0.15f : 0.1f, CalamityWorld.bossRushActive ? 0.2f : 0.15f);
			Vector2 velocity = new Vector2(8f, 5f);
			float deceleration = 0.98f;

			acceleration *= 0.2f * enrageScale + 1f;
			velocity *= 0.2f * enrageScale + 1f;
			deceleration *= 1f - enrageScale * 0.2f;

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
                npc.width = 100;
                npc.height = 100;
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
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<PlaguebringerGoliathTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgePlaguebringerGoliath>(), true, !CalamityWorld.downedPlaguebringer);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedPlaguebringer, 4, 2, 1);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.WitchDoctor }, CalamityWorld.downedPlaguebringer);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<PlagueCellCluster>(), 10, 14);
                DropHelper.DropItemSpray(npc, ModContent.ItemType<InfectedArmorPlating>(), 13, 17);
                DropHelper.DropItemSpray(npc, ItemID.Stinger, 3, 5);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<VirulentKatana>(), 4); // Virulence
                DropHelper.DropItemChance(npc, ModContent.ItemType<DiseasedPike>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<ThePlaguebringer>(), 4); // Pandemic
                DropHelper.DropItemChance(npc, ModContent.ItemType<Malevolence>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<PestilentDefiler>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<TheHive>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<MepheticSprayer>(), 4); // Blight Spewer
                DropHelper.DropItemChance(npc, ModContent.ItemType<PlagueStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<TheSyringe>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<FuelCellBundle>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<InfectedRemote>(), 4);

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<BloomStone>(), 10);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
                DropHelper.DropItemChance(npc, ModContent.ItemType<PlagueCaller>(), 10);
            }

            // Mark PBG as dead
            CalamityWorld.downedPlaguebringer = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 300, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 180, true);
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
            }
        }
    }
}
