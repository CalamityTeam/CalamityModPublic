using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Crabulon
{
	[AutoloadBossHead]
    public class CrabulonIdle : ModNPC
    {
        private int shotSpacing = 1000;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crabulon");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 14f;
			npc.GetNPCDamage();
			npc.width = 280;
            npc.height = 160;
            npc.defense = 8;
            npc.LifeMaxNERB(3000, 4000, 11000000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = true;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Crabulon");
            else
                music = MusicID.Boss4;
            npc.boss = true;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 4, 0, 0);
            npc.HitSound = SoundID.NPCHit45;
            npc.DeathSound = SoundID.NPCDeath1;
            bossBag = ModContent.ItemType<CrabulonBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(npc.localAI[0]);
			writer.Write(shotSpacing);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			npc.localAI[0] = reader.ReadSingle();
			shotSpacing = reader.ReadInt32();
        }

        public override void AI()
        {
			npc.gfxOffY = -16;

			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0f, 0.3f, 0.7f);
			
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			npc.spriteDirection = npc.direction;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases
			bool phase2 = lifeRatio < 0.66f && expertMode;
			bool phase3 = lifeRatio < 0.33f && expertMode;

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];
			if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.noTileCollide = true;

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
					npc.velocity.Y += 0.1f;
					if (npc.velocity.Y > 12f)
						npc.velocity.Y = 12f;

					if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

					if (npc.ai[0] != 1f)
					{
						npc.ai[0] = 1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						shotSpacing = 1000;
						npc.netUpdate = true;
					}
					return;
                }
            }
            else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			float enrageScale = 0f;
			if ((npc.position.Y / 16f) < Main.worldSurface)
				enrageScale += 1f;
			if (!player.ZoneGlowshroom)
				enrageScale += 1f;

			if (BossRushEvent.BossRushActive)
				enrageScale = 0f;

			if (npc.ai[0] != 0f && npc.ai[0] < 3f)
            {
                Vector2 vector34 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num349 = player.position.X + (player.width / 2) - vector34.X;
                float num350 = player.position.Y + (player.height / 2) - vector34.Y;
                float num351 = (float)Math.Sqrt(num349 * num349 + num350 * num350);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num352 = 1;
                    npc.localAI[3] += 2f;
                    if (BossRushEvent.BossRushActive)
                    {
                        npc.localAI[3] += 2f;
                        num352 += 3;
                    }
                    if (phase2)
                    {
                        npc.localAI[3] += 1f;
                        num352 += 2;
                    }
                    if (phase3)
                    {
                        npc.localAI[3] += 2f;
                        num352 += 3;
                    }
                    if (npc.ai[3] == 0f)
                    {
                        if (npc.localAI[3] > 600f)
                        {
                            npc.ai[3] = 1f;
                            npc.localAI[3] = 0f;
                        }
                    }
                    else if (npc.localAI[3] > 45f)
                    {
                        npc.localAI[3] = 0f;
                        npc.ai[3] += 1f;
                        if (npc.ai[3] >= num352)
                        {
                            npc.ai[3] = 0f;
                        }
                        float num353 = 10f;
                        int type = ModContent.ProjectileType<MushBomb>();
                        Main.PlaySound(SoundID.Item42, (int)npc.position.X, (int)npc.position.Y);
                        if (BossRushEvent.BossRushActive)
                        {
                            num353 += 3f;
                        }
                        if (phase2)
                        {
                            num353 += 1f;
                        }
                        if (phase3)
                        {
                            num353 += 1f;
                        }
                        vector34 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        num349 = player.position.X + player.width * 0.5f - vector34.X;
                        num350 = player.position.Y + player.height * 0.5f - vector34.Y;
                        num351 = (float)Math.Sqrt(num349 * num349 + num350 * num350);
                        num351 = num353 / num351;
                        num349 *= num351;
                        num350 *= num351;
                        vector34.X += num349;
                        vector34.Y += num350;
                        Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350 - 5f, type, npc.GetProjectileDamage(type), 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
            if (npc.ai[0] == 0f)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!player.dead && player.active && (player.Center - npc.Center).Length() < 800f)
                    {
                        player.AddBuff(ModContent.BuffType<Mushy>(), 2);
                    }
                }
                int sporeDust = Dust.NewDust(npc.position, npc.width, npc.height, 56, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), 1.2f);
                Main.dust[sporeDust].noGravity = true;
                Main.dust[sporeDust].velocity *= 0.5f;
                npc.ai[1] += 1f;
                if (npc.justHit || npc.ai[1] >= 420f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.velocity *= 0.98f;
                npc.ai[1] += 1f;
                if (npc.ai[1] >= (death ? 5f : revenge ? 30f : 60f))
                {
					npc.TargetClosest(true);
					npc.noGravity = true;
                    npc.noTileCollide = true;
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                float num823 = 1f;
                if (phase2)
                    num823 = 1.25f;
                if (phase3)
                    num823 = 1.75f;
				if (death)
					num823 += 2f * (1f - lifeRatio);
                if (BossRushEvent.BossRushActive)
                    num823 = 12f;
                if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && BossRushEvent.BossRushActive))
                    num823 = 16f;
				num823 += 2f * enrageScale;

				bool flag51 = false;
				if (Math.Abs(npc.Center.X - player.Center.X) < 50f)
                    flag51 = true;

                if (flag51)
                {
                    npc.velocity.X *= 0.9f;
                    if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                else
                {
                    float playerLocation = npc.Center.X - player.Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;

                    if (npc.direction > 0)
                        npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;
                    if (npc.direction < 0)
                        npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
                }
                int num854 = 80;
                int num855 = 20;
                Vector2 position2 = new Vector2(npc.Center.X - (num854 / 2), npc.position.Y + npc.height - num855);
                bool flag52 = false;
                if (npc.position.X < player.position.X && npc.position.X + npc.width > player.position.X + player.width && npc.position.Y + npc.height < player.position.Y + player.height - 16f)
                {
                    flag52 = true;
                }
                if (flag52)
                {
                    npc.velocity.Y += 0.5f;
                }
                else if (Collision.SolidCollision(position2, num854, num855))
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = 0f;
                    }
                    if (npc.velocity.Y > -0.2)
                    {
                        npc.velocity.Y -= 0.025f;
                    }
                    else
                    {
                        npc.velocity.Y -= 0.2f;
                    }
                    if (npc.velocity.Y < -4f)
                    {
                        npc.velocity.Y = -4f;
                    }
                }
                else
                {
                    if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y = 0f;
                    }
                    if (npc.velocity.Y < 0.1)
                    {
                        npc.velocity.Y += 0.025f;
                    }
                    else
                    {
                        npc.velocity.Y += 0.5f;
                    }
                }
                npc.ai[1] += 1f;
                if (npc.ai[1] >= (360f - (death ? 120f * (1f - lifeRatio) : 0f)))
                {
					npc.TargetClosest(true);
					npc.noGravity = false;
                    npc.noTileCollide = false;
                    npc.ai[0] = 3f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
                if (npc.velocity.Y > 10f)
                {
                    npc.velocity.Y = 10f;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.noTileCollide = false;
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X *= 0.8f;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 0f)
                    {
						if (revenge)
						{
							switch ((int)npc.ai[3])
							{
								case 0:
									break;
								case 1:
									npc.ai[1] += 1f;
									break;
								case 2:
									npc.ai[1] += 3f;
									break;
								case 3:
									npc.ai[1] += 6f;
									break;
								default:
									break;
							}
						}
                        if (phase2)
                        {
                            npc.ai[1] += !revenge ? 4f : 1f;
                        }
                        if (phase3)
                        {
                            npc.ai[1] += !revenge ? 4f : 1f;
                        }
                    }

                    if (npc.ai[1] >= 300f)
                    {
                        npc.ai[1] = -20f;
                    }
                    else if (npc.ai[1] == -1f)
                    {
						int velocityX = BossRushEvent.BossRushActive ? 12 : 4;
						float velocityY = BossRushEvent.BossRushActive ? -16f : -12f;

						float distanceBelowTarget = npc.position.Y - (player.position.Y + 80f);
						float speedMult = 1f;

						if (revenge)
						{
							switch ((int)npc.ai[3])
							{
								case 0: // Normal
									break;
								case 1: // High
									velocityY -= 4f;
									break;
								case 2: // Low
									velocityY += 4f;
									break;
								case 3: // Long and low
									velocityX += 4;
									velocityY += 4f;
									break;
								default:
									break;
							}

							if (distanceBelowTarget > 0f)
								speedMult += distanceBelowTarget * 0.001f;

							if (speedMult > 2f)
								speedMult = 2f;

							velocityY *= speedMult;
						}

						if (expertMode)
						{
							if (player.position.Y < npc.Bottom.Y)
								npc.velocity.Y = velocityY;
							else
								npc.velocity.Y = 1f;

							npc.noTileCollide = true;
						}
						else
							npc.velocity.Y = velocityY;

						npc.velocity.X = velocityX * npc.direction;

                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                    }
                }
            }
            else
            {
                if (npc.velocity.Y == 0f)
                {
                    Main.PlaySound(SoundID.Item14, npc.position);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile((int)npc.Center.X, (int)npc.Center.Y + 20, 0f, 0f, ModContent.ProjectileType<Mushmash>(), 20, 0f, Main.myPlayer, 0f, 0f);

					int type = ModContent.ProjectileType<MushBombFall>();
					int damage = npc.GetProjectileDamage(type);

					if (npc.ai[2] % 2f == 0f && phase2 && revenge)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							float velocityX = npc.ai[2] == 0f ? -4f : 4f;
							for (int x = 0; x < 20; x++)
							{
								Projectile.NewProjectile(npc.Center.X + shotSpacing, npc.Center.Y - 1000f, velocityX, 0f, type, damage, 0f, Main.myPlayer, 0f, 0f);
								shotSpacing -= 100;
							}
							shotSpacing = 1000;
						}
					}

					npc.TargetClosest(true);
					npc.ai[2] += 1f;
					if (npc.ai[2] >= (phase2 ? 4f : 3f))
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && revenge && !phase2)
                        {
                            for (int x = 0; x < 20; x++)
                            {
                                Projectile.NewProjectile(npc.Center.X + shotSpacing, npc.Center.Y - 1000f, 0f, 0f, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                shotSpacing -= 100;
                            }
                            shotSpacing = 1000;
                        }

                        npc.ai[0] = 1f;
                        npc.ai[2] = 0f;
						if (revenge)
							npc.ai[3] = 0f;
                    }
                    else
                    {
                        npc.ai[0] = 3f;
						if (revenge)
							npc.ai[3] += 1f;
					}

                    for (int num622 = (int)npc.position.X - 20; num622 < (int)npc.position.X + npc.width + 40; num622 += 20)
                    {
                        for (int num623 = 0; num623 < 4; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(npc.position.X - 20f, npc.position.Y + npc.height), npc.width + 20, 4, 56, 0f, 0f, 100, default, 1.5f);
                            Main.dust[num624].velocity *= 0.2f;
                        }
                    }
                }
                else
                {
					if (!player.dead && expertMode)
					{
						if ((player.position.Y > npc.Bottom.Y && npc.velocity.Y > 0f) || (player.position.Y < npc.Bottom.Y && npc.velocity.Y < 0f))
							npc.noTileCollide = true;
						else if ((npc.velocity.Y > 0f && npc.Bottom.Y > Main.player[npc.target].Top.Y) || (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height)))
							npc.noTileCollide = false;
					}

					if (npc.position.X < player.position.X && npc.position.X + npc.width > player.position.X + player.width)
                    {
                        npc.velocity.X *= 0.9f;
                        npc.velocity.Y += BossRushEvent.BossRushActive ? 0.3f : death ? 0.18f : 0.15f;
                    }
                    else
                    {
						float velocityX = 0.11f +
							(expertMode ? 0.02f : 0f) +
							(revenge ? 0.02f : 0f) +
							(death ? 0.02f : 0f);
						velocityX += 0.05f * enrageScale;

                        if (npc.direction < 0)
                            npc.velocity.X -= velocityX;
                        else if (npc.direction > 0)
                            npc.velocity.X += velocityX;

                        float num626 = BossRushEvent.BossRushActive ? 5f : 2.5f;
						num626 += enrageScale;
                        if (revenge)
                        {
                            num626 += 1f;
                        }
                        if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && BossRushEvent.BossRushActive))
                        {
                            num626 += 3f;
                        }
                        if (phase2)
                        {
                            num626 += 1f;
                        }
                        if (phase3)
                        {
                            num626 += 1f;
                        }
                        if (npc.velocity.X < -num626)
                        {
                            npc.velocity.X = -num626;
                        }
                        if (npc.velocity.X > num626)
                        {
                            npc.velocity.X = num626;
                        }
                    }
                }
            }

            if (npc.localAI[0] == 0f && npc.life > 0)
            {
                npc.localAI[0] = npc.lifeMax;
            }
            if (npc.life > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num660 = (int)(npc.lifeMax * 0.05);
                    if ((npc.life + num660) < npc.localAI[0])
                    {
                        npc.localAI[0] = npc.life;
                        int num661 = death ? 3 : expertMode ? Main.rand.Next(2, 4) : 2;
                        for (int num662 = 0; num662 < num661; num662++)
                        {
                            int x = (int)(npc.position.X + Main.rand.Next(npc.width - 32));
                            int y = (int)(npc.position.Y + Main.rand.Next(npc.height - 32));
                            int num663 = ModContent.NPCType<CrabShroom>();
                            int num664 = NPC.NewNPC(x, y, num663);
                            Main.npc[num664].SetDefaults(num663, -1f);
                            Main.npc[num664].velocity.X = Main.rand.Next(-50, 51) * 0.1f;
                            Main.npc[num664].velocity.Y = Main.rand.Next(-50, -31) * 0.1f;
                            if (Main.netMode == NetmodeID.Server && num664 < 200)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
                    }
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return npc.ai[0] > 1f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D glow = ModContent.GetTexture("CalamityMod/NPCs/Crabulon/CrabulonIdleGlow");
			Texture2D texture = ModContent.GetTexture("CalamityMod/NPCs/Crabulon/CrabulonIdleAlt");
			Texture2D textureGlow = ModContent.GetTexture("CalamityMod/NPCs/Crabulon/CrabulonIdleAltGlow");
			Texture2D textureAttack = ModContent.GetTexture("CalamityMod/NPCs/Crabulon/CrabulonAttack");
			Texture2D textureAttackGlow = ModContent.GetTexture("CalamityMod/NPCs/Crabulon/CrabulonAttackGlow");

			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(Main.npcTexture[npc.type].Width, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

			if (npc.ai[0] > 2f)
			{
				vector11 = new Vector2(textureAttack.Width / 2, textureAttack.Height / 2);
				vector43 = npc.Center - Main.screenPosition;
				vector43 -= new Vector2(textureAttack.Width, textureAttack.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
				vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);

				spriteBatch.Draw(textureAttack, vector43, npc.frame, npc.GetAlpha(drawColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

				spriteBatch.Draw(textureAttackGlow, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
			}
			else if (npc.ai[0] == 2f)
			{
				vector11 = new Vector2(texture.Width / 2, texture.Height / 2);
				vector43 = npc.Center - Main.screenPosition;
				vector43 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
				vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);

				spriteBatch.Draw(texture, vector43, npc.frame, npc.GetAlpha(drawColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

				spriteBatch.Draw(textureGlow, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
			}
			else
			{
				spriteBatch.Draw(Main.npcTexture[npc.type], vector43, npc.frame, npc.GetAlpha(drawColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

				spriteBatch.Draw(glow, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
			}

			return false;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<CrabulonTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeCrabulon>(), true, !CalamityWorld.downedCrabulon);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedCrabulon, 2, 0, 0);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Dryad }, CalamityWorld.downedCrabulon);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItem(npc, ItemID.GlowingMushroom, 20, 30);
                DropHelper.DropItem(npc, ItemID.MushroomGrassSeeds, 3, 6);

                // Weapons
                float w = DropHelper.DirectWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<MycelialClaws>(w),
                    DropHelper.WeightStack<Fungicide>(w),
                    DropHelper.WeightStack<HyphaeRod>(w),
                    DropHelper.WeightStack<Mycoroot>(w),
                    DropHelper.WeightStack<Shroomerang>(w)
                );

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<CrabulonMask>(), 7);
            }

            // Mark Crabulon as dead
            CalamityWorld.downedCrabulon = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 56, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 200;
                npc.height = 100;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 56, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 56, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 56, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                float randomSpread = Main.rand.Next(-200, 200) / 100;
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon4"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon5"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon6"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon7"), 1f);
            }
        }
    }
}
