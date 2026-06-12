using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.MaceFlails
{
    public class UrchinMaceProj : BaseMaceFlailProjectile
    {
        // The entire chain is on the mace sprite, in which the player swings
        public override string Texture => "CalamityMod/Items/Weapons/Melee/UrchinMace";
        public override string ChainTexturePath => string.Empty;

        // This flail only runs through one state (spin), thereby making most parameters unnecessary to fill out
        public override int AssociatedItemID => ModContent.ItemType<UrchinMace>();
        public override int SpinIFrames => 15;
        public override float SpinHitboxRadius => 56f;
        public override float SpinVerticalFactor => 1f; // This flail moves circular
        public override float LaunchSpeed => 22f;

        public static float MaxWindup = 45f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            base.SetDefaults();
            Projectile.localNPCHitCooldown = SpinIFrames * Projectile.MaxUpdates;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
        }

        public override void SpinAI(float launchSpeed)
        {
            float WindupProgress = MathHelper.Clamp(StateTimer, 0, MaxWindup) / MaxWindup;
            if (Projectile.owner == Main.myPlayer)
            {
				// 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
				Vector2 toMouse = Owner.MountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * Owner.direction);
				Owner.ChangeDir((toMouse.X > 0f).ToDirectionInt());
				if (!Owner.channel && WindupProgress >= 1f)
				{
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.MountedCenter, toMouse * launchSpeed, ModContent.ProjectileType<RedtideWhirlpool>(), (int)(Projectile.damage * LaunchDamage), Projectile.knockBack, Projectile.owner);
                    SoundEngine.PlaySound(SoundID.Item7, Owner.MountedCenter);
                    Projectile.Kill();
					return;
				}

                if (StateTimer == MaxWindup)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        Vector2 dustPos = Owner.position + Main.rand.NextVector2FromRectangle(Owner.Hitbox);
                        Dust dust = Dust.NewDustPerfect(dustPos, DustID.BubbleBurst_Blue, Vector2.UnitY * -5f * Main.rand.NextFloat(1f, 2f) + Owner.velocity, Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;
                    }
                }
            }

            Projectile.velocity = Vector2.Zero;
            Projectile.rotation += (WindupProgress * MathHelper.PiOver4 / 1.5f) * Owner.direction;
            Projectile.Center = Owner.MountedCenter + Projectile.rotation.ToRotationVector2() * 10f - Vector2.UnitX * 4 * Owner.direction;

            if (WindupProgress > 0.5f)
            {
                int dustCount = Main.rand.Next(4);
                float offset = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < dustCount; i++)
                {
                    float angle = i / (float)dustCount * MathHelper.TwoPi + offset;
                    Vector2 dustPos = Owner.MountedCenter + angle.ToRotationVector2() * 40f * WindupProgress;
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.BubbleBurst_Blue, (angle - MathHelper.PiOver2 * Owner.direction).ToRotationVector2() * 5f + Owner.velocity, Scale: Main.rand.NextFloat(1f, 2f));
                    dust.noGravity = true;
                }
            }

            StateTimer++;
        }

        public override bool ExtraBehavior()
        {
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce, Owner.MountedCenter);
                Projectile.soundDelay = 28;
            }

            float armPointingDirection = ((Owner.Calamity().mouseWorld - Owner.MountedCenter).ToRotation());

            //"crop" the rotation so the player only points their arm in a smaller range. (The back arm points in the throw direction)
            if (armPointingDirection < MathHelper.PiOver2 && armPointingDirection >= -MathHelper.PiOver2)
                armPointingDirection = -MathHelper.PiOver2 + MathHelper.PiOver4 / 2f + MathHelper.PiOver2 * 1.5f * Utils.GetLerpValue(0f, MathHelper.Pi, armPointingDirection + MathHelper.PiOver2, true);
            else
            {
                if (armPointingDirection > 0)
                    armPointingDirection = MathHelper.PiOver2 + MathHelper.PiOver4 / 2f + MathHelper.PiOver4 * 1.5f * Utils.GetLerpValue(0f, MathHelper.PiOver2, armPointingDirection - MathHelper.PiOver2, true);
                else
                    armPointingDirection = -MathHelper.Pi + MathHelper.PiOver4 * 1.5f * Utils.GetLerpValue(-MathHelper.Pi, -MathHelper.PiOver4, armPointingDirection, true);
            }

            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            Projectile.timeLeft = 2;
            Owner.heldProj = Projectile.whoAmI;
            Owner.SetDummyItemTime(2);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 300);
            for (int i = 0; i < 3; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);

                Particle spike = new UrchinSpikeParticle(target.Center + angle.ToRotationVector2() * 15f, angle.ToRotationVector2() * 6f, angle + MathHelper.PiOver2, Main.rand.NextFloat(1f, 1.3f), lifetime: Main.rand.Next(10) + 25);
                GeneralParticleHandler.SpawnParticle(spike);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D maceTexture = TextureAssets.Projectile[Type].Value;
            Texture2D whirlpoolTexture = TextureAssets.Projectile[ModContent.ProjectileType<RedtideWhirlpool>()].Value;

            float WindupProgress = MathHelper.Clamp(StateTimer, 0, MaxWindup) / MaxWindup;
            float whirlpoolScale = MathHelper.Clamp(WindupProgress * 3f - 0.4f, 0f, 1f) * 1.6f;
            float whirlpoolOpacity = WindupProgress * 0.2f + MathF.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f;
            float whirlpoolRotation = StateTimer * 0.34f * Owner.direction;
            SpriteEffects flip = Owner.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(whirlpoolTexture, Owner.MountedCenter - Main.screenPosition, null, Lighting.GetColor((int)Owner.MountedCenter.X / 16, (int)Owner.MountedCenter.Y / 16) * whirlpoolOpacity * 0.3f, whirlpoolRotation * 1.2f, whirlpoolTexture.Size() * 0.5f, whirlpoolScale, flip);
            Main.EntitySpriteDraw(whirlpoolTexture, Owner.MountedCenter - Main.screenPosition, null, Lighting.GetColor((int)Owner.MountedCenter.X / 16, (int)Owner.MountedCenter.Y / 16) * whirlpoolOpacity, whirlpoolRotation, whirlpoolTexture.Size() * 0.5f, whirlpoolScale, flip);

            Vector2 handleOrigin = new Vector2(0, maceTexture.Height);
            float maceRotation = Projectile.rotation + MathHelper.PiOver4;
            Main.EntitySpriteDraw(maceTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), maceRotation, handleOrigin, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
