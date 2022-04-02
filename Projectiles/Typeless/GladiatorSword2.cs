using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class GladiatorSword2 : ModProjectile
    {
        private double rotation = 0D;

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
            bool flag64 = projectile.type == ModContent.ProjectileType<GladiatorSword2>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.AverageDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.AverageDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.AverageDamage());
                projectile.damage = damage2;
            }

            if (!modPlayer.gladiatorSword)
            {
                projectile.active = false;
                return;
            }

            if (flag64)
            {
                if (player.dead)
                    modPlayer.glSword = false;
                if (modPlayer.glSword)
                    projectile.timeLeft = 2;
            }

            Lighting.AddLight(projectile.Center, 0.15f, 0.15f, 0f);

            Vector2 vector = player.Center - projectile.Center;
            projectile.rotation = vector.ToRotation() - MathHelper.PiOver2;

            projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(rotation);

            // Values are slightly different from the other sword to make this sword marginally slower so the intersection point isn't always at the same spot
            rotation -= 0.09;
            if (rotation <= 0D)
                rotation = 360D;

            projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;
        }
    }
}
