using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Rancor>();
        public Player Owner => Main.player[Projectile.owner];
        public ref float Time => ref Projectile.ai[0];
        public const int ManaConsumeRate = 12;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.timeLeft = 90000;
        }

        public override void AI()
        {
            Projectile.Center = Owner.Center + Vector2.UnitX * Owner.direction * 8f;

            // CheckMana returns true if the mana cost can be paid. If mana isn't consumed this frame, the CheckMana short-circuits out of being evaluated.
            bool allowContinuedUse = Time % ManaConsumeRate != ManaConsumeRate - 1f || Owner.CheckMana(Owner.ActiveItem(), -1, true, false);
            bool bookStillInUse = Owner.channel && allowContinuedUse && !Owner.noItems && !Owner.CCed;

            // If the owner is no longer able to hold the book, kill it.
            if (!bookStillInUse)
            {
                Projectile.Kill();
                return;
            }

            Time++;

            // Cast the magic circle on the first frame.
            if (Main.myPlayer == Projectile.owner && Time == 1f)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RancorMagicCircle>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            // Handle frames.
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
                Projectile.frameCounter = 0;
            }

            AdjustPlayerHoldValues();
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void AdjustPlayerHoldValues()
        {
            Projectile.spriteDirection = Projectile.direction = Owner.direction;
            Projectile.timeLeft = 2;
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = 0f;
        }

        public override bool? CanDamage() => false;
    }
}
