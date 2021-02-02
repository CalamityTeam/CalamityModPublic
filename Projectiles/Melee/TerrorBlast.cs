using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class TerrorBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terror Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 2;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;

            // This projectile can only hit when it explodes, so these values aren't a problem.
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.melee = true;
        }

        // Terror Blasts do nothing until they explode.
        public override bool CanDamage() => projectile.localAI[0] > 0f;

        public override void Kill(int timeLeft)
        {
            // Explode on death, becoming an enormous hitbox and spawning a ton of dust.
            Main.PlaySound(SoundID.Item60, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 400;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);

            for (int i = 0; i < 6; i++)
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1.5f);

            for (int i = 0; i < 60; i++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }

            // Guarantee a hit on all nearby enemies when the projectile explodes. Changing localAI[0] enables it to hit.
            projectile.localAI[0] = 1f;
            projectile.Damage();
        }
    }
}
