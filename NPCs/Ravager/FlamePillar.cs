using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    public class FlamePillar : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Pillar");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.GetNPCDamage();
            npc.width = 40;
            npc.height = 150;
            npc.defense = 35;
            npc.DR_NERD(0.2f);
            npc.chaseable = false;
            npc.canGhostHeal = false;
            npc.lifeMax = CalamityWorld.downedProvidence ? 14000 : 3500;
            npc.alpha = 255;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToWater = true;
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
            bool provy = CalamityWorld.downedProvidence;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                npc.life = 0;
                HitEffect(npc.direction, 9999);
                npc.netUpdate = true;
                return;
            }

            if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0f, 0.5f, 0.5f);

            if (npc.alpha > 0)
            {
                npc.damage = 0;

                npc.alpha -= 5;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }
            else
            {
                if (CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive)
                    npc.damage = (int)(npc.defDamage * 1.5);
                else
                    npc.damage = npc.defDamage;
            }

            if (npc.ai[0] == 0f)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 60f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 180f;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                if (npc.ai[1] >= 0f)
                {
                    npc.ai[1] -= 1f;
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] % (death ? 45f : 60f) == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float speedY = -12f;
                            float speedX = 0f;
                            switch ((int)npc.ai[2])
                            {
                                case 0:
                                    break;
                                case 1:
                                    speedX = 2f;
                                    break;
                                case 2:
                                    speedX = -2f;
                                    break;
                                default:
                                    break;
                            }
                            Vector2 velocity = new Vector2(speedX, speedY);
                            int type = ModContent.ProjectileType<RavagerFlame>();
                            int damage = npc.GetProjectileDamage(type);
                            Projectile.NewProjectile(npc.Center, velocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                        }
                        npc.ai[2] += 1f;
                        npc.localAI[0] = 0f;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    npc.life = 0;
                    HitEffect(npc.direction, 9999);
                    npc.netUpdate = true;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[npc.type];
            Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);
            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);

            spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Ravager/FlamePillarGlow");
            Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

            spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive() => false;

        public override bool PreNPCLoot() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 50;
                npc.height = 180;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 30; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 135, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
            else
            {
                for (int num621 = 0; num621 < 2; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 2; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
