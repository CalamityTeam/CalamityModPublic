using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CosmilampMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        public int HoverOffsetIndex => (int)Projectile.ai[0];

        public float HoverOffsetInterpolant
        {
            get
            {
                float projectileCounts = Owner.ownedProjectileCounts[Type];

                // Use a midway interpolant if the projectile count is one. This makes the fist use middle positions instead of
                // sitting awkwardly to the left.
                if (projectileCounts <= 1f)
                    return 0.5f;

                return HoverOffsetIndex / (projectileCounts - 1f);
            }
        }

        public ref float Timer => ref Projectile.ai[1];

        public override string Texture => "CalamityMod/NPCs/Signus/CosmicLantern";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.minionSlots = Cosmilamp.LanternSummonCost;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.MaxUpdates = 2;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            // Decide whether the minion should still exist.
            HandleMinionBools();

            // Decide frames.
            DecideFrames();

            // Hover above the owner.
            HoverInPlace();

            // Increment the universal timer.
            Timer++;

            // Shoot at nearby enemies.
            NPC potentialTarget = Projectile.Center.MinionHoming(Cosmilamp.MaxTargetingDistance, Owner);
            if (potentialTarget is not null)
            {
                int wrappedAttackTimer = (int)(Timer % Cosmilamp.BeamShootRate);
                if (wrappedAttackTimer == (int)(HoverOffsetInterpolant * (Cosmilamp.BeamShootRate - 18f)))
                {
                    SoundEngine.PlaySound(SoundID.Item158, Projectile.Center);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 beamVelocity = Projectile.SafeDirectionTo(potentialTarget.Center).RotatedByRandom(0.32f) * Main.rand.NextFloat(9.4f, 11f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, beamVelocity, ModContent.ProjectileType<CosmilampBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }

        public void HandleMinionBools()
        {
            Owner.AddBuff(ModContent.BuffType<CosmilampBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<CosmilampMinion>())
            {
                if (Owner.dead)
                    Owner.Calamity().cLamp = false;

                if (Owner.Calamity().cLamp)
                    Projectile.timeLeft = 2;
            }
        }

        public void DecideFrames()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 8 % Main.projFrames[Projectile.type];
        }

        public void HoverInPlace()
        {
            Vector2 hoverDestination = Owner.Top + new Vector2(MathHelper.Lerp(-100f, 100f, HoverOffsetInterpolant), -80f);
            hoverDestination.Y += ((float)Math.Sin(MathHelper.TwoPi * HoverOffsetInterpolant + Timer / 50f) * 0.5f + 0.5f) * 40f;

            // Zoom towards the hover destination.
            Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.02f).MoveTowards(hoverDestination, 8f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw afterimages.
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float afterimageFade = (1f - i / (float)Projectile.oldPos.Length);
                Color afterimageDrawColor = Color.Lerp(Color.Fuchsia, Color.Cyan, afterimageFade) with { A = 25 } * Projectile.Opacity * afterimageFade * 0.6f;
                Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Main.EntitySpriteDraw(texture, afterimageDrawPosition, frame, afterimageDrawColor, Projectile.rotation, origin, Projectile.scale, direction, 0);
            }

            // Draw a cyan backglow.
            for (int i = 0; i < 8; i++)
            {
                Color afterimageDrawColor = Color.Cyan with { A = 25 } * Projectile.Opacity * 0.4f;
                Vector2 afterimageDrawPosition = Projectile.Center - Main.screenPosition + (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 3f;
                Main.EntitySpriteDraw(texture, afterimageDrawPosition, frame, afterimageDrawColor, Projectile.rotation, origin, Projectile.scale, direction, 0);
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }

        // The lamps themselves do not do damage, but they do store damage for the sake of shooting projectiles.
        public override bool? CanDamage() => false;
    }
}
