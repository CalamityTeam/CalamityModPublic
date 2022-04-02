using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class OmnibladeSwing : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omniblade");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 246;
            projectile.height = 184;
            projectile.scale = 1.15f;

            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ownerHitCheck = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 3;
            projectile.Calamity().trueMelee = true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 3;
            if (projectile.frame >= Main.projFrames[projectile.type])
                projectile.Kill();

            Vector2 playerRotatedPoint = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            if (Main.myPlayer == projectile.owner)
            {
                if (Owner.channel && !Owner.noItems && !Owner.CCed)
                    HandleChannelMovement(playerRotatedPoint);
                else
                    projectile.Kill();
            }

            // Rotation and directioning.
            projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();

            // Sprite and player directioning.
            projectile.spriteDirection = -projectile.direction;
            if (projectile.direction == 1)
                projectile.Left = Owner.Center;
            else
                projectile.Right = Owner.Center;
            projectile.position.X += projectile.spriteDirection == -1 ? -116f : 88f;
            projectile.position.Y -= projectile.scale * 66f;
            Owner.ChangeDir(projectile.direction);

            // Prevents the projectile from dying
            projectile.timeLeft = 2;

            // Player item-based field manipulation.
            Owner.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public void HandleChannelMovement(Vector2 playerRotatedPoint)
        {
            Vector2 newVelocity = Vector2.UnitX * (Main.MouseWorld.X > playerRotatedPoint.X).ToDirectionInt();

            // Sync if a velocity component changes.
            if (projectile.velocity.X != newVelocity.X || projectile.velocity.Y != newVelocity.Y)
                projectile.netUpdate = true;

            projectile.velocity = newVelocity;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 300);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 300);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 170);

        // Don't suffer from the same issues Murasama did in the past; encouraging people to kill their wrists for some extra DPS is bad lmao
        public override bool CanDamage() => projectile.frameCounter > 6;
    }
}
