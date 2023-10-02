using CalamityMod.Items.Weapons.Magic;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityBook : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Eternity>();
        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }

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
            Player player = Main.player[Projectile.owner];

            // If the player is no longer able to hold the book, kill it (and by extension the other projectiles).
            if (!player.channel || player.noItems || player.CCed)
            {
                Projectile.Kill();
                return;
            }

            Time++;

            // Summon a bunch of cool things the moment the book is created, assuming an NPC is near the mouse position.
            if (Main.myPlayer == Projectile.owner && Time == 1f)
            {
                NPC target = Main.MouseWorld.ClosestNPCAt(4400f, true, true);

                if (target != null)
                {
                    SoundEngine.PlaySound(Providence.HolyRaySound);
                    SummonProjectilesOnTarget(target, player);
                }
            }

            // Switch frames at a linearly increasing rate to make it look like the player is flipping pages quickly.
            Projectile.localAI[0] += Utils.Remap(Time, 0f, 200f, 1f, 5f);
            Projectile.frame = (int)Math.Round(Projectile.localAI[0] / 10f) % Main.projFrames[Projectile.type];
            if (Projectile.localAI[0] >= Main.projFrames[Projectile.type] * 10f)
                Projectile.localAI[0] = 0f;

            AdjustPlayerValues(player);
            Projectile.Center = player.Center + (player.compositeFrontArm.rotation + MathHelper.PiOver2).ToRotationVector2() * 14f;
        }

        public void AdjustPlayerValues(Player player)
        {
            Projectile.spriteDirection = Projectile.direction = player.direction;
            Projectile.timeLeft = 2;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (Projectile.direction * Projectile.velocity).ToRotation();

            // Update the player's arm directions to make it look as though they're flipping through the book.
            float frontArmRotation = (MathHelper.PiOver2 - 0.46f) * -player.direction;
            float backArmRotation = frontArmRotation + MathHelper.Lerp(0.12f, 1.1f, CalamityUtils.Convert01To010(Projectile.localAI[0] / Main.projFrames[Projectile.type] / 10f)) * -player.direction;
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, backArmRotation);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, frontArmRotation);
        }

        public void SummonProjectilesOnTarget(NPC target, Player owner)
        {
            Projectile hex = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<EternityHex>(), Projectile.damage, 0f, owner.whoAmI, target.whoAmI);
            hex.localAI[1] = Projectile.whoAmI;

            for (int i = 0; i < 5; i++)
            {
                float crystalAngleOffset = MathHelper.TwoPi / 5f * i;
                Projectile crystal = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<EternityCrystal>(), 0, 0f, owner.whoAmI, target.whoAmI, crystalAngleOffset);
                crystal.frame = i % 2;
                crystal.localAI[1] = Projectile.whoAmI;
            }
            for (int i = 0; i < 10; i++)
            {
                float circleOffset = MathHelper.TwoPi / 10f * i;
                Projectile circleSpell = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<EternityCircle>(), 0, 0f, owner.whoAmI, target.whoAmI, circleOffset);
                circleSpell.localAI[1] = Projectile.whoAmI;
            }
        }

        public override bool? CanDamage() => false;
    }
}
