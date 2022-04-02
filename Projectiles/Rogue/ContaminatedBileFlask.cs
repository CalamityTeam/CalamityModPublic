using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ContaminatedBileFlask : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ContaminatedBile";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Contaminated Bile");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.tileCollide = true;
            projectile.alpha = 0;
            projectile.Calamity().rogue = true;
        }
        public override void AI()
        {
            if (projectile.ai[0]++ > 45f)
            {
                if (projectile.velocity.Y < 10f)
                {
                    projectile.velocity.Y += 0.15f;
                }
            }
            projectile.rotation += MathHelper.ToRadians(projectile.velocity.Length());
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item107, projectile.Bottom);
            Projectile explosion = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BileExplosion>(), (int)(projectile.damage * 0.75), projectile.knockBack, projectile.owner);
            if (explosion.whoAmI.WithinBounds(Main.maxProjectiles))
            {
                explosion.Calamity().stealthStrike = projectile.Calamity().stealthStrike;
                explosion.timeLeft = explosion.Calamity().stealthStrike ? 60 : 20;
            }
        }
    }
}
