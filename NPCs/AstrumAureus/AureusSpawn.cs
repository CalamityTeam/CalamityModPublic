using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumAureus
{
    public class AureusSpawn : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aureus Spawn");
            Main.npcFrameCount[npc.type] = 4;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.aiStyle = -1;
            aiType = -1;
            npc.GetNPCDamage();
            npc.width = 90;
            npc.height = 60;
            npc.alpha = 255;
            npc.defense = 10;
            npc.lifeMax = NPC.downedMoonlord ? 8250 : 3000;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            npc.damage = 0;

            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.6f, 0.25f, 0f);

            npc.rotation = Math.Abs(npc.velocity.X) * (float)npc.direction * 0.04f;

            npc.spriteDirection = npc.direction;

            if (npc.alpha > 0)
            {
                npc.alpha -= 5;
                int num;
                for (int num245 = 0; num245 < 10; num245 = num + 1)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), npc.velocity.X, npc.velocity.Y, 255, default, 0.8f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.5f;
                    num = num245;
                }
            }

            npc.TargetClosest();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            if (lifeRatio <= 0.5f || Main.dayTime)
            {
                npc.ai[1] += 1f;
                npc.dontTakeDamage = true;
                Vector2 vector = Main.player[npc.target].Center - npc.Center;
                Point point15 = npc.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(point15);
                bool flag121 = tileSafely.nactive() && Main.tileSolid[tileSafely.type] && !Main.tileSolidTop[tileSafely.type] && !TileID.Sets.Platforms[tileSafely.type];
                if (npc.ai[1] > ((CalamityWorld.death || BossRushEvent.BossRushActive) ? 600f : 300f))
                {
                    npc.Calamity().newAI[0] += 1f;
                    npc.scale = MathHelper.Lerp(1f, 2f, npc.Calamity().newAI[0] / 45f);
                }
                if (vector.Length() < 60f || flag121 || npc.Calamity().newAI[0] >= 45f)
                {
                    npc.dontTakeDamage = false;
                    CheckDead();
                    npc.life = 0;
                    return;
                }
                float num1372 = 18f;
                Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
                float num1373 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector167.X;
                float num1374 = Main.player[npc.target].Center.Y - vector167.Y;
                float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
                float num1376 = num1372 / num1375;
                num1373 *= num1376;
                num1374 *= num1376;
                npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
                npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;

                if (CalamityWorld.death || BossRushEvent.BossRushActive)
                {
                    float pushVelocity = 0.5f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active)
                        {
                            if (i != npc.whoAmI && Main.npc[i].type == npc.type)
                            {
                                if (Vector2.Distance(npc.Center, Main.npc[i].Center) < 320f)
                                {
                                    if (npc.position.X < Main.npc[i].position.X)
                                        npc.velocity.X = npc.velocity.X - pushVelocity;
                                    else
                                        npc.velocity.X = npc.velocity.X + pushVelocity;

                                    if (npc.position.Y < Main.npc[i].position.Y)
                                        npc.velocity.Y = npc.velocity.Y - pushVelocity;
                                    else
                                        npc.velocity.Y = npc.velocity.Y + pushVelocity;
                                }
                            }
                        }
                    }
                }

                return;
            }

            float num1446 = 7f;
            int num1447 = 480;
            if (npc.localAI[1] == 1f)
            {
                npc.localAI[1] = 0f;
                if (Main.rand.NextBool(4))
                {
                    npc.ai[0] = num1447;
                }
            }

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
            {
                npc.localAI[0] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 180f)
                {
                    npc.localAI[0] = 0f;
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 109);
                        float speed = 5f;
                        Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                        float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X;
                        float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y;
                        float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                        num8 = speed / num8;
                        num6 *= num8;
                        num7 *= num8;
                        int type = ModContent.ProjectileType<AstralFlame>();
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, num6, num7, type, damage, 0f, Main.myPlayer);
                    }
                }
            }

            Vector2 value53 = npc.Center + new Vector2((float)(npc.direction * 20), 6f);
            Vector2 vector251 = Main.player[npc.target].Center - value53;
            bool flag104 = Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1);
            if (vector251.Length() > 400f || !flag104)
            {
                Vector2 value54 = vector251;
                if (value54.Length() > num1446)
                {
                    value54.Normalize();
                    value54 *= num1446;
                }
                int num1448 = 30;
                npc.velocity = (npc.velocity * (float)(num1448 - 1) + value54) / (float)num1448;
            }
            else
            {
                npc.velocity *= 0.98f;
            }

            if (npc.ai[2] != 0f && npc.ai[3] != 0f)
            {
                Main.PlaySound(SoundID.Item8, npc.Center);
                int num;
                for (int num1449 = 0; num1449 < 20; num1449 = num + 1)
                {
                    int num1450 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, Color.Transparent, 0.6f);
                    Dust dust = Main.dust[num1450];
                    dust.velocity *= 3f;
                    Main.dust[num1450].noGravity = true;
                    Main.dust[num1450].scale = 2.5f;
                    num = num1449;
                }
                npc.Center = new Vector2(npc.ai[2] * 16f, npc.ai[3] * 16f);
                npc.velocity = Vector2.Zero;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                Main.PlaySound(SoundID.Item8, npc.Center);
                for (int num1451 = 0; num1451 < 20; num1451 = num + 1)
                {
                    int num1452 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, Color.Transparent, 0.6f);
                    Dust dust = Main.dust[num1452];
                    dust.velocity *= 3f;
                    Main.dust[num1452].noGravity = true;
                    Main.dust[num1452].scale = 2.5f;
                    num = num1451;
                }
            }

            npc.ai[0] += 1f;
            if (npc.ai[0] >= num1447 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[0] = 0f;
                Point point12 = npc.Center.ToTileCoordinates();
                Point point13 = Main.player[npc.target].Center.ToTileCoordinates();
                int num1453 = 20;
                int num1454 = 3;
                int num1455 = 10;
                int num1456 = 1;
                int num1457 = 0;
                bool flag106 = false;
                if (vector251.Length() > 2000f)
                {
                    flag106 = true;
                }
                while (!flag106 && num1457 < 100)
                {
                    num1457++;
                    int num1458 = Main.rand.Next(point13.X - num1453, point13.X + num1453 + 1);
                    int num1459 = Main.rand.Next(point13.Y - num1453, point13.Y + num1453 + 1);
                    if ((num1459 < point13.Y - num1455 || num1459 > point13.Y + num1455 || num1458 < point13.X - num1455 || num1458 > point13.X + num1455) && (num1459 < point12.Y - num1454 || num1459 > point12.Y + num1454 || num1458 < point12.X - num1454 || num1458 > point12.X + num1454) && !Main.tile[num1458, num1459].nactive())
                    {
                        bool flag107 = true;
                        if (flag107 && Main.tile[num1458, num1459].lava())
                        {
                            flag107 = false;
                        }
                        if (flag107 && Collision.SolidTiles(num1458 - num1456, num1458 + num1456, num1459 - num1456, num1459 + num1456))
                        {
                            flag107 = false;
                        }
                        if (flag107)
                        {
                            npc.ai[2] = (float)num1458;
                            npc.ai[3] = (float)num1459;
                            break;
                        }
                    }
                }
                npc.netUpdate = true;
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.5f * bossLifeScale);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (npc.Calamity().newAI[0] >= 45f)
                return false;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[npc.type];
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 10;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = npc.GetAlpha(color38);
                    color38 *= (float)(num153 - num155) / 15f;
                    Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
                    vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AureusSpawnGlow");
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 *= (float)(num153 - num163) / 15f;
                    Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
                    vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
                    vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override bool PreNPCLoot() => false;

        public override bool CheckDead()
        {
            Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 14);
            npc.position.X = npc.position.X + (float)(npc.width / 2);
            npc.position.Y = npc.position.Y + (float)(npc.height / 2);
            npc.damage = (NPC.downedMoonlord && !BossRushEvent.BossRushActive) ? npc.defDamage * 2 : npc.defDamage;
            npc.width = npc.height = 432;
            npc.position.X = npc.position.X - (float)(npc.width / 2);
            npc.position.Y = npc.position.Y - (float)(npc.height / 2);
            for (int num621 = 0; num621 < 30; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
                Main.dust[num622].noGravity = true;
            }
            for (int num623 = 0; num623 < 60; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }
            return true;
        }
    }
}
