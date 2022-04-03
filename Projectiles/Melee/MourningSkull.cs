using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class MourningSkull : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mourning Skull");
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
                int num554 = (int)Projectile.ai[0];
                if (Main.npc[num554].active)
                {
                    float num555 = 8f;
                    Vector2 vector44 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num556 = Main.npc[num554].position.X - vector44.X;
                    float num557 = Main.npc[num554].position.Y - vector44.Y;
                    float num558 = (float)Math.Sqrt((double)(num556 * num556 + num557 * num557));
                    num558 = num555 / num558;
                    num556 *= num558;
                    num557 *= num558;
                    Projectile.velocity.X = (Projectile.velocity.X * 14f + num556) / 15f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 14f + num557) / 15f;
                }
                else
                {
                    float num559 = 1000f;
                    int num3;
                    for (int num560 = 0; num560 < 200; num560 = num3 + 1)
                    {
                        if (Main.npc[num560].CanBeChasedBy(Projectile, false))
                        {
                            float num561 = Main.npc[num560].position.X + (float)(Main.npc[num560].width / 2);
                            float num562 = Main.npc[num560].position.Y + (float)(Main.npc[num560].height / 2);
                            float num563 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num561) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num562);
                            if (num563 < num559 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num560].position, Main.npc[num560].width, Main.npc[num560].height))
                            {
                                num559 = num563;
                                Projectile.ai[0] = (float)num560;
                            }
                        }
                        num3 = num560;
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

                int num564 = 8;
                int num565 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num564, Projectile.position.Y + (float)num564), Projectile.width - num564 * 2, Projectile.height - num564 * 2, Main.rand.NextBool(2) ? 5 : 6, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[num565];
                dust.velocity *= 0.5f;
                dust = Main.dust[num565];
                dust.velocity += Projectile.velocity * 0.5f;
                Main.dust[num565].noGravity = true;
                Main.dust[num565].noLight = true;
                Main.dust[num565].scale = 1.4f;
                return;
            }

            Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Main.rand.Next(0, 128));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 5, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 10; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int k = 0; k < 2; k++)
                {
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 174, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                    Projectile.NewProjectile(Projectile.position.X, Projectile.position.Y, (float)Main.rand.Next(-35, 36) * 0.2f, (float)Main.rand.Next(-35, 36) * 0.2f, ModContent.ProjectileType<TinyFlare>(),
                     (int)((double)Projectile.damage * 0.35), Projectile.knockBack * 0.35f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
