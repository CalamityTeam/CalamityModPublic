using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class GruesomeEminenceHoldout : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public override void SetStaticDefaults() => DisplayName.SetDefault("Gruesome Eminence");

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            StickToOwner();

            // Die if the owner is no longer channeling the item.
            int congregationType = ModContent.ProjectileType<SpiritCongregation>();
            if (!Owner.channel)
                projectile.Kill();

            // Summon a congregation of spirits if the player doesn't have one already.
            else if (Owner.ownedProjectileCounts[congregationType] <= 0)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    Vector2 spiritSpawnPosition = projectile.Center - Vector2.UnitY * 12f;
                    Projectile.NewProjectile(spiritSpawnPosition, -Vector2.UnitY * 10f, congregationType, projectile.damage, projectile.knockBack, projectile.owner);
                }
                Main.PlaySound(SoundID.DD2_EtherianPortalOpen, projectile.Center);
            }

            // Emit light.
            Lighting.AddLight(projectile.Center, 0.13f, 0f, 0.05f);

            // Also emit dust.
            Dust ghostlyMagic = Dust.NewDustPerfect(projectile.Top + Main.rand.NextVector2Circular(5f, 5f) + Vector2.UnitX * projectile.direction * 7f, 267);
            ghostlyMagic.color = Color.Lerp(Color.DarkRed, Color.Fuchsia, Main.rand.NextFloat(0.7f));
            ghostlyMagic.color = Color.Lerp(ghostlyMagic.color, Color.Black, 0.5f);
            ghostlyMagic.velocity = -Vector2.UnitY.RotatedBy(0.44f) * Main.rand.NextFloat(0.8f, 2f);
            ghostlyMagic.noGravity = true;
            if (Main.rand.NextBool(8))
                ghostlyMagic.velocity *= 1.7f;
        }

        public void StickToOwner()
        {
            projectile.rotation = 0f;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            Owner.ChangeDir(projectile.direction);

            Vector2 centerDelta = Main.OffsetsPlayerOnhand[Owner.bodyFrame.Y / 56] * 2f;
            if (Owner.direction != 1)
                centerDelta.X = Owner.bodyFrame.Width - centerDelta.X;

            if (Owner.gravDir != 1f)
                centerDelta.Y = Owner.bodyFrame.Height - centerDelta.Y;

            if (Owner.heldProj == -1)
                Owner.heldProj = projectile.whoAmI;
            centerDelta -= new Vector2(Owner.bodyFrame.Width - Owner.width, Owner.bodyFrame.Height - 42) / 2f;
            projectile.Center = Owner.RotatedRelativePoint(Owner.position + centerDelta, true) - projectile.velocity;
            if (projectile.spriteDirection == 1)
                projectile.position.X += 4f;

            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemLocation = projectile.Center;
            Owner.itemRotation = MathHelper.PiOver4 * -projectile.direction;
        }

        // This is just a casting item. It should not do contact damage.
        public override bool CanDamage() => false;
    }
}
