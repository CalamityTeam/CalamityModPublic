using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class AtlasMunitionsHeldCannon : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public ref float Time => ref Projectile.ai[0];

        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Ultima";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultima");
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 48;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.sentry = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            AttachToPlayer();
            AttemptToFireProjectiles();

            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            Time++;
        }

        public void AttemptToFireProjectiles()
        {
            // Look at the mouse.
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(Main.MouseWorld);

                if (Projectile.velocity != Projectile.oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
        }

        public void AttachToPlayer()
        {
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true) + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.width * Projectile.scale * 0.34f;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        public override bool? CanDamage() => false;
    }
}
