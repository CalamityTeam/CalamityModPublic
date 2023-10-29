using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class MourningSkull : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 0f)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 50;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2((double)-(double)Projectile.velocity.Y, (double)-(double)Projectile.velocity.X);
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);
            }

            if (Projectile.ai[0] >= 0f && Projectile.ai[0] < 200f)
            {
                int npcTracker = (int)Projectile.ai[0];
                if (Main.npc[npcTracker].active)
                {
                    Vector2 projPos = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float npcXDist = Main.npc[npcTracker].position.X - projPos.X;
                    float npcYDist = Main.npc[npcTracker].position.Y - projPos.Y;
                    float npcDistance = (float)Math.Sqrt((double)(npcXDist * npcXDist + npcYDist * npcYDist));
                    npcDistance = 8f / npcDistance;
                    npcXDist *= npcDistance;
                    npcYDist *= npcDistance;
                    Projectile.velocity.X = (Projectile.velocity.X * 14f + npcXDist) / 15f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 14f + npcYDist) / 15f;
                }
                else
                {
                    float homingRange = 1000f;
                    int inc;
                    for (int j = 0; j < Main.maxNPCs; j = inc + 1)
                    {
                        if (Main.npc[j].CanBeChasedBy(Projectile, false))
                        {
                            float targetX = Main.npc[j].position.X + (float)(Main.npc[j].width / 2);
                            float targetY = Main.npc[j].position.Y + (float)(Main.npc[j].height / 2);
                            float targetDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - targetX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - targetY);
                            if (targetDist < homingRange && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[j].position, Main.npc[j].width, Main.npc[j].height))
                            {
                                homingRange = targetDist;
                                Projectile.ai[0] = (float)j;
                            }
                        }
                        inc = j;
                    }
                }

                if (Projectile.velocity.X < 0f)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = (float)Math.Atan2((double)-(double)Projectile.velocity.Y, (double)-(double)Projectile.velocity.X);
                }
                else
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);
                }

                int eightConst = 8;
                int mourningDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)eightConst, Projectile.position.Y + (float)eightConst), Projectile.width - eightConst * 2, Projectile.height - eightConst * 2, Main.rand.NextBool() ? 5 : 6, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[mourningDust];
                dust.velocity *= 0.5f;
                dust = Main.dust[mourningDust];
                dust.velocity += Projectile.velocity * 0.5f;
                Main.dust[mourningDust].noGravity = true;
                Main.dust[mourningDust].noLight = true;
                Main.dust[mourningDust].scale = 1.4f;
                return;
            }

            Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Main.rand.Next(0, 128));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 5; i++)
            {
                int bloody = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 5, 0f, 0f, 100, default, 2f);
                Main.dust[bloody].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[bloody].scale = 0.5f;
                    Main.dust[bloody].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 10; j++)
            {
                int fiery = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[fiery].noGravity = true;
                Main.dust[fiery].velocity *= 5f;
                fiery = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[fiery].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 300);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int k = 0; k < 2; k++)
                {
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 174, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, Main.rand.Next(-35, 36) * 0.2f, Main.rand.Next(-35, 36) * 0.2f, ModContent.ProjectileType<TinyFlare>(),
                     (int)(Projectile.damage * 0.35), Projectile.knockBack * 0.35f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
