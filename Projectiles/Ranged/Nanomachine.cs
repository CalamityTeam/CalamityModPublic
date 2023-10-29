using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class Nanomachine : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] = Main.rand.Next(1, 3);
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/Nanomachine").Value;
            if (Projectile.localAI[1] == 2)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/Nanomachine2").Value;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, texture);
            return false;
        }

        public override void AI()
        {
            //Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
            }

            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * Projectile.direction;


            Vector2 dspeed = -Projectile.velocity * Main.rand.NextFloat(0.5f, 0.8f);
            float x2 = Projectile.Center.X - Projectile.velocity.X / 10f;
            float y2 = Projectile.Center.Y - Projectile.velocity.Y / 10f;
            int nanoDust = Dust.NewDust(new Vector2(x2, y2), 1, 1, 229, 0f, 0f, 0, default, 0.8f);
            Main.dust[nanoDust].alpha = Projectile.alpha;
            Main.dust[nanoDust].position.X = x2;
            Main.dust[nanoDust].position.Y = y2;
            Main.dust[nanoDust].velocity = dspeed;
            Main.dust[nanoDust].noGravity = true;

            float velocityAdjust = (float)Math.Sqrt((double)(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y));
            float aiTracker = Projectile.localAI[0];
            if (aiTracker == 0f)
            {
                Projectile.localAI[0] = velocityAdjust;
                aiTracker = velocityAdjust;
            }
            float projX = Projectile.position.X;
            float projY = Projectile.position.Y;
            float constant = 800f;
            bool canHit = false;
            int targetID = 0;
            if (Projectile.ai[1] == 0f)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false) && (Projectile.ai[1] == 0f || Projectile.ai[1] == (float)(i + 1)))
                    {
                        float targetX = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                        float targetY = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                        float targetDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - targetX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - targetY);
                        if (targetDist < constant && Collision.CanHit(new Vector2(Projectile.position.X + (float)(Projectile.width / 2), Projectile.position.Y + (float)(Projectile.height / 2)), 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                        {
                            constant = targetDist;
                            projX = targetX;
                            projY = targetY;
                            canHit = true;
                            targetID = i;
                        }
                    }
                }
                if (canHit)
                {
                    Projectile.ai[1] = (float)(targetID + 1);
                }
                canHit = false;
            }
            if (Projectile.ai[1] > 0f)
            {
                int secondTargetID = (int)(Projectile.ai[1] - 1f);
                if (Main.npc[secondTargetID].active && Main.npc[secondTargetID].CanBeChasedBy(Projectile, true) && !Main.npc[secondTargetID].dontTakeDamage)
                {
                    float secondTargetX = Main.npc[secondTargetID].position.X + (float)(Main.npc[secondTargetID].width / 2);
                    float secondTargetY = Main.npc[secondTargetID].position.Y + (float)(Main.npc[secondTargetID].height / 2);
                    float secondTargetDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - secondTargetX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - secondTargetY);
                    if (secondTargetDist < 1000f)
                    {
                        canHit = true;
                        projX = Main.npc[secondTargetID].position.X + (float)(Main.npc[secondTargetID].width / 2);
                        projY = Main.npc[secondTargetID].position.Y + (float)(Main.npc[secondTargetID].height / 2);
                    }
                }
                else
                {
                    Projectile.ai[1] = 0f;
                }
            }
            if (!Projectile.friendly)
            {
                canHit = false;
            }
            if (canHit)
            {
                Vector2 projPos = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float projXSpeed = projX - projPos.X;
                float projYSpeed = projY - projPos.Y;
                float projSpeedAdjust = (float)Math.Sqrt((double)(projXSpeed * projXSpeed + projYSpeed * projYSpeed));
                projSpeedAdjust = aiTracker / projSpeedAdjust;
                projXSpeed *= projSpeedAdjust;
                projYSpeed *= projSpeedAdjust;
                int projVelAdjust = 12;
                Projectile.velocity.X = (Projectile.velocity.X * (float)(projVelAdjust - 1) + projXSpeed) / (float)projVelAdjust;
                Projectile.velocity.Y = (Projectile.velocity.Y * (float)(projVelAdjust - 1) + projYSpeed) / (float)projVelAdjust;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 180);
        }
    }
}
