using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class HerringMinion : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public ref float MinionOrigin => ref Projectile.ai[0];

        public ref float HerringPosition => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Herring");
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;

            Projectile.width = 42;
            Projectile.height = 14;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.minion = true;
        }

        public override void AI()
        {
            NPC target = Projectile.Center.MinionHoming(1200f, Owner); // Detects a target.
            Projectile minionAI = Main.projectile[(int)MinionOrigin]; // Gets the invisible minion.

            if (target is not null)
                TargetPosition(minionAI); // The minions position themselves near the invisible minion to seem like they're ramming all together.
            else
                FollowOrigin(minionAI); // Vibes around their respective invisible minion.
            
            CheckMinionExistance(); // Checks if the decorative herring can still exist, if the invisible minion despawns, so do these ones.
            DoAnimation(); // Does the animation of the minion.
            PointInRightDirection(minionAI, target); // Points in the correct direction.
            Projectile.MinionAntiClump(); // Prevents the minion from cluttering on one spot.

            Projectile.netUpdate = true;
        }

        #region Methods

        public void CheckMinionExistance()
        {
            Projectile minionAI = Main.projectile[(int)MinionOrigin];

            if (MinionOrigin < 0 || MinionOrigin >= Main.projectile.Length)
            {
                Projectile.Kill();
                return;
            }

            if (!minionAI.active || minionAI.type != ModContent.ProjectileType<HerringAI>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
        }

        public void DoAnimation()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 8 % Main.projFrames[Projectile.type];
        }

        public void PointInRightDirection(Projectile origin, NPC target)
        {
            if (target is not null)
            {
                Projectile.rotation = origin.rotation;
                Projectile.spriteDirection = origin.spriteDirection;
                // Because when there's a target, the herrings act like the invisible minion, we call it's direction and rotation so that it has a proper look.
            }
            else
            {
                Projectile.rotation = (Projectile.spriteDirection == -1) ? Projectile.velocity.ToRotation() + MathHelper.Pi : Projectile.velocity.ToRotation();
                Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
                // If there isn't a target, just look at where it's going.
            }
        }

        public void FollowOrigin(Projectile origin)
        {
            if (Projectile.WithinRange(origin.Center, 1200f) && !Projectile.WithinRange(origin.Center, 300f)) // If the minion starts to get far, force the minion to go to you.
                Projectile.velocity = (origin.Center - Projectile.Center) / 30f;
            else if (!Projectile.WithinRange(origin.Center, 160f)) // The minion will change directions to you if it's going away from you, meaning it'll just hover around you.
                Projectile.velocity = (Projectile.velocity * 37f + Projectile.SafeDirectionTo(origin.Center) * 17f) / 40f;

            // Teleport to the owner if sufficiently far away.
            if (!Projectile.WithinRange(origin.Center, 1200f))
            {
                Projectile.position = origin.Center;
                Projectile.velocity *= 0.3f;
            }
        }

        public void TargetPosition(Projectile origin)
        {
            Projectile.Center = Vector2.Lerp(Projectile.Center, origin.Center + HerringPosition.ToRotationVector2() * 20f, 0.2f);
            Projectile.velocity = Vector2.Zero;
            // We set the herrings to their respective offset from the invisible minion's position, and because it's following it's position, we don't give the herring velocity.

            int trailDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Water, -Projectile.velocity.X, -Projectile.velocity.Y);
            Main.dust[trailDust].noGravity = true;
            Main.dust[trailDust].customData = false;
            // When ramming, tries to give the herrings a trail of water, as if they were going fast.
        }

        #endregion
    }
}
