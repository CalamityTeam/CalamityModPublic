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
    public class GodKiller : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override void SetStaticDefaults()
        {
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
                    for (int i = 0; i < 2; i++)
                    {
                        float shortXVel = 0f;
                        float shortYVel = 0f;
                        if (i == 1)
                        {
                            shortXVel = Projectile.velocity.X * 0.5f;
                            shortYVel = Projectile.velocity.Y * 0.5f;
                        }
                        int dusting = Dust.NewDust(new Vector2(Projectile.position.X + 3f + shortXVel, Projectile.position.Y + 3f + shortYVel) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 173, 0f, 0f, 100, default, 1f);
                        Main.dust[dusting].scale *= 1f + (float)Main.rand.Next(5) * 0.1f;
                        Main.dust[dusting].velocity *= 0.2f;
                        Main.dust[dusting].noGravity = true;
                        dusting = Dust.NewDust(new Vector2(Projectile.position.X + 3f + shortXVel, Projectile.position.Y + 3f + shortYVel) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 199, 0f, 0f, 100, default, 0.1f);
                        Main.dust[dusting].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                        Main.dust[dusting].velocity *= 0.05f;
                    }
                }
            }
            CalamityUtils.HomeInOnNPC(Projectile, true, 300f, 12f, 20f);
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 origin = new Vector2(11f, 23f);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GodKillerGlow").Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            for (int j = 0; j < 5; j++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 199, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int k = 0; k < 10; k++)
            {
                int dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust2].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
