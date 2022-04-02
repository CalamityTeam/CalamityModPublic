using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SealedSingularityProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SealedSingularity";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sealed Singularity");
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;

            if (projectile.ai[0] >= 70f)
            {
                projectile.velocity *= 0.96f;
            }
            else
            {
                projectile.rotation += 0.3f * (float)projectile.direction;
            }
        }

        public override void Kill (int timeLeft)
        {
            if (projectile.owner != Main.myPlayer)
                return;

            //glass-pot break sound
            Main.PlaySound(SoundID.Shatter, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);

            int blackhole = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<SealedSingularityBlackhole>(), projectile.damage, projectile.knockBack, projectile.owner, projectile.Calamity().stealthStrike ? -180f : 0f, 0f);
            if (blackhole.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[blackhole].Center = projectile.Center;
                Main.projectile[blackhole].Calamity().stealthStrike = projectile.Calamity().stealthStrike;
            }

            for (int index = 0; index < 3; ++index)
            {
                float SpeedX = -projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                float SpeedY = -projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                Projectile.NewProjectile(projectile.Center.X + SpeedX, projectile.Center.Y + SpeedY, SpeedX, SpeedY, ModContent.ProjectileType<SealedSingularityGore>(), (int)(projectile.damage * 0.25), 0f, projectile.owner, index, 0f);
            }
        }
    }
}
