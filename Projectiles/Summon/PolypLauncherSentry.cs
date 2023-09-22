using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class PolypLauncherSentry : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 25;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            Projectile.velocity.Y += 0.5f;
            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }

            Projectile.StickToTiles(false, false);

            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0] -= 1f;
                return;
            }
            Projectile.ai[1] += Main.rand.Next(1,3);

            NPC potentialTarget = Projectile.Center.MinionHoming(800f, player, false);

            if (Projectile.owner == Main.myPlayer && potentialTarget != null)
            {
                if (Projectile.ai[1] > 40f)
                {
                    Vector2 spawnPosition = new Vector2(Projectile.oldPosition.X + (Projectile.width / 2), Projectile.oldPosition.Y + (Projectile.height / 2));

                    float shootSpeed = 16f;
                    float gravity = -PolypLauncherProjectile.Gravity;
                    float distance = Vector2.Distance(spawnPosition, potentialTarget.Center);
                    float angle = 0.25f * (float)Math.Asin(MathHelper.Clamp(gravity * distance * 1.5f / (float)Math.Pow(shootSpeed, 2), -1f, 1f));

                    Vector2 velocity = new Vector2(0f, -shootSpeed).RotatedBy(angle).RotatedByRandom(0.1f);
                    velocity.X *= (potentialTarget.Center.X - Projectile.Center.X < 0).ToDirectionInt();

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, velocity, ModContent.ProjectileType<PolypLauncherProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool? CanDamage() => false;
    }
}
