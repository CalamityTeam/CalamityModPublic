using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    class SeashineSwordProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.melee = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(127, 255, 253);
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0.25f / 255f);
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 187, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, new Color(255, 255, 255), 1.2f);
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 187, projectile.oldVelocity.X, projectile.oldVelocity.Y, 0, new Color(255, 255, 255), 1.5f);
            }
        }
    }
}
