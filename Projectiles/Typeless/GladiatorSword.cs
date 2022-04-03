using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class GladiatorSword : ModProjectile
    {
        private double rotation = 0D;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gladiator Sword");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.timeLeft *= 5;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            bool flag64 = Projectile.type == ModContent.ProjectileType<GladiatorSword>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.Calamity().spawnedPlayerMinionDamageValue = player.AverageDamage();
                Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
                Projectile.localAI[0] += 1f;
            }
            if (player.AverageDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.AverageDamage());
                Projectile.damage = damage2;
            }

            if (!modPlayer.gladiatorSword)
            {
                Projectile.active = false;
                return;
            }

            if (flag64)
            {
                if (player.dead)
                    modPlayer.glSword = false;
                if (modPlayer.glSword)
                    Projectile.timeLeft = 2;
            }

            Lighting.AddLight(Projectile.Center, 0.15f, 0.15f, 0f);

            Vector2 vector = player.Center - Projectile.Center;
            Projectile.rotation = vector.ToRotation() - MathHelper.PiOver2;

            Projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(rotation);

            rotation += 0.1;
            if (rotation >= 360D)
                rotation = 0D;

            Projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;
        }
    }
}
