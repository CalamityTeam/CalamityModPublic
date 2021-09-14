using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class GruesomeEminenceHoldout : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gruesome Eminence");
            Main.projFrames[projectile.type] = 4;
        }

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
            Lighting.AddLight(projectile.Center, 0.65f, 0f, 0.1f);
        }

        public void StickToOwner()
		{
            projectile.position = Owner.RotatedRelativePoint(Owner.MountedCenter, true) - projectile.Size / 2f;
            projectile.rotation = 0f;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            Owner.ChangeDir(projectile.direction);
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = 0f;
        }

        // This is just a casting item. It should not do contact damage.
        public override bool CanDamage() => false;
	}
}
