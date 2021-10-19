using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorHoldout : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rancor");
            Main.projFrames[projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 34;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.hide = true;
        }

        public override void AI()
        {
            projectile.Center = Owner.Center;

            // If the owner is no longer able to hold the book, kill it.
            if (!Owner.channel || Owner.noItems || Owner.CCed)
            {
                projectile.Kill();
                return;
            }

            Time++;

            // Cast the magic circle on the first frame.
            if (Main.myPlayer == projectile.owner && Time == 1f)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<RancorMagicCircle>(), projectile.damage, projectile.knockBack, projectile.owner);

            // Handle frames.
            projectile.frameCounter++;
            if (projectile.frameCounter >= 4)
            {
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
                projectile.frameCounter = 0;
            }

            AdjustPlayerHoldValues();
        }

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
            drawCacheProjsOverWiresUI.Add(index);
        }

		public void AdjustPlayerHoldValues()
        {
            projectile.spriteDirection = projectile.direction = Owner.direction;
            projectile.timeLeft = 2;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = 0f;
        }

        public override bool CanDamage() => false;
    }
}
