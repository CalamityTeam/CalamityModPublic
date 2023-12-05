using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class LunarKunaiProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        bool lunarEnhance = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            CalamityPlayer modPlayer = Main.player[Projectile.owner].Calamity();
            Projectile.ai[0] += 1f;
            if(Projectile.ai[0] == 1f && modPlayer.StealthStrikeAvailable())
                lunarEnhance = true;
            else if (Projectile.ai[0] >= 50f)
                lunarEnhance = true;

            if (lunarEnhance)
                Projectile.frame = 1;
            else
                Projectile.frame = 0;

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, lunarEnhance ? 300f : 150f, lunarEnhance ? 12f : 8f, 20f);
            if (Main.rand.Next(6) == 0 && lunarEnhance)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 229, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override void OnKill(int timeLeft)
        {
            if (lunarEnhance)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 28;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.damage /= 5;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 229, 0f, 0f, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 3f;
                    dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 229, 0f, 0f, 100, default, 1f);
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].noGravity = true;
                }
                Projectile.Damage();
            }
            else
            {
                for (int j = 0; j < 5; j++)
                {
                    int dusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 265, 0f, 0f, 100, default, 1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].velocity *= 1.2f;
                    Main.dust[dusty].velocity -= Projectile.oldVelocity * 0.3f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
