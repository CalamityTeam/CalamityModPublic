using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class OldDukeHeadCorpse : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 60;
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
            NPC target = Projectile.Center.MinionHoming(845f, player, false);
            // No sense in targeting something below this sentry.
            if (target != null)
            {
                if (target.Bottom.Y > Projectile.Top.Y)
                {
                    target = null;
                }
            }
            Projectile.frame = (target != null).ToInt();
            if (target != null)
            {
                Projectile.ai[0] += 1f;
                if (Main.myPlayer == Projectile.owner &&
                    Projectile.ai[0] % 8f == 0f)
                {
                    float angle = (float)Math.Atan(Math.Abs(target.Center.X - Projectile.Center.X) / 450f);
                    angle *= Math.Sign(target.Center.X - Projectile.Center.X);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Top + Vector2.UnitY * 7f,
                        new Vector2(0f, -Main.rand.NextFloat(21f, 30.5f)).RotatedBy(angle),
                        ModContent.ProjectileType<OldDukeSharkVomit>(), Projectile.damage, 5f,
                        Projectile.owner);
                }
            }
            Projectile.velocity.Y += 0.5f;

            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }

            Projectile.StickToTiles(false, false);
        }

        public override bool? CanDamage() => false;
        // Don't die on tile collision
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
