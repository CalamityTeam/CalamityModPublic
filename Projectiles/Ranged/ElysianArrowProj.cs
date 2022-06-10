using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class ElysianArrowProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Ammo/ElysianArrow";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = 1;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 32;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int num621 = 0; num621 < 2; num621++)
            {
                int num622 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 6; num623++)
            {
                int num624 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    Mod mod = ModContent.GetInstance<CalamityMod>();
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }

            float x = Projectile.position.X + Main.rand.Next(-400, 400);
            float y = Projectile.position.Y - Main.rand.Next(500, 800);
            Vector2 vector = new Vector2(x, y);
            float num15 = Projectile.position.X + (Projectile.width / 2) - vector.X;
            float num16 = Projectile.position.Y + (Projectile.height / 2) - vector.Y;
            num15 += Main.rand.Next(-100, 101);
            int num17 = 25;
            float num18 = (float)Math.Sqrt(num15 * num15 + num16 * num16);
            num18 = num17 / num18;
            num15 *= num18;
            num16 *= num18;
            if (Projectile.owner == Main.myPlayer)
            {
                int num19 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), x, y, num15, num16, ModContent.ProjectileType<SkyFlareFriendly>(), Projectile.damage / 2, 5f, Projectile.owner, 0f, 0f);
                Main.projectile[num19].ai[1] = Projectile.position.Y;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
