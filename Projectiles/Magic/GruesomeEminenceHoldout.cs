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
            Lighting.AddLight(projectile.Center, 0.65f, 0f, 0.1f);

            StickToOwner();
            GhostlyFusableParticleSet.Instance.SpawnParticle(Main.MouseWorld + Main.rand.NextVector2Circular(6f, 6f));

            if (!Owner.channel)
                projectile.Kill();
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
	}
}
