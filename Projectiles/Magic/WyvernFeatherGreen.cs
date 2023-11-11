using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class WyvernFeatherGreen : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = ProjAIStyleID.Nail;
            AIType = ProjectileID.NailFriendly;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(6))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 61, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            for (int i = 0; i < 15; i++)
            {
                int greenDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 61, 0f, 0f, 100, default, 1.2f);
                Main.dust[greenDust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[greenDust].scale = 0.5f;
                    Main.dust[greenDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 30; j++)
            {
                int greenDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 61, 0f, 0f, 100, default, 1.7f);
                Main.dust[greenDust2].noGravity = true;
                Main.dust[greenDust2].velocity *= 5f;
                greenDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 61, 0f, 0f, 100, default, 1f);
                Main.dust[greenDust2].velocity *= 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
