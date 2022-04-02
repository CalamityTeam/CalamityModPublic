using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Polterghast
{
    public class PhantomFuckYou : ModNPC
    {
        private bool start = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom");
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.width = 30;
            npc.height = 30;
            npc.defense = 45;
            npc.DR_NERD(0.1f);
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.damage = 0;
            npc.LifeMaxNERB(Main.expertMode ? 11250 : 8750, CalamityWorld.death ? 22500 : 18750, 15000);
            npc.HitSound = SoundID.NPCHit36;
            npc.DeathSound = SoundID.NPCDeath39;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(start);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            start = reader.ReadBoolean();
        }

        public override bool PreAI()
        {
            if (start)
            {
                start = false;

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                }

                npc.ai[1] = npc.ai[0];
            }

            if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return false;
            }

            bool chargePhase = Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] >= 420f;

            // Percent life remaining, Polter
            float lifeRatio = Main.npc[CalamityGlobalNPC.ghostBoss].life / Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax;

            // Scale multiplier based on nearby active tiles
            float tileEnrageMult = Main.npc[CalamityGlobalNPC.ghostBoss].ai[3];

            npc.TargetClosest(true);

            Vector2 direction = Main.player[npc.target].Center - npc.Center;
            direction.Normalize();
            direction *= 0.05f;
            npc.rotation = direction.ToRotation();

            if (!chargePhase)
            {
                npc.ai[2] += 1f;
                if (npc.ai[2] >= 150f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int type = ModContent.ProjectileType<PhantomMine>();
                        int damage = npc.GetProjectileDamage(type);
                        float maxVelocity = 8f * tileEnrageMult;
                        float acceleration = 1.15f + (tileEnrageMult - 1f) * 0.15f;
                        Projectile.NewProjectile(npc.Center, direction, type, damage, 1f, npc.target, maxVelocity, acceleration);
                    }
                    npc.ai[2] = 0f;
                }
            }

            NPC parent = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<Polterghast>())];
            double deg = npc.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = 500;
            npc.position.X = parent.Center.X - (int)(Math.Cos(rad) * dist) - npc.width / 2;
            npc.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * dist) - npc.height / 2;
            float SPEEN = 1f - lifeRatio * 2f;
            if (SPEEN < 0f)
                SPEEN = 0f;
            npc.ai[1] += 0.5f + SPEEN;
            return false;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[npc.type];
            Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = npc.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
                    vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(npc.position, npc.width, npc.height, 180, hitDirection, -1f, 0, default, 1f);
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 45;
                npc.height = 45;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 2; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
