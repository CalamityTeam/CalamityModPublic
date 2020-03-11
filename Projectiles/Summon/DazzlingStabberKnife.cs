using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class DazzlingStabberKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.White.ToVector3());
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            NPC potentialTarget = projectile.Center.MinionHoming(750f, Main.player[projectile.owner]);
            if (potentialTarget != null)
            {
                if (projectile.Distance(potentialTarget.Center) > 70f)
                {
                    projectile.velocity = (projectile.velocity * 19f + projectile.DirectionTo(potentialTarget.Center) * 17f) / 20f;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }
    }
}
