using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class LifeScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 62;
            projectile.height = 72;
            projectile.aiStyle = 18;
            projectile.alpha = 55;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 240;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            aiType = ProjectileID.DeathSickle;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
        }

        public override void AI()
        {
            float num395 = Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            projectile.scale = num395 + 0.95f;

            Lighting.AddLight(projectile.Center, 0.1f, 0.5f, 0.15f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
