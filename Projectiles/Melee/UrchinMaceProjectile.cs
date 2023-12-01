using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class UrchinMaceProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Items/Weapons/Melee/UrchinMace";

        public static float MaxWindup = 110;
        public ref float Windup => ref Projectile.ai[0];
        public float WindupProgress => MathHelper.Clamp(Windup, 0, MaxWindup) / MaxWindup;
        public static float whirlpoolDamageMultiplier = 2f;

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.ownerHitCheck = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //Prevent spam click abuse
            if (Windup < Projectile.localNPCHitCooldown)
                return false;

            float collisionPoint = 0f;
            float bladeLength = 70 * Projectile.scale;
            float bladeWidth = 30 * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation.ToRotationVector2() * bladeLength), bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            Owner.ChangeDir(Math.Sign(Owner.Calamity().mouseWorld.X - Owner.position.X));

            Projectile.velocity = Vector2.Zero;

            Projectile.rotation += (WindupProgress * MathHelper.PiOver4 / 1.5f) * Owner.direction;

            if (Owner.channel)
            {
                Projectile.timeLeft = 2;
                UpdateOwnerVars();
            }

            if (WindupProgress > 0.5f)
            {
                int dustCount = Main.rand.Next(4);
                float offset = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < dustCount; i++)
                {
                    float angle = i / (float)dustCount * MathHelper.TwoPi + offset;
                    Vector2 dustPos = Owner.MountedCenter + angle.ToRotationVector2() * 40f * WindupProgress;
                    Dust dust = Dust.NewDustPerfect(dustPos, 176, (angle - MathHelper.PiOver2 * Owner.direction).ToRotationVector2() * 5f + Owner.velocity, Scale: Main.rand.NextFloat(1f, 2f));
                    dust.noGravity = true;
                }
            }

            Projectile.Center = Owner.MountedCenter + Projectile.rotation.ToRotationVector2() * 10f - Vector2.UnitX * 4 * Owner.direction;

            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce, Owner.MountedCenter);
                Projectile.soundDelay = 28;
            }

            if (Windup == MaxWindup && Owner.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item43, Owner.MountedCenter);

                for (int i = 0; i < 25; i++)
                {
                    Vector2 dustPos = Owner.position + Main.rand.NextVector2FromRectangle(Owner.Hitbox);
                    Dust dust = Dust.NewDustPerfect(dustPos, 176, Vector2.UnitY * -5f * Main.rand.NextFloat(1f, 2f) + Owner.velocity, Scale: Main.rand.NextFloat(1f, 2f));
                    dust.noGravity = true;
                }
            }

            Windup++;
        }

        public void UpdateOwnerVars()
        {
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
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);

                Particle spike = new UrchinSpikeParticle(target.Center + angle.ToRotationVector2() * 15f, angle.ToRotationVector2() * 6f, angle + MathHelper.PiOver2, Main.rand.NextFloat(1f, 1.3f), lifetime: Main.rand.Next(10) + 25);
                GeneralParticleHandler.SpawnParticle(spike);
            }
        }

        //If we don't do that, the hit enemies get knocked back towards you if you hit them from the right??
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // This would knock enemies away consistently, but i'm choosing to go with the other option
            //hitDirection = Math.Sign(target.Center.X - Owner.Center.X);
            
            //Doing it this way lets the player choose if they want to knockback enemies towards them by pointing away from them
            modifiers.HitDirectionOverride = Owner.direction;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Owner.ChangeDir(Math.Sign(Owner.Calamity().mouseWorld.X - Owner.position.X));

            Texture2D maceTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D whirlpoolTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/RedtideWhirlpool").Value;

            float whirlpoolScale = MathHelper.Clamp(WindupProgress * 3f - 0.4f, 0f, 1f) * 2f;
            float whirlpoolOpacity = WindupProgress * 0.2f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f;
            float whirlpoolRotation = Windup * 0.34f * Owner.direction;
            SpriteEffects flip = Owner.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(whirlpoolTexture, Owner.MountedCenter - Main.screenPosition, null, Lighting.GetColor((int)Owner.MountedCenter.X / 16, (int)Owner.MountedCenter.Y / 16) * whirlpoolOpacity * 0.3f, whirlpoolRotation * 1.2f, whirlpoolTexture.Size() / 2f, whirlpoolScale, flip, 0);
            Main.EntitySpriteDraw(whirlpoolTexture, Owner.MountedCenter - Main.screenPosition, null, Lighting.GetColor((int)Owner.MountedCenter.X / 16, (int)Owner.MountedCenter.Y / 16) * whirlpoolOpacity, whirlpoolRotation, whirlpoolTexture.Size() / 2f, whirlpoolScale, flip, 0);

            Vector2 handleOrigin = new Vector2(0, maceTexture.Height);
            float maceRotation = Projectile.rotation + MathHelper.PiOver4;
            Main.EntitySpriteDraw(maceTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), maceRotation, handleOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            //Spawn a whirlpool typhoon after sending it out
            if (WindupProgress >= 1f && Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, (Main.MouseWorld - Owner.position).SafeNormalize(Vector2.One) * 25f, ModContent.ProjectileType<RedtideWhirlpool>(), (int)(Projectile.damage * whirlpoolDamageMultiplier), Projectile.knockBack, Projectile.owner, 0, 0f);
            }

            SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
        }
    }
}
