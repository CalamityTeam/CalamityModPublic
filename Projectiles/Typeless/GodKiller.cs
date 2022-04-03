using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Typeless
{
    public class GodKiller : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Killer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 5;

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                Projectile.ai[1] = 0f;
                Projectile.alpha = 255;
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 100;
                Projectile.height = 100;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.knockBack = 5f;
            }
            else
            {
                if (Math.Abs(Projectile.velocity.X) >= 2f || Math.Abs(Projectile.velocity.Y) >= 2f)
                {
                    for (int num246 = 0; num246 < 2; num246++)
                    {
                        float num247 = 0f;
                        float num248 = 0f;
                        if (num246 == 1)
                        {
                            num247 = Projectile.velocity.X * 0.5f;
                            num248 = Projectile.velocity.Y * 0.5f;
                        }
                        int num249 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num247, Projectile.position.Y + 3f + num248) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 173, 0f, 0f, 100, default, 1f);
                        Main.dust[num249].scale *= 1f + (float)Main.rand.Next(5) * 0.1f;
                        Main.dust[num249].velocity *= 0.2f;
                        Main.dust[num249].noGravity = true;
                        num249 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num247, Projectile.position.Y + 3f + num248) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 199, 0f, 0f, 100, default, 0.1f);
                        Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                        Main.dust[num249].velocity *= 0.05f;
                    }
                }
            }
            CalamityGlobalProjectile.HomeInOnNPC(Projectile, true, 300f, 12f, 20f);
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 origin = new Vector2(11f, 23f);
            spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GodKillerGlow"), Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 199, 0f, 0f, 100, default, 1.5f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 10; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 1.5f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
