using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeraphimAngelicLight : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Orb");
            Main.projFrames[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            projectile.width = 82;
            projectile.height = 82;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 10;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.scale = CalamityUtils.Convert01To010(projectile.timeLeft / 10f);
            projectile.Opacity = (float)Math.Sqrt(projectile.scale);
            projectile.velocity *= 0.8f;
            projectile.frame = Main.projFrames[projectile.type] - projectile.timeLeft;
        }

        public override void Kill(int timeLeft)
        {
            // Release a puff of golden light dust.
            for (int i = 0; i < 15; i++)
            {
                Dust light = Dust.NewDustPerfect(projectile.Center, 267);
                light.color = Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat(0.5f, 1f));
                light.velocity = (MathHelper.TwoPi * i / 16f).ToRotationVector2() * 5f;
                light.scale = 1.35f;
                light.noGravity = true;
            }

            // Release a laser at the nearest target, if one exists.
            NPC potentialTarget = projectile.Center.ClosestNPCAt(1300f, true, true);
            if (Main.myPlayer != projectile.owner || potentialTarget is null)
                return;

            int damage = projectile.damage;
            Vector2 laserDirection = projectile.SafeDirectionTo(potentialTarget.Center);
            Projectile.NewProjectile(projectile.Center, laserDirection, ModContent.ProjectileType<SeraphimBeamLarge>(), damage, 0f, projectile.owner);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255 - projectile.alpha);

        public override bool CanDamage() => false;
    }
}
