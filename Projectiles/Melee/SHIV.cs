using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class SHIV : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shiv");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = 27;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < 25f && Projectile.timeLeft % 2 == 0)
                Projectile.velocity *= 1.04f;

            int rainbow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 66, (float)(Projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
            Main.dust[rainbow].noGravity = true;
            Main.dust[rainbow].velocity = Vector2.Zero;
        }

        public override void Kill(int timeLeft)
        {
            int rainbow = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 66, (float)(Projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
            Main.dust[rainbow].noGravity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
