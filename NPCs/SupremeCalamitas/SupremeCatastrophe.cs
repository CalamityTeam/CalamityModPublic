using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    [AutoloadBossHead]
    public class SupremeCatastrophe : ModNPC
    {
        private int distanceY = 375;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastrophe");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.npcSlots = 5f;
            npc.width = 120;
            npc.height = 120;
            npc.defense = 100;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = CalamityWorld.bossRushActive ? 0.6f : CalamityWorld.death ? 0.75f : 0.7f;
            global.customDR = true;
            global.multDRReductions.Add(BuffID.Ichor, 0.9f);
            global.multDRReductions.Add(BuffID.CursedInferno, 0.91f);
            npc.lifeMax = CalamityWorld.revenge ? 2100000 : 1800000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 1500000;
            }
            double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
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
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(distanceY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            distanceY = reader.ReadInt32();
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
            CalamityGlobalNPC.SCalCatastrophe = npc.whoAmI;
            bool expertMode = Main.expertMode;
            if (CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            npc.TargetClosest(true);
            float num676 = 60f;
            float num677 = 1.5f;
            float distanceX = 750f;
            if (npc.ai[3] < 750f)
            {
                npc.ai[3] += 1f;
                distanceY -= 1;
            }
            else if (npc.ai[3] < 1500f)
            {
                npc.ai[3] += 1f;
                distanceY += 1;
            }
            if (npc.ai[3] >= 1500f)
            {
                npc.ai[3] = 0f;
            }
            Vector2 vector83 = new Vector2(npc.Center.X, npc.Center.Y);
            float num678 = Main.player[npc.target].Center.X - vector83.X - distanceX;
            float num679 = Main.player[npc.target].Center.Y - vector83.Y + (float)distanceY;
            npc.rotation = 4.71f;
            float num680 = (float)Math.Sqrt((double)(num678 * num678 + num679 * num679));
            num680 = num676 / num680;
            num678 *= num680;
            num679 *= num680;
            if (npc.velocity.X < num678)
            {
                npc.velocity.X = npc.velocity.X + num677;
                if (npc.velocity.X < 0f && num678 > 0f)
                {
                    npc.velocity.X = npc.velocity.X + num677;
                }
            }
            else if (npc.velocity.X > num678)
            {
                npc.velocity.X = npc.velocity.X - num677;
                if (npc.velocity.X > 0f && num678 < 0f)
                {
                    npc.velocity.X = npc.velocity.X - num677;
                }
            }
            if (npc.velocity.Y < num679)
            {
                npc.velocity.Y = npc.velocity.Y + num677;
                if (npc.velocity.Y < 0f && num679 > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + num677;
                }
            }
            else if (npc.velocity.Y > num679)
            {
                npc.velocity.Y = npc.velocity.Y - num677;
                if (npc.velocity.Y > 0f && num679 < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - num677;
                }
            }
            if (npc.localAI[0] < 120f)
            {
                npc.localAI[0] += 1f;
            }
            if (npc.localAI[0] >= 120f)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 30f)
                {
                    npc.ai[1] = 0f;
                    Vector2 vector85 = new Vector2(npc.Center.X, npc.Center.Y);
                    float num689 = 4f;
                    int num690 = expertMode ? 150 : 200; //600 500
                    int num691 = ModContent.ProjectileType<BrimstoneHellblast2>();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num695 = Projectile.NewProjectile(vector85.X, vector85.Y, num689, 0f, num691, num690, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                npc.ai[2] += 1f;
                if (!NPC.AnyNPCs(ModContent.NPCType<SupremeCataclysm>()))
                {
                    npc.ai[2] += 2f;
                }
                if (npc.ai[2] >= 300f)
                {
                    npc.ai[2] = 0f;
                    float num689 = 7f;
                    int num690 = expertMode ? 150 : 200; //600 500
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 20);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (i = 0; i < 8; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)(Math.Sin(offsetAngle) * num689), (float)(Math.Cos(offsetAngle) * num689), ModContent.ProjectileType<BrimstoneBarrage>(), num690, 0f, Main.myPlayer, 0f, 1f);
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)(-Math.Sin(offsetAngle) * num689), (float)(-Math.Cos(offsetAngle) * num689), ModContent.ProjectileType<BrimstoneBarrage>(), num690, 0f, Main.myPlayer, 0f, 1f);
                        }
                    }
                    for (int dust = 0; dust <= 5; dust++)
                    {
                        Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, 235, 0f, 0f);
                    }
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ModContent.ProjectileType<SonOfYharon>())
            {
                damage /= 2;
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            return !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            Color color24 = npc.GetAlpha(drawColor);
            Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            Texture2D texture2D3 = Main.npcTexture[npc.type];
            int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int y3 = num156 * (int)npc.frameCounter;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && Lighting.NotRetro)
            {
                Color color26 = npc.GetAlpha(color25);
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
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
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
    }
}
