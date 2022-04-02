using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
	public class RancorHoldout : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float Time => ref projectile.ai[0];
        public const int ManaConsumeRate = 12;
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
            projectile.timeLeft = 90000;
        }

        public override void AI()
        {
            projectile.Center = Owner.Center + Vector2.UnitX * Owner.direction * 8f;

            // CheckMana returns true if the mana cost can be paid. If mana isn't consumed this frame, the CheckMana short-circuits out of being evaluated.
            bool allowContinuedUse = Time % ManaConsumeRate != ManaConsumeRate - 1f || Owner.CheckMana(Owner.ActiveItem(), -1, true, false);
            bool bookStillInUse = Owner.channel && allowContinuedUse && !Owner.noItems && !Owner.CCed;

            // If the owner is no longer able to hold the book, kill it.
            if (!bookStillInUse)
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public void AdjustPlayerHoldValues()
        {
            projectile.spriteDirection = projectile.direction = Owner.direction;
            projectile.timeLeft = 2;
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = 0f;
        }

        public override bool CanDamage() => false;
    }
}
