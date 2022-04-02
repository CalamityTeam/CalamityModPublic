using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EventHorizonBlackhole : ModProjectile
    {
        public int killCounter = 21;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blackhole");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 90;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 60;

        public override void AI()
        {
            if (projectile.frame == 8)
                return;

            // Update animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }

            if (projectile.timeLeft > 15)
            {
                if (projectile.frame >= 4)
                    projectile.frame = 0;
            }
            else
            {
                if (projectile.frame < 4)
                    projectile.frame = 4;
                if (projectile.frame >= 8)
                    projectile.frame = 4;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
