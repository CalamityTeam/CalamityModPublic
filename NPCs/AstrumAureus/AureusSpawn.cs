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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.AstrumAureus
{
    public class AureusSpawn : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aureus Spawn");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.GetNPCDamage();
            NPC.width = 90;
            NPC.height = 60;
            NPC.alpha = 255;
            NPC.defense = 10;
            NPC.lifeMax = NPC.downedMoonlord ? 8250 : 3000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            NPC.damage = 0;

            Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 0.6f, 0.25f, 0f);

            NPC.rotation = Math.Abs(NPC.velocity.X) * (float)NPC.direction * 0.04f;

            NPC.spriteDirection = NPC.direction;

            if (NPC.alpha > 0)
            {
                NPC.alpha -= 5;
                int num;
                for (int num245 = 0; num245 < 10; num245 = num + 1)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), NPC.velocity.X, NPC.velocity.Y, 255, default, 0.8f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.5f;
                    num = num245;
                }
            }

            NPC.TargetClosest();

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            if (lifeRatio <= 0.5f || Main.dayTime)
            {
                NPC.ai[1] += 1f;
                NPC.dontTakeDamage = true;
                Vector2 vector = Main.player[NPC.target].Center - NPC.Center;
                Point point15 = NPC.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(point15);
                bool flag121 = tileSafely.HasUnactuatedTile && Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType] && !TileID.Sets.Platforms[tileSafely.TileType];
                if (NPC.ai[1] > ((CalamityWorld.death || BossRushEvent.BossRushActive) ? 600f : 300f))
                {
                    NPC.Calamity().newAI[0] += 1f;
                    NPC.scale = MathHelper.Lerp(1f, 2f, NPC.Calamity().newAI[0] / 45f);
                }
                if (vector.Length() < 60f || flag121 || NPC.Calamity().newAI[0] >= 45f)
                {
                    NPC.dontTakeDamage = false;
                    CheckDead();
                    NPC.life = 0;
                    return;
                }
                float num1372 = 18f;
                Vector2 vector167 = new Vector2(NPC.Center.X + (float)(NPC.direction * 20), NPC.Center.Y + 6f);
                float num1373 = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - vector167.X;
                float num1374 = Main.player[NPC.target].Center.Y - vector167.Y;
                float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
                float num1376 = num1372 / num1375;
                num1373 *= num1376;
                num1374 *= num1376;
                NPC.velocity.X = (NPC.velocity.X * 50f + num1373) / 51f;
                NPC.velocity.Y = (NPC.velocity.Y * 50f + num1374) / 51f;

                if (CalamityWorld.death || BossRushEvent.BossRushActive)
                {
                    float pushVelocity = 0.5f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active)
                        {
                            if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                            {
                                if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < 320f)
                                {
                                    if (NPC.position.X < Main.npc[i].position.X)
                                        NPC.velocity.X = NPC.velocity.X - pushVelocity;
                                    else
                                        NPC.velocity.X = NPC.velocity.X + pushVelocity;

                                    if (NPC.position.Y < Main.npc[i].position.Y)
                                        NPC.velocity.Y = NPC.velocity.Y - pushVelocity;
                                    else
                                        NPC.velocity.Y = NPC.velocity.Y + pushVelocity;
                                }
                            }
                        }
                    }
                }

                return;
            }

            float num1446 = 7f;
            int num1447 = 480;
            if (NPC.localAI[1] == 1f)
            {
                NPC.localAI[1] = 0f;
                if (Main.rand.NextBool(4))
                {
                    NPC.ai[0] = num1447;
                }
            }

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
            {
                NPC.localAI[0] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] >= 180f)
                {
                    NPC.localAI[0] = 0f;
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 109);
                        float speed = 5f;
                        Vector2 vector = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)(NPC.height / 2));
                        float num6 = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - vector.X;
                        float num7 = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - vector.Y;
                        float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                        num8 = speed / num8;
                        num6 *= num8;
                        num7 *= num8;
                        int type = ModContent.ProjectileType<AstralFlame>();
                        int damage = NPC.GetProjectileDamage(type);
                        Projectile.NewProjectile(NPC.Center.X, NPC.Center.Y, num6, num7, type, damage, 0f, Main.myPlayer);
                    }
                }
            }

            Vector2 value53 = NPC.Center + new Vector2((float)(NPC.direction * 20), 6f);
            Vector2 vector251 = Main.player[NPC.target].Center - value53;
            bool flag104 = Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1);
            if (vector251.Length() > 400f || !flag104)
            {
                Vector2 value54 = vector251;
                if (value54.Length() > num1446)
                {
                    value54.Normalize();
                    value54 *= num1446;
                }
                int num1448 = 30;
                NPC.velocity = (NPC.velocity * (float)(num1448 - 1) + value54) / (float)num1448;
            }
            else
            {
                NPC.velocity *= 0.98f;
            }

            if (NPC.ai[2] != 0f && NPC.ai[3] != 0f)
            {
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                int num;
                for (int num1449 = 0; num1449 < 20; num1449 = num + 1)
                {
                    int num1450 = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, Color.Transparent, 0.6f);
                    Dust dust = Main.dust[num1450];
                    dust.velocity *= 3f;
                    Main.dust[num1450].noGravity = true;
                    Main.dust[num1450].scale = 2.5f;
                    num = num1449;
                }
                NPC.Center = new Vector2(NPC.ai[2] * 16f, NPC.ai[3] * 16f);
                NPC.velocity = Vector2.Zero;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                for (int num1451 = 0; num1451 < 20; num1451 = num + 1)
                {
                    int num1452 = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, Color.Transparent, 0.6f);
                    Dust dust = Main.dust[num1452];
                    dust.velocity *= 3f;
                    Main.dust[num1452].noGravity = true;
                    Main.dust[num1452].scale = 2.5f;
                    num = num1451;
                }
            }

            NPC.ai[0] += 1f;
            if (NPC.ai[0] >= num1447 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.ai[0] = 0f;
                Point point12 = NPC.Center.ToTileCoordinates();
                Point point13 = Main.player[NPC.target].Center.ToTileCoordinates();
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
                    if ((num1459 < point13.Y - num1455 || num1459 > point13.Y + num1455 || num1458 < point13.X - num1455 || num1458 > point13.X + num1455) && (num1459 < point12.Y - num1454 || num1459 > point12.Y + num1454 || num1458 < point12.X - num1454 || num1458 > point12.X + num1454) && !Main.tile[num1458, num1459].HasUnactuatedTile)
                    {
                        bool flag107 = true;
                        if (flag107 && Main.tile[num1458, num1459].LiquidType == LiquidID.Lava)
                        {
                            flag107 = false;
                        }
                        if (flag107 && Collision.SolidTiles(num1458 - num1456, num1458 + num1456, num1459 - num1456, num1459 + num1456))
                        {
                            flag107 = false;
                        }
                        if (flag107)
                        {
                            NPC.ai[2] = (float)num1458;
                            NPC.ai[3] = (float)num1459;
                            break;
                        }
                    }
                }
                NPC.netUpdate = true;
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f * bossLifeScale);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (NPC.Calamity().newAI[0] >= 45f)
                return false;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 10;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (float)(num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - Main.screenPosition;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AureusSpawnGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 *= (float)(num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - Main.screenPosition;
                    vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 173, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 173, hitDirection, -1f, 0, default, 1f);
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
            SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 14);
            NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
            NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
            NPC.damage = (NPC.downedMoonlord && !BossRushEvent.BossRushActive) ? NPC.defDamage * 2 : NPC.defDamage;
            NPC.width = NPC.height = 432;
            NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
            NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
            for (int num621 = 0; num621 < 30; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
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
                int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }
            return true;
        }
    }
}
