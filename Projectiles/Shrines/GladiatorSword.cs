using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class GladiatorSword : ModProjectile
    {
        private double rotation = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gladiator Sword");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.timeLeft *= 5;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            bool flag64 = projectile.type == ModContent.ProjectileType<GladiatorSword>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.gladiatorSword)
            {
                projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.glSword = false;
                }
                if (modPlayer.glSword)
                {
                    projectile.timeLeft = 2;
                }
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.01f / 255f);
            Vector2 vector = player.Center - projectile.Center;
            projectile.rotation = vector.ToRotation() - 1.57f;
            projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(rotation);
            rotation += 0.03;
            if (rotation >= 360)
            {
                rotation = 0;
            }
            projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;
        }
    }
}
