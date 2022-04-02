using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.Projectiles.Magic
{
    public class HeresyProj : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public float ShootIntensity => MathHelper.SmoothStep(0f, 1f, Utils.InverseLerp(0f, 275f, Time, true));
        public ref float Time => ref projectile.ai[0];
        public ref float AttackTimer => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heresy");
            Main.projFrames[projectile.type] = 8;
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
            projectile.Center = Owner.Center + Vector2.UnitX * Owner.direction * 18f;

            // If the player is no longer able to hold the book, kill it.
            if (!Owner.channel || Owner.noItems || Owner.CCed)
            {
                projectile.Kill();
                return;
            }

            if (AttackTimer >= Main.rand.Next(20, 28) - (int)MathHelper.Lerp(0f, 16f, Utils.InverseLerp(0f, 120f, Time, true)))
                ReleaseThings();

            // Switch frames at a linearly increasing rate to make it look like the player is flipping pages quickly.
            projectile.frameCounter++;
            if (projectile.frameCounter >= (int)MathHelper.Lerp(10f, 2f, Utils.InverseLerp(0f, 180f, Time, true)))
            {
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
                projectile.frameCounter = 0;
            }

            AdjustPlayerValues();
            projectile.timeLeft = 2;
            AttackTimer++;
            Time++;
        }

        public void ReleaseThings()
        {
            Main.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy);
            if (Main.myPlayer != projectile.owner)
                return;

            // If the owner has sufficient mana, consume it.
            // Otherwise, delete the book and don't bother summoning anything.
            if (!Owner.CheckMana(Owner.ActiveItem().mana, true, false))
            {
                projectile.Kill();
                return;
            }
            WeightedRandom<int> typeDecider = new WeightedRandom<int>();

            typeDecider.Add(ModContent.ProjectileType<RedirectingFire>(), 1.5f);

            // Make souls appear more frequently (aka with a higher weight) the more "intense" the shots should be.
            typeDecider.Add(ModContent.ProjectileType<RedirectingLostSoul>(), ShootIntensity * 0.75f);
            typeDecider.Add(ModContent.ProjectileType<RedirectingVengefulSoul>(), ShootIntensity * 0.4f);
            typeDecider.Add(ModContent.ProjectileType<RedirectingGildedSoul>(), ShootIntensity * 0.2f);

            Vector2 spawnPosition = projectile.Top + Main.rand.NextVector2CircularEdge(4f, 4f);
            Vector2 shootVelocity = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.13f, 0.23f) * Owner.direction) * Owner.gravDir;
            shootVelocity *= Main.rand.NextFloat(5f, 8);

            if (Owner.velocity.Y < 0f)
                shootVelocity.Y += Owner.velocity.Y;

            Projectile.NewProjectile(spawnPosition, shootVelocity, typeDecider.Get(), projectile.damage, projectile.knockBack, projectile.owner, ShootIntensity);

            AttackTimer = 0f;
            projectile.netUpdate = true;
        }

        public void AdjustPlayerValues()
        {
            projectile.spriteDirection = projectile.direction = Owner.direction;
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (projectile.direction * projectile.velocity).ToRotation();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float glowOutwardness = MathHelper.SmoothStep(0f, 4f, Utils.InverseLerp(90f, 270f, Time, true));
            Texture2D bookTexture = Main.projectileTexture[projectile.type];
            Rectangle frame = bookTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 drawPosition;
            Vector2 origin = frame.Size() * 0.5f;
            Color glowColor = Color.Lerp(Color.Pink, Color.Red, (float)Math.Cos(Main.GlobalTime * 5f) * 0.5f + 0.5f);
            glowColor.A = 0;

            // Draw an ominous glowing version of the book after a bit of time.
            for (int i = 0; i < 8; i++)
            {
                drawPosition = projectile.Center + (MathHelper.TwoPi * i / 8f + Main.GlobalTime * 4f).ToRotationVector2() * glowOutwardness - Main.screenPosition;
                spriteBatch.Draw(bookTexture, drawPosition, frame, projectile.GetAlpha(glowColor), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }

            drawPosition = projectile.Center - Main.screenPosition;
            spriteBatch.Draw(bookTexture, drawPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool CanDamage() => false;
    }
}
