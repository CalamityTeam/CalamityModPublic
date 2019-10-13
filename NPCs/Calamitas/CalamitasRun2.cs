using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Calamitas
{
    [AutoloadBossHead]
    public class CalamitasRun2 : ModNPC
    {
        private bool canDespawn = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastrophe");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.damage = 65;
            npc.npcSlots = 5f;
            npc.width = 120;
            npc.height = 120;
            npc.defense = 10;
            npc.Calamity().RevPlusDR(0.15f);
			npc.LifeMaxNERD(7500, 11025, 13200, 800000, 900000);
			if (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive)
			{
				npc.damage *= 3;
				npc.defense *= 5;
				npc.lifeMax *= 3;
			}
			double HPBoost = Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[mod.BuffType("MarkedforDeath")] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.Daybreak] = false;
            npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
            npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
            npc.buffImmune[mod.BuffType("DemonFlames")] = false;
            npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
            npc.buffImmune[mod.BuffType("HolyLight")] = false;
            npc.buffImmune[mod.BuffType("Nightwither")] = false;
            npc.buffImmune[mod.BuffType("Plague")] = false;
            npc.buffImmune[mod.BuffType("Shred")] = false;
            npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
            npc.buffImmune[mod.BuffType("SilvaStun")] = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Calamitas");
            else
                music = MusicID.Boss2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(canDespawn);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            canDespawn = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityGlobalNPC.catastrophe = npc.whoAmI;

            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool dayTime = Main.dayTime && !CalamityWorld.bossRushActive;
            bool provy = CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive;

            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(true);
            }
            Player player = Main.player[npc.target];

            float num840 = npc.position.X + (float)(npc.width / 2) - player.position.X - (float)(player.width / 2);
            float num841 = npc.position.Y + (float)npc.height - 59f - player.position.Y - (float)(player.height / 2);
            float num842 = (float)Math.Atan2((double)num841, (double)num840) + 1.57f;
            if (num842 < 0f)
            {
                num842 += 6.283f;
            }
            else if ((double)num842 > 6.283)
            {
                num842 -= 6.283f;
            }

            float num843 = 0.15f;
            if (npc.rotation < num842)
            {
                if ((double)(num842 - npc.rotation) > 3.1415)
                {
                    npc.rotation -= num843;
                }
                else
                {
                    npc.rotation += num843;
                }
            }
            else if (npc.rotation > num842)
            {
                if ((double)(npc.rotation - num842) > 3.1415)
                {
                    npc.rotation += num843;
                }
                else
                {
                    npc.rotation -= num843;
                }
            }

            if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
            {
                npc.rotation = num842;
            }
            if (npc.rotation < 0f)
            {
                npc.rotation += 6.283f;
            }
            else if ((double)npc.rotation > 6.283)
            {
                npc.rotation -= 6.283f;
            }
            if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
            {
                npc.rotation = num842;
            }

            if (!player.active || player.dead || (dayTime && !Main.eclipse))
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead || (dayTime && !Main.eclipse))
                {
                    npc.velocity = new Vector2(0f, -10f);
                    canDespawn = true;
                    if (npc.timeLeft > 150)
                    {
                        npc.timeLeft = 150;
                    }
                    return;
                }
            }
            else
            {
                canDespawn = false;
            }

            if (npc.ai[1] == 0f)
            {
                float num861 = 4.5f;
                float num862 = 0.2f;
                if (CalamityWorld.bossRushActive)
                {
                    num861 *= 1.5f;
                    num862 *= 1.5f;
                }

                int num863 = 1;
                if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
                {
                    num863 = -1;
                }

                Vector2 vector86 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num864 = player.position.X + (float)(player.width / 2) + (float)(num863 * (CalamityWorld.bossRushActive ? 270 : 180)) - vector86.X;
                float num865 = player.position.Y + (float)(player.height / 2) - vector86.Y;
                float num866 = (float)Math.Sqrt((double)(num864 * num864 + num865 * num865));
                if (expertMode)
                {
                    if (num866 > 300f)
                    {
                        num861 += 0.5f;
                    }
                    if (num866 > 400f)
                    {
                        num861 += 0.5f;
                    }
                    if (num866 > 500f)
                    {
                        num861 += 0.55f;
                    }
                    if (num866 > 600f)
                    {
                        num861 += 0.55f;
                    }
                    if (num866 > 700f)
                    {
                        num861 += 0.6f;
                    }
                    if (num866 > 800f)
                    {
                        num861 += 0.6f;
                    }
                }

                num866 = num861 / num866;
                num864 *= num866;
                num865 *= num866;

                if (npc.velocity.X < num864)
                {
                    npc.velocity.X = npc.velocity.X + num862;
                    if (npc.velocity.X < 0f && num864 > 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num862;
                    }
                }
                else if (npc.velocity.X > num864)
                {
                    npc.velocity.X = npc.velocity.X - num862;
                    if (npc.velocity.X > 0f && num864 < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - num862;
                    }
                }
                if (npc.velocity.Y < num865)
                {
                    npc.velocity.Y = npc.velocity.Y + num862;
                    if (npc.velocity.Y < 0f && num865 > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + num862;
                    }
                }
                else if (npc.velocity.Y > num865)
                {
                    npc.velocity.Y = npc.velocity.Y - num862;
                    if (npc.velocity.Y > 0f && num865 < 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y - num862;
                    }
                }

                npc.ai[2] += (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 2f : 1f;
                if (npc.ai[2] >= 180f)
                {
                    npc.ai[1] = 1f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.target = 255;
                    npc.netUpdate = true;
                }

                bool fireDelay = npc.ai[2] > 120f || (double)npc.life < (double)npc.lifeMax * 0.9;
                if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && fireDelay)
                {
                    npc.localAI[2] += 1f;
                    if (npc.localAI[2] > 36f)
                    {
                        npc.localAI[2] = 0f;
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 34);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[1] += 1f;
                        if (revenge)
                            npc.localAI[1] += 0.5f;

                        if (npc.localAI[1] > 50f)
                        {
                            npc.localAI[1] = 0f;
                            float num867 = CalamityWorld.bossRushActive ? 18f : 12f;
                            int num868 = expertMode ? 29 : 36;
                            int num869 = mod.ProjectileType("BrimstoneBall");
                            vector86 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            num864 = player.position.X + (float)(player.width / 2) - vector86.X;
                            num865 = player.position.Y + (float)(player.height / 2) - vector86.Y;
                            num866 = (float)Math.Sqrt((double)(num864 * num864 + num865 * num865));
                            num866 = num867 / num866;
                            num864 *= num866;
                            num865 *= num866;
                            num865 += npc.velocity.Y * 0.5f;
                            num864 += npc.velocity.X * 0.5f;
                            vector86.X -= num864 * 1f;
                            vector86.Y -= num865 * 1f;
                            Projectile.NewProjectile(vector86.X, vector86.Y, num864, num865, num869, num868 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
            else
            {
                if (npc.ai[1] == 1f)
                {
                    Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0);
                    npc.rotation = num842;
                    float num870 = 16f;
                    if (expertMode)
                    {
                        num870 += 2.5f;
                    }
                    if (revenge)
                    {
                        num870 += 2.5f;
                    }
                    if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                    {
                        num870 += 4f;
                    }
                    if (CalamityWorld.bossRushActive)
                    {
                        num870 *= 1.25f;
                    }

                    Vector2 vector87 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num871 = player.position.X + (float)(player.width / 2) - vector87.X;
                    float num872 = player.position.Y + (float)(player.height / 2) - vector87.Y;
                    float num873 = (float)Math.Sqrt((double)(num871 * num871 + num872 * num872));
                    num873 = num870 / num873;
                    npc.velocity.X = num871 * num873;
                    npc.velocity.Y = num872 * num873;
                    npc.ai[1] = 2f;
                    return;
                }
                if (npc.ai[1] == 2f)
                {
                    npc.ai[2] += 1f;
                    if (expertMode)
                    {
                        npc.ai[2] += 0.5f;
                    }
                    if (revenge)
                    {
                        npc.ai[2] += 0.5f;
                    }
                    if (CalamityWorld.bossRushActive)
                    {
                        npc.ai[2] += 0.25f;
                    }

                    if (npc.ai[2] >= 60f) //50
                    {
                        npc.velocity.X = npc.velocity.X * 0.93f;
                        npc.velocity.Y = npc.velocity.Y * 0.93f;
                        if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                        {
                            npc.velocity.X = 0f;
                        }
                        if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
                        {
                            npc.velocity.Y = 0f;
                        }
                    }
                    else
                    {
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;
                    }

                    if (npc.ai[2] >= 90f) //80
                    {
                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;
                        npc.target = 255;
                        npc.rotation = num842;
                        if (npc.ai[3] >= 4f)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[3] = 0f;
                            return;
                        }
                        npc.ai[1] = 1f;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            Microsoft.Xna.Framework.Color color24 = npc.GetAlpha(drawColor);
            Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            Texture2D texture2D3 = Main.npcTexture[npc.type];
            int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int y3 = num156 * (int)npc.frameCounter;
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            while (npc.ai[1] > 0f && Lighting.NotRetro && ((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)))
            {
                Microsoft.Xna.Framework.Color color26 = npc.GetAlpha(color25);
                {
                    goto IL_6899;
                }
                IL_6881:
                num161 += num158;
                continue;
                IL_6899:
                float num164 = (float)(num157 - num161);
                if (num158 < 0)
                {
                    num164 = (float)(num159 - num161);
                }
                color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
                Vector2 value4 = npc.oldPos[num161];
                float num165 = npc.rotation;
                Main.spriteBatch.Draw(texture2D3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
                goto IL_6881;
            }
            var something = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture2D3, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, something, 0);
            return false;
        }

        public override bool CheckActive()
        {
            return canDespawn;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(10))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CatastropheTrophy"));
            }
            if (Main.expertMode)
            {
                if (Main.rand.NextBool(10))
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrushsawCrasher"));
            }
            else if (Main.rand.NextBool(15))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrushsawCrasher"));
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Catastrophe"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Catastrophe2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Catastrophe3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Catastrophe4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/Catastrophe5"), 1f);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 100;
                npc.height = 100;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(mod.BuffType("MarkedforDeath"), 180);
                player.AddBuff(mod.BuffType("Horror"), 180, true);
            }
            player.AddBuff(mod.BuffType("BrimstoneFlames"), 300, true);
        }
    }
}
