using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.Projectiles.Magic
{
    public class HeresyProj : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Heresy>();
        public Player Owner => Main.player[Projectile.owner];
        public float ShootIntensity => MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(0f, 275f, Time, true));
        public ref float Time => ref Projectile.ai[0];
        public ref float AttackTimer => ref Projectile.ai[1];
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
            // If the player is no longer able to hold the book, kill it.
            if (!Owner.channel || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            if (AttackTimer >= Main.rand.Next(20, 28) - (int)MathHelper.Lerp(0f, 16f, Utils.GetLerpValue(0f, 120f, Time, true)))
                ReleaseThings();

            // Switch frames at a linearly increasing rate to make it look like the player is flipping pages quickly.
            Projectile.localAI[0] += Utils.Remap(Time, 0f, 180f, 1f, 5f);
            Projectile.frame = (int)Math.Round(Projectile.localAI[0] / 10f) % Main.projFrames[Projectile.type];
            if (Projectile.localAI[0] >= Main.projFrames[Projectile.type] * 10f)
                Projectile.localAI[0] = 0f;

            AdjustPlayerValues();
            Projectile.Center = Owner.Center + (Owner.compositeFrontArm.rotation + MathHelper.PiOver2).ToRotationVector2() * 14f;
            Projectile.timeLeft = 2;
            AttackTimer++;
            Time++;
        }

        public void ReleaseThings()
        {
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy);
            if (Main.myPlayer != Projectile.owner)
                return;

            // If the owner has sufficient mana, consume it.
            // Otherwise, delete the book and don't bother summoning anything.
            if (!Owner.CheckMana(Owner.ActiveItem().mana, true, false))
            {
                Projectile.Kill();
                return;
            }
            WeightedRandom<int> typeDecider = new WeightedRandom<int>();

            typeDecider.Add(ModContent.ProjectileType<RedirectingFire>(), 1.5f);

            // Make souls appear more frequently (aka with a higher weight) the more "intense" the shots should be.
            typeDecider.Add(ModContent.ProjectileType<RedirectingLostSoul>(), ShootIntensity * 0.75f);
            typeDecider.Add(ModContent.ProjectileType<RedirectingVengefulSoul>(), ShootIntensity * 0.4f);
            typeDecider.Add(ModContent.ProjectileType<RedirectingGildedSoul>(), ShootIntensity * 0.2f);

            Vector2 spawnPosition = Projectile.Top + Main.rand.NextVector2CircularEdge(4f, 4f);
            Vector2 shootVelocity = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.13f, 0.23f) * Owner.direction) * Owner.gravDir;
            shootVelocity *= Main.rand.NextFloat(5f, 8);

            if (Owner.velocity.Y < 0f)
                shootVelocity.Y += Owner.velocity.Y;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, shootVelocity, typeDecider.Get(), Projectile.damage, Projectile.knockBack, Projectile.owner, ShootIntensity);

            AttackTimer = 0f;
            Projectile.netUpdate = true;
        }

        public void AdjustPlayerValues()
        {
            Projectile.spriteDirection = Projectile.direction = Owner.direction;
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.direction * Projectile.velocity).ToRotation();

            // Update the player's arm directions to make it look as though they're flipping through the book.
            float frontArmRotation = (MathHelper.PiOver2 - 0.46f) * -Owner.direction;
            float backArmRotation = frontArmRotation + MathHelper.Lerp(0.23f, 0.97f, CalamityUtils.Convert01To010(Projectile.localAI[0] / Main.projFrames[Projectile.type] / 10f)) * -Owner.direction;
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, backArmRotation);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, frontArmRotation);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float glowOutwardness = MathHelper.SmoothStep(0f, 4f, Utils.GetLerpValue(90f, 270f, Time, true));
            Texture2D bookTexture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = bookTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPosition;
            Vector2 origin = frame.Size() * 0.5f;
            Color glowColor = Color.Lerp(Color.Pink, Color.Red, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 5f) * 0.5f + 0.5f);
            glowColor.A = 0;

            // Draw an ominous glowing version of the book after a bit of time.
            for (int i = 0; i < 8; i++)
            {
                drawPosition = Projectile.Center + (MathHelper.TwoPi * i / 8f + Main.GlobalTimeWrappedHourly * 4f).ToRotationVector2() * glowOutwardness - Main.screenPosition;
                Main.EntitySpriteDraw(bookTexture, drawPosition, frame, Projectile.GetAlpha(glowColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(bookTexture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
