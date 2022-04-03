using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class DestructionBolt : ModProjectile
    {
        public int dustType = 191;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Destruction Bolt");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.Calamity().rogue = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Main.rand.Next(8) == 0)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }

            if (Projectile.alpha > 0)
                Projectile.alpha -= 8;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            float num29 = 5f;
            float num30 = 300f;
            float scaleFactor = 6f;
            Vector2 value7 = new Vector2(10f, 20f);
            int num32 = 3 * Projectile.MaxUpdates;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                Projectile.localAI[0] = (float)-(float)Main.rand.Next(48);
            }
            else if (Projectile.ai[1] == 1f && Projectile.owner == Main.myPlayer)
            {
                if (Projectile.alpha < 128)
                {
                    int num35 = -1;
                    float num36 = num30;
                    for (int num37 = 0; num37 < Main.maxNPCs; num37++)
                    {
                        if (Main.npc[num37].active && Main.npc[num37].CanBeChasedBy(Projectile, false))
                        {
                            Vector2 center3 = Main.npc[num37].Center;
                            float num38 = Vector2.Distance(center3, Projectile.Center);
                            if (num38 < num36 && num35 == -1 && Collision.CanHitLine(Projectile.Center, 1, 1, center3, 1, 1))
                            {
                                num36 = num38;
                                num35 = num37;
                            }
                        }
                    }
                    if (num36 < 4f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (num35 != -1)
                    {
                        Projectile.ai[1] = num29 + 1f;
                        Projectile.ai[0] = (float)num35;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (Projectile.ai[1] > num29)
            {
                Projectile.ai[1] += 1f;
                int num39 = (int)Projectile.ai[0];
                if (!Main.npc[num39].active || !Main.npc[num39].CanBeChasedBy(Projectile, false))
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.velocity.ToRotation();
                    Vector2 vector6 = Main.npc[num39].Center - Projectile.Center;
                    if (vector6.Length() < 10f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (vector6 != Vector2.Zero)
                    {
                        vector6.Normalize();
                        vector6 *= scaleFactor;
                    }
                    float num40 = 30f;
                    Projectile.velocity = (Projectile.velocity * (num40 - 1f) + vector6) / num40;
                }
            }
            if (Projectile.ai[1] >= 1f && Projectile.ai[1] < num29)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] == num29)
                {
                    Projectile.ai[1] = 1f;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 48f)
            {
                Projectile.localAI[0] = 0f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override bool CanDamage()
        {
            return Projectile.alpha < 128;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 50;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, (int)Projectile.position.X, (int)Projectile.position.Y);
            for (int num621 = 0; num621 < 20; num621++)
            {
                int num622 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 45; num623++)
            {
                int num624 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(Projectile.Center, 9);
        }
    }
}
