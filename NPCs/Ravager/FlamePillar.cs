using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    public class FlamePillar : ModNPC
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/RavagerRockPillarHit", 3);
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.width = 40;
            NPC.height = 150;
            NPC.defense = 35;
            NPC.DR_NERD(0.2f);
            NPC.chaseable = false;
            NPC.canGhostHeal = false;
            NPC.lifeMax = DownedBossSystem.downedProvidence ? 14000 : 3500;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.HitSound = HitSound;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            bool provy = DownedBossSystem.downedProvidence;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();

                return;
            }

            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 0.5f, 0.5f);

            if (NPC.alpha > 0)
            {
                NPC.damage = 0;

                NPC.alpha -= 5;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }
            else
            {
                if (DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive)
                    NPC.damage = (int)(NPC.defDamage * 1.5);
                else
                    NPC.damage = NPC.defDamage;
            }

            if (NPC.ai[0] == 0f)
            {
                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 60f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 180f;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                if (NPC.ai[1] >= 0f)
                {
                    NPC.ai[1] -= 1f;
                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] % (death ? 45f : 60f) == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float speedY = -12f;
                            float speedX = 0f;
                            switch ((int)NPC.ai[2])
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
                            int damage = NPC.GetProjectileDamage(type);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                        }
                        NPC.ai[2] += 1f;
                        NPC.localAI[0] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.StrikeInstantKill();
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / 2);
            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/FlamePillarGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 50;
                NPC.height = 180;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 30; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 135, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
            else
            {
                for (int num621 = 0; num621 < 2; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 2; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
