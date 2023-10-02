using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class GruesomeEminenceHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<GruesomeEminence>();
        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            StickToOwner();

            // Die if the owner is no longer channeling the item.
            int congregationType = ModContent.ProjectileType<SpiritCongregation>();
            if (!Owner.channel)
                Projectile.Kill();

            // Summon a congregation of spirits if the player doesn't have one already.
            else if (Owner.ownedProjectileCounts[congregationType] <= 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 spiritSpawnPosition = Projectile.Center - Vector2.UnitY * 12f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spiritSpawnPosition, -Vector2.UnitY * 10f, congregationType, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.Center);
            }

            // Emit light.
            Lighting.AddLight(Projectile.Center, 0.13f, 0f, 0.05f);

            // Also emit dust.
            Dust ghostlyMagic = Dust.NewDustPerfect(Projectile.Top + Main.rand.NextVector2Circular(5f, 5f) + Vector2.UnitX * Projectile.direction * 7f, 267);
            ghostlyMagic.color = Color.Lerp(Color.DarkRed, Color.Fuchsia, Main.rand.NextFloat(0.7f));
            ghostlyMagic.color = Color.Lerp(ghostlyMagic.color, Color.Black, 0.5f);
            ghostlyMagic.velocity = -Vector2.UnitY.RotatedBy(0.44f) * Main.rand.NextFloat(0.8f, 2f);
            ghostlyMagic.noGravity = true;
            if (Main.rand.NextBool(8))
                ghostlyMagic.velocity *= 1.7f;
        }

        public void StickToOwner()
        {
            Projectile.rotation = 0f;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            Owner.ChangeDir(Projectile.direction);

            Vector2 centerDelta = Main.OffsetsPlayerOnhand[Owner.bodyFrame.Y / 56] * 2f;
            if (Owner.direction != 1)
                centerDelta.X = Owner.bodyFrame.Width - centerDelta.X;

            if (Owner.gravDir != 1f)
                centerDelta.Y = Owner.bodyFrame.Height - centerDelta.Y;

            if (Owner.heldProj == -1)
                Owner.heldProj = Projectile.whoAmI;
            centerDelta -= new Vector2(Owner.bodyFrame.Width - Owner.width, Owner.bodyFrame.Height - 42) / 2f;
            Projectile.Center = Owner.RotatedRelativePoint(Owner.position + centerDelta, true) - Projectile.velocity;
            if (Projectile.spriteDirection == 1)
                Projectile.position.X += 4f;

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemLocation = Projectile.Center;
            Owner.itemRotation = MathHelper.PiOver4 * -Projectile.direction;
        }

        // This is just a casting item. It should not do contact damage.
        public override bool? CanDamage() => false;
    }
}
