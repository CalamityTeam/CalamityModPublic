using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Cryogen
{
    [AutoloadBossHead]
    public class Cryogen : ModNPC
    {
        private int time = 0;
        private int iceShard = 0;
        private int currentPhase = 1;
        private int teleportLocationX = 0;

        public override string Texture => "CalamityMod/NPCs/Cryogen/Cryogen_Phase1";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryogen");
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 24f;
            npc.damage = 50;
            npc.width = 86;
            npc.height = 88;
            npc.defense = 12;
			npc.DR_NERD(0.1f);
            npc.LifeMaxNERB(17900, 26300, 3000000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 12, 0, 0);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[ModContent.BuffType<MarkedforDeath>()] = false;
            npc.buffImmune[BuffID.OnFire] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[BuffID.BetsysCurse] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.DryadsWardDebuff] = false;
			npc.buffImmune[BuffID.Oiled] = false;
			npc.buffImmune[BuffID.BoneJavelin] = false;
			npc.buffImmune[BuffID.ShadowFlame] = false;
			npc.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Plague>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WarCleave>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = false;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath15;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Cryogen");
            else
                music = MusicID.FrostMoon;
            bossBag = ModContent.ItemType<CryogenBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(time);
            writer.Write(iceShard);
            writer.Write(teleportLocationX);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            time = reader.ReadInt32();
            iceShard = reader.ReadInt32();
            teleportLocationX = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0f, 1f, 1f);

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

			bool isChill = player.ZoneSnow;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases
			bool phase2 = lifeRatio < (death ? 0.95 : revenge ? 0.89 : 0.83);
			bool phase3 = lifeRatio < (death ? 0.8 : revenge ? 0.73 : 0.66);
			bool phase4 = lifeRatio < (death ? 0.7 : revenge ? 0.6 : 0.49);
			bool phase5 = lifeRatio < (death ? 0.55 : revenge ? 0.43 : 0.32);
			bool phase6 = lifeRatio < (death ? 0.4 : revenge ? 0.27 : 0.15);
			bool phase7 = lifeRatio < (death ? 0.25 : 0.15) && revenge;
			bool phase8 = lifeRatio < (revenge ? 0.225 : 0.1);
			bool phase9 = lifeRatio < (revenge ? 0.2 : 0.05) || death;

			if ((int)npc.ai[0] + 1 > currentPhase && currentPhase < 6)
            {
                HandlePhaseTransition((int)npc.ai[0] + 1);
            }

			if (npc.ai[2] == 0f && npc.localAI[1] == 0f && Main.netMode != NetmodeID.MultiplayerClient && (npc.ai[0] < 4f || BossRushEvent.BossRushActive)) //spawn shield for phase 0 1 2 3, not 4 5 6
            {
                int num6 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CryogenIce>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                npc.ai[2] = num6 + 1;
                npc.localAI[1] = -1f;
                npc.netUpdate = true;
                Main.npc[num6].ai[0] = npc.whoAmI;
                Main.npc[num6].netUpdate = true;
            }

            int num7 = (int)npc.ai[2] - 1;
            if (num7 != -1 && Main.npc[num7].active && Main.npc[num7].type == ModContent.NPCType<CryogenIce>())
            {
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.dontTakeDamage = false;
                npc.ai[2] = 0f;
                if (npc.localAI[1] == -1f)
                {
                    npc.localAI[1] = expertMode ? 720f : 1080f;
                }
                if (npc.localAI[1] > 0f)
                {
                    npc.localAI[1] -= 1f;
                }
            }

            if (npc.ai[0] != 5f)
            {
                npc.rotation = npc.velocity.X * 0.1f;
            }

			CalamityMod.StopRain();

			if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

					if (npc.ai[1] != 0f)
					{
						npc.ai[1] = 0f;
						teleportLocationX = 0;
						iceShard = 0;
						npc.netUpdate = true;
					}
					return;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            if (Main.netMode != NetmodeID.MultiplayerClient && expertMode && (npc.ai[0] < 5f || !phase7))
            {
                time++;
                if (time >= 600)
                {
					int totalProjectiles = 4;
					float radians = MathHelper.TwoPi / totalProjectiles;
					int damage = 25;
					float velocity = BossRushEvent.BossRushActive ? 12f : 4f;
					Vector2 spinningPoint = Main.rand.NextBool(2) ? new Vector2(0f, -velocity) : Vector2.Normalize(new Vector2(-velocity, -velocity)) * velocity;
					for (int k = 0; k < totalProjectiles; k++)
					{
						Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
						Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<IceBomb>(), damage, 0f, Main.myPlayer, 0f, 0f);
					}
                    time = 0;
                }
            }

            if (npc.ai[0] == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 120f)
                    {
                        npc.localAI[0] = 0f;
                        npc.netUpdate = true;
                        if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
							int totalProjectiles = 16;
							float radians = MathHelper.TwoPi / totalProjectiles;
							int damage = expertMode ? 20 : 23;
							float velocity = BossRushEvent.BossRushActive ? 12f : 8f;
							for (int k = 0; k < totalProjectiles; k++)
							{
								Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * k);
								Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<IceBlast>(), damage, 0f, Main.myPlayer, 0f, 0f);
							}
                        }
                    }
                }

                Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
                float num1243 = player.Center.X - vector142.X;
                float num1244 = player.Center.Y - vector142.Y;
                float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);
                float num1246 = isChill ? 4f : 8f;
                if (death)
                {
                    num1246 = isChill ? 5f : 10f;
                }
                if (BossRushEvent.BossRushActive)
                {
                    num1246 = 14f;
                }
                num1245 = num1246 / num1245;
                num1243 *= num1245;
                num1244 *= num1245;
                npc.velocity.X = (npc.velocity.X * 50f + num1243) / 51f;
                npc.velocity.Y = (npc.velocity.Y * 50f + num1244) / 51f;

                if (phase2)
                {
					npc.TargetClosest(true);
					npc.ai[0] = 1f;
                    npc.localAI[0] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 120f)
                    {
                        npc.localAI[0] = 0f;
                        if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
							int totalProjectiles = 12;
							float radians = MathHelper.TwoPi / totalProjectiles;
							int damage = expertMode ? 20 : 23;
							float velocity2 = BossRushEvent.BossRushActive ? 12f : 8f;
							for (int k = 0; k < totalProjectiles; k++)
							{
								Vector2 vector255 = new Vector2(0f, -velocity2).RotatedBy(radians * k);
								Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<IceBlast>(), damage, 0f, Main.myPlayer, 0f, 0f);
							}
                        }
                    }
                }

                float velocity = isChill ? 7.5f : 4f;
                float acceleration = isChill ? 0.1f : 0.15f;
                if (death)
                    velocity = isChill ? 7f : 3.5f;
				if (BossRushEvent.BossRushActive)
				{
					velocity = 3f;
					acceleration *= 1.5f;
				}

				if (npc.position.Y > player.position.Y - 375f)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y -= acceleration;

					if (npc.velocity.Y > velocity)
						npc.velocity.Y = velocity;
				}
				else if (npc.position.Y < player.position.Y - 425f)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y += acceleration;

					if (npc.velocity.Y < -velocity)
						npc.velocity.Y = -velocity;
				}

				if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 300f)
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X *= 0.98f;

					npc.velocity.X -= acceleration;

					if (npc.velocity.X > velocity)
						npc.velocity.X = velocity;
				}
				if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 300f)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X *= 0.98f;

					npc.velocity.X += acceleration;

					if (npc.velocity.X < -velocity)
						npc.velocity.X = -velocity;
				}

				if (phase3)
                {
					npc.TargetClosest(true);
					npc.ai[0] = 2f;
                    npc.localAI[0] = 0f;
                    iceShard = 0;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 120f)
                    {
                        npc.localAI[0] = 0f;
                        npc.netUpdate = true;
                        if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
                            if (Main.rand.NextBool(2))
                            {
								int totalProjectiles = 12;
								float radians = MathHelper.TwoPi / totalProjectiles;
								int damage = expertMode ? 20 : 23;
								float velocity = BossRushEvent.BossRushActive ? 14f : 9f;
								for (int k = 0; k < totalProjectiles; k++)
								{
									Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * k);
									Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<IceBlast>(), damage, 0f, Main.myPlayer, 0f, 0f);
								}
                            }
                            else
                            {
                                float num179 = revenge ? 9f : 7f;
                                if (BossRushEvent.BossRushActive)
                                    num179 = 14f;

                                Vector2 value9 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                                float num180 = player.position.X + player.width * 0.5f - value9.X;
                                float num181 = Math.Abs(num180) * 0.1f;
                                float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
                                float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                                num183 = num179 / num183;
                                num180 *= num183;
                                num182 *= num183;
                                int num184 = expertMode ? 23 : 26;
                                int num185 = ModContent.ProjectileType<IceRain>();
                                value9.X += num180;
                                value9.Y += num182;
                                for (int num186 = 0; num186 < 6; num186++)
                                {
                                    num180 = player.position.X + player.width * 0.5f - value9.X;
                                    num182 = player.position.Y + player.height * 0.5f - value9.Y;
                                    num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                                    num183 = num179 / num183;
                                    num180 += Main.rand.Next(-360, 361);
                                    num182 += Main.rand.Next(-360, 361);
                                    num180 *= num183;
                                    Projectile.NewProjectile(value9.X, value9.Y, num180, -8f, num185, num184, 0f, Main.myPlayer, 0f, 0f);
                                }
                            }
                        }
                    }
                }

                Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
                float num1243 = player.Center.X - vector142.X;
                float num1244 = player.Center.Y - vector142.Y;
                float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);
                float num1246 = isChill ? 6f : 12f;
                if (death)
                {
                    num1246 = isChill ? 7f : 14f;
                }
                if (BossRushEvent.BossRushActive)
                {
                    num1246 = 20f;
                }
                num1245 = num1246 / num1245;
                num1243 *= num1245;
                num1244 *= num1245;
                npc.velocity.X = (npc.velocity.X * 50f + num1243) / 51f;
                npc.velocity.Y = (npc.velocity.Y * 50f + num1244) / 51f;

				if (phase4)
                {
					npc.TargetClosest(true);
					npc.ai[0] = 3f;
                    npc.localAI[0] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 120f)
                    {
                        npc.localAI[0] = 0f;
                        if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
                            float num179 = revenge ? 9f : 7f;
                            if (BossRushEvent.BossRushActive)
                                num179 = 14f;

                            Vector2 value9 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                            float num180 = player.position.X + player.width * 0.5f - value9.X;
                            float num181 = Math.Abs(num180) * 0.1f;
                            float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
                            float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                            npc.netUpdate = true;
                            num183 = num179 / num183;
                            num180 *= num183;
                            num182 *= num183;
                            int num184 = expertMode ? 23 : 26;
                            int num185 = ModContent.ProjectileType<IceRain>();
                            value9.X += num180;
                            value9.Y += num182;
                            for (int num186 = 0; num186 < 12; num186++)
                            {
                                num180 = player.position.X + player.width * 0.5f - value9.X;
                                num182 = player.position.Y + player.height * 0.5f - value9.Y;
                                num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                                num183 = num179 / num183;
                                num180 += Main.rand.Next(-360, 361);
                                num182 += Main.rand.Next(-360, 361);
                                num180 *= num183;
                                Projectile.NewProjectile(value9.X, value9.Y, num180, -2.5f, num185, num184, 0f, Main.myPlayer, 1f, 0f);
                            }
                        }
                    }
                }

				float velocity = isChill ? 7.5f : 4f;
				float acceleration = isChill ? 0.1f : 0.15f;
				if (death)
					velocity = isChill ? 7f : 3.5f;
				if (BossRushEvent.BossRushActive)
				{
					velocity = 3f;
					acceleration *= 1.5f;
				}

				if (npc.position.Y > player.position.Y - 375f)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y -= acceleration;

					if (npc.velocity.Y > velocity)
						npc.velocity.Y = velocity;
				}
				else if (npc.position.Y < player.position.Y - 425f)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y += acceleration;

					if (npc.velocity.Y < -velocity)
						npc.velocity.Y = -velocity;
				}

				if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 300f)
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X *= 0.98f;

					npc.velocity.X -= acceleration;

					if (npc.velocity.X > velocity)
						npc.velocity.X = velocity;
				}
				if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 300f)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X *= 0.98f;

					npc.velocity.X += acceleration;

					if (npc.velocity.X < -velocity)
						npc.velocity.X = -velocity;
				}

				if (phase5)
                {
					npc.TargetClosest(true);
					npc.ai[0] = 4f;
                    npc.localAI[0] = 0f;
                    iceShard = 0;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 4f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 60f && npc.alpha == 0)
                    {
                        npc.localAI[0] = 0f;
                        if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
							int totalProjectiles = 12;
							float radians = MathHelper.TwoPi / totalProjectiles;
							int damage = expertMode ? 20 : 23;
							float velocity = BossRushEvent.BossRushActive ? 14f : 9f;
							for (int k = 0; k < totalProjectiles; k++)
							{
								Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * k);
								Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<IceBlast>(), damage, 0f, Main.myPlayer, 0f, 0f);
							}
                        }
                    }
                }

                Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
                float num1243 = player.Center.X - vector142.X;
                float num1244 = player.Center.Y - vector142.Y;
                float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);
                float speed = revenge ? 5.5f : 5f;
                if (BossRushEvent.BossRushActive)
                {
                    speed = 10f;
                }
                num1245 = speed / num1245;
                num1243 *= num1245;
                num1244 *= num1245;
                npc.velocity.X = (npc.velocity.X * 50f + num1243) / 51f;
                npc.velocity.Y = (npc.velocity.Y * 50f + num1244) / 51f;

                if (npc.ai[1] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[2] += 1f;
                        if (npc.localAI[2] >= (BossRushEvent.BossRushActive ? 90f : 180f))
                        {
							npc.TargetClosest(true);
							npc.localAI[2] = 0f;
                            int num1249 = 0;
                            int num1250;
                            int num1251;
                            while (true)
                            {
                                num1249++;
                                num1250 = (int)player.Center.X / 16;
                                num1251 = (int)player.Center.Y / 16;

                                int min = 16;
                                int max = 20;

                                if (Main.rand.NextBool(2))
                                    num1250 += Main.rand.Next(min, max);
                                else
                                    num1250 -= Main.rand.Next(min, max);

                                if (Main.rand.NextBool(2))
                                    num1251 += Main.rand.Next(min, max);
                                else
                                    num1251 -= Main.rand.Next(min, max);

                                if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2(num1250 * 16, num1251 * 16), 1, 1, player.position, player.width, player.height))
                                    break;

                                if (num1249 > 100)
                                    goto Block;
                            }
                            npc.ai[1] = 1f;
                            teleportLocationX = num1250;
                            iceShard = num1251;
                            npc.netUpdate = true;
                            Block:
                            ;
                        }
                    }
                }
                else if (npc.ai[1] == 1f)
                {
                    Vector2 position = new Vector2(teleportLocationX * 16f - (npc.width / 2), iceShard * 16f - (npc.height / 2));
                    for (int m = 0; m < 5; m++)
                    {
                        int dust = Dust.NewDust(position, npc.width, npc.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                    }

                    npc.alpha += 2;
                    if (npc.alpha >= 255)
                    {
                        Main.PlaySound(SoundID.Item8, npc.Center);
                        npc.alpha = 255;
                        npc.position = position;

                        for (int n = 0; n < 15; n++)
                        {
                            int num39 = Dust.NewDust(npc.position, npc.width, npc.height, 67, 0f, 0f, 100, default, 3f);
                            Main.dust[num39].noGravity = true;
                        }

                        npc.ai[1] = 2f;
                        npc.netUpdate = true;
                    }
                }
                else if (npc.ai[1] == 2f)
                {
                    npc.alpha -= 50;
                    if (npc.alpha <= 0)
                    {
                        npc.alpha = 0;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }
                }

				if (phase6)
                {
                    for (int num621 = 0; num621 < 40; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 70; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }

                    float randomSpread = Main.rand.Next(-200, 200) / 100;
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore1"), 1f);
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore2"), 1f);
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore3"), 1f);

					npc.TargetClosest(true);
					npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.localAI[0] = 0f;
                    npc.localAI[2] = 0f;
                    teleportLocationX = 0;
                    iceShard = 0;
                    npc.netUpdate = true;

					int chance = 100;
					if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
					{
						chance = 20;
					}
					if (Main.rand.NextBool(chance))
					{
						string key = "Mods.CalamityMod.CryogenBossText";
						Color messageColor = Color.Cyan;
						if (Main.netMode == NetmodeID.SinglePlayer)
						{
							Main.NewText(Language.GetTextValue(key), messageColor);
						}
						else if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
						}
					}
                }
            }
			else if (npc.ai[0] == 5f)
			{
                npc.damage = (int)(npc.defDamage * 1.5f);

				if (phase7)
				{
					if (npc.ai[1] == 60f)
						npc.velocity = Vector2.Normalize(player.Center - npc.Center) * (BossRushEvent.BossRushActive ? 30f : (isChill ? 18f : 24f));

					npc.ai[1] -= 1f;
					if (npc.ai[1] <= 0f)
					{
						npc.ai[3] += 1f;
						if (npc.ai[3] > 2f)
						{
							npc.TargetClosest(true);
							npc.damage = 0;
							npc.defense = npc.defDefense + 8;
							npc.ai[0] = 6f;
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
							time = 0;
						}
						else
							npc.ai[1] = 60f;

						npc.rotation = npc.velocity.X * 0.1f;
					}
					else if (npc.ai[1] <= 15f)
					{
						npc.velocity *= 0.95f;
						npc.rotation = npc.velocity.X * 0.15f;
					}
					else
						npc.rotation += npc.direction * 0.5f;

					return;
				}

				float num1372 = isChill ? 16f : 24f;
				if (BossRushEvent.BossRushActive)
                {
                    num1372 = 32f;
                }
                Vector2 vector167 = new Vector2(npc.Center.X + (npc.direction * 20), npc.Center.Y + 6f);
                float num1373 = player.position.X + player.width * 0.5f - vector167.X;
                float num1374 = player.Center.Y - vector167.Y;
                float num1375 = (float)Math.Sqrt(num1373 * num1373 + num1374 * num1374);
                float num1376 = num1372 / num1375;
                num1373 *= num1376;
                num1374 *= num1376;
                iceShard--;

				if (phase9)
                {
                    if (num1375 < 170f || iceShard > 0)
                    {
                        if (num1375 < 170f)
                        {
                            iceShard = 17;
                        }
                        npc.rotation += npc.direction * 0.5f;
                        return;
                    }
                }
                else if (phase8)
                {
                    if (num1375 < 190f || iceShard > 0)
                    {
                        if (num1375 < 190f)
                        {
                            iceShard = 19;
                        }
                        npc.rotation += npc.direction * 0.35f;
                        return;
                    }
                }
                else
                {
                    if (num1375 < 200f || iceShard > 0)
                    {
                        if (num1375 < 200f)
                        {
                            iceShard = 20;
                        }
                        npc.rotation += npc.direction * 0.3f;
                        return;
                    }
                }

                npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
                npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
                if (num1375 < 350f)
                {
                    npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
                    npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
                }
                if (num1375 < 300f)
                {
                    npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
                    npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
                }
                npc.rotation = npc.velocity.X * 0.15f;
            }
			else
			{
				time++;
				if (time >= 75)
				{
					int totalProjectiles = 4;
					float radians = MathHelper.TwoPi / totalProjectiles;
					int damage = 25;
					float velocity2 = BossRushEvent.BossRushActive ? 16f : 6f;
					Vector2 spinningPoint = Main.rand.NextBool(2) ? new Vector2(0f, -velocity2) : Vector2.Normalize(new Vector2(-velocity2, -velocity2)) * velocity2;
					for (int k = 0; k < totalProjectiles; k++)
					{
						Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
						Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<IceBomb>(), damage, 0f, Main.myPlayer, 0f, 0f);
					}
					time = 0;
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 180f)
				{
					npc.TargetClosest(true);
					npc.ai[0] = 5f;
					npc.ai[1] = 60f;
					time = 0;
					iceShard = 0;
					npc.netUpdate = true;
				}

				float velocity = isChill ? 6f : 3f;
				float acceleration = isChill ? 0.2f : 0.3f;
				if (death)
					velocity = isChill ? 5f : 2.5f;
				if (BossRushEvent.BossRushActive)
				{
					velocity = 2f;
					acceleration *= 1.5f;
				}

				if (npc.position.Y > player.position.Y - 375f)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y -= acceleration;

					if (npc.velocity.Y > velocity)
						npc.velocity.Y = velocity;
				}
				else if (npc.position.Y < player.position.Y - 400f)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y += acceleration;

					if (npc.velocity.Y < -velocity)
						npc.velocity.Y = -velocity;
				}

				if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 350f)
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X *= 0.98f;

					npc.velocity.X -= acceleration;

					if (npc.velocity.X > velocity)
						npc.velocity.X = velocity;
				}
				if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 350f)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X *= 0.98f;

					npc.velocity.X += acceleration;

					if (npc.velocity.X < -velocity)
						npc.velocity.X = -velocity;
				}
			}

			if (!phase7)
			{
				if (npc.ai[3] == 0f && npc.life > 0)
				{
					npc.ai[3] = npc.lifeMax;
				}
				if (npc.life > 0)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int num660 = (int)(npc.lifeMax * 0.075);
						if ((npc.life + num660) < npc.ai[3])
						{
							npc.ai[3] = npc.life;
							for (int num662 = 0; num662 < 2; num662++)
							{
								int x = (int)(npc.position.X + Main.rand.Next(npc.width - 32));
								int y = (int)(npc.position.Y + Main.rand.Next(npc.height - 32));
								int random = 1;
								switch ((int)npc.ai[0])
								{
									case 0:
									case 1:
										break;
									case 2:
									case 3:
										random = 2;
										break;
									case 4:
									case 5:
										random = 3;
										break;
									default:
										break;
								}
								int randomSpawn = Main.rand.Next(random);
								if (randomSpawn == 0)
								{
									randomSpawn = ModContent.NPCType<Cryocore>();
								}
								else if (randomSpawn == 1)
								{
									randomSpawn = ModContent.NPCType<IceMass>();
								}
								else
								{
									randomSpawn = ModContent.NPCType<Cryocore2>();
								}
								int num664 = NPC.NewNPC(x, y, randomSpawn, 0, 0f, 0f, 0f, 0f, 255);
								if (Main.netMode == NetmodeID.Server && num664 < 200)
								{
									NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
								}
							}
						}
					}
				}
			}
        }

        private void HandlePhaseTransition(int newPhase)
        {
            int chipGoreAmount = newPhase >= 5 ? 3 : newPhase >= 3 ? 2 : 1;
            for (int i = 1; i < chipGoreAmount; i++)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CryoChipGore" + i), 1f);
            }
            currentPhase = newPhase;
            Main.PlaySound(SoundID.NPCDeath14, (int)npc.position.X, (int)npc.position.Y);
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) //for alt textures
        {
            if (currentPhase > 1)
            {
                string phase = "CalamityMod/NPCs/Cryogen/Cryogen_Phase" + currentPhase;
                Texture2D texture = ModContent.GetTexture(phase);
                CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor);
                return false;
            }
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.9f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                float randomSpread = Main.rand.Next(-200, 200) / 100;
                for (int i = 1; i < 4; i++)
                {
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoDeathGore" + i), 1f);
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoChipGore" + i), 1f);
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

            DropHelper.DropItemChance(npc, ModContent.ItemType<CryogenTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeCryogen>(), true, !CalamityWorld.downedCryogen);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedCryogen, 4, 2, 1);

            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<CryoBar>(), 15, 25);
                DropHelper.DropItemSpray(npc, ModContent.ItemType<EssenceofEleum>(), 4, 8);
                DropHelper.DropItem(npc, ItemID.FrostCore);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<Avalanche>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<GlacialCrusher>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<EffluviumBow>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<BittercoldStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<SnowstormStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Icebreaker>(), 4);

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<CryoStone>(), 10);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Regenator>(), DropHelper.RareVariantDropRateInt);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<CryogenMask>(), 7);

                // Other
                DropHelper.DropItemChance(npc, ItemID.FrozenKey, 5);
            }

            // Spawn Permafrost if he isn't in the world
            int permafrostNPC = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (permafrostNPC == -1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DILF>(), 0, 0f, 0f, 0f, 0f, 255);
            }

            // If Cryogen has not been killed, notify players about Cryonic Ore
            if (!CalamityWorld.downedCryogen)
            {
                string key = "Mods.CalamityMod.IceOreText";
                Color messageColor = Color.LightSkyBlue;
                WorldGenerationMethods.SpawnOre(ModContent.TileType<CryonicOre>(), 15E-05, .45f, .65f);

                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(key), messageColor);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }

            // Mark Cryogen as dead
            CalamityWorld.downedCryogen = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 120, true);
            player.AddBuff(BuffID.Chilled, 90, true);
        }
    }
}
