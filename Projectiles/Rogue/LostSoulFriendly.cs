using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class LostSoulFriendly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lost Soul");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.Calamity().rogue = true;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 150;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override bool? CanHitNPC(NPC target) => (Projectile.timeLeft < 210 || Projectile.ai[0] == 2f) && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0.25f, 0.25f, 0.25f);
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            if (Projectile.ai[0] != 1f && (Projectile.timeLeft < 210 || Projectile.ai[0] == 2f))
                CalamityGlobalProjectile.HomeInOnNPC(Projectile, !Projectile.tileCollide, 600f, 12f, 20f);
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!Projectile.Calamity().rogue)
            {
                int projectileCount = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type];
                int cap = 5;
                int oldDamage = damage;
                if (projectileCount > cap)
                {
                    damage -= (int)(oldDamage * ((projectileCount - cap) * 0.05));
                    if (damage < 1)
                        damage = 1;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int j = 0; j <= 10; j++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 175, 0f, 0f, 100, default, 1f);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(150, 150, 150, 150);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
