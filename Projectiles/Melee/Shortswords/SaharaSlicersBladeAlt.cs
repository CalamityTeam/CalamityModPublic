using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Humanizer.In;

namespace CalamityMod.Projectiles.Melee.Shortswords
{
    public class SaharaSlicersBladeAlt: BaseShortswordProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<SaharaSlicers>();
        public override string Texture => "CalamityMod/Projectiles/Melee/SaharaSlicersBladeAlt";
        public ref int Bolts => ref Main.player[Projectile.owner].Calamity().saharaSlicersBolts;
        public override float FadeInDuration => 8f;
        public override float FadeOutDuration => 0f;
        public override float TotalDuration => 12f;
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(15);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
        }

        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), 288);
                dust.scale = Main.rand.NextFloat(0.15f, 0.6f);
                dust.noGravity = true;
                dust.velocity = -Projectile.velocity * 0.5f;
            }

            float armPointingDirection = (Owner.Calamity().mouseWorld - Owner.MountedCenter).ToRotation();
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 0);
            Owner.heldProj = Projectile.whoAmI;

            Projectile.Center = Projectile.Center + new Vector2(0, -6f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Add to bolt counter, spawn 2 quiver bolts, and on hit visual
            if (Bolts < 10)
            {
                Bolts++;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero, ModContent.ProjectileType<SaharaSlicersBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Bolts);
                if (Bolts < 10)
                {
                    Bolts++;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero, ModContent.ProjectileType<SaharaSlicersBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Bolts);
                }
            }
            float numberOfDusts = 5f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(Main.rand.NextFloat(0.5f, 2.5f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Vector2 velOffset = new Vector2(Main.rand.NextFloat(0.5f, 2.5f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Dust dust = Dust.NewDustPerfect(target.Center + offset, Main.rand.NextBool() ? 288 : 207, new Vector2(velOffset.X, velOffset.Y));
                dust.noGravity = false;
                dust.velocity = velOffset;
                dust.scale = Main.rand.NextFloat(1.5f, 1.2f);
            }
        }
    }
}
