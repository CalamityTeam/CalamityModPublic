using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PhantomLanceProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/PhantomLance";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Lance");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void AI()
        {
            if (!projectile.Calamity().stealthStrike)
            {
                if (projectile.timeLeft <= 255)
                    projectile.alpha += 1;
                if (projectile.timeLeft >= 75)
                {
                    projectile.velocity.X *= 0.995f;
                    projectile.velocity.Y *= 0.995f;
                }
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 175, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default, 0.85f);
            projectile.ai[0]++;
            if (projectile.ai[0] % 18f == 0f)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    float damageMult = projectile.timeLeft * 0.7f / 300f;
                    if (projectile.Calamity().stealthStrike)
                        damageMult = 0.7f;
                    int soulDamage = (int)(projectile.damage * damageMult);
                    int soul = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<Phantom>(), soulDamage, projectile.knockBack, projectile.owner);
                    if (soul.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[soul].Calamity().forceRogue = true;
                        Main.projectile[soul].usesLocalNPCImmunity = true;
                        Main.projectile[soul].localNPCHitCooldown = -2;
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 175, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
