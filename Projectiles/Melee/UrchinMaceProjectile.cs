using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Melee
{
    public class UrchinMaceProjectile : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Items/Weapons/Melee/UrchinMace";

        public static float MaxWindup = 110;
        public ref float Windup => ref Projectile.ai[0];
        public float WindupProgress => MathHelper.Clamp(Windup, 0, MaxWindup) / MaxWindup;
        public static float whirlpoolDamageMultiplier = 1.6f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Mace");
        }

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
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //Prevent spam click abuse
            if (Windup < Projectile.localNPCHitCooldown)
                return false;

            float collisionPoint = 0f;
            float bladeLenght = 70 * Projectile.scale;
            float bladeWidth = 30 * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation.ToRotationVector2() * bladeLenght), bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            Owner.direction = Math.Sign(Owner.Calamity().mouseWorld.X - Owner.position.X);

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
                    Vector2 dustPos = Owner.Center + angle.ToRotationVector2() * 40f * WindupProgress;
                    Dust dust = Dust.NewDustPerfect(dustPos, 176, (angle - MathHelper.PiOver2 * Owner.direction).ToRotationVector2() * 5f + Owner.velocity, Scale: Main.rand.NextFloat(1f, 2f));
                    dust.noGravity = true;
                }
            }

            Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * 10f - Vector2.UnitX * 4 * Owner.direction;

            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce, Owner.Center);
                Projectile.soundDelay = 28;
            }

            if (Windup == MaxWindup && Owner.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item43, Owner.Center);
            }

            Windup++;
        }

        public void UpdateOwnerVars()
        {
            float armPointingDirection = ((Owner.Calamity().mouseWorld - Owner.Center).ToRotation());

            //"crop" the rotation so the player only tilts the fishing rod slightly up and slightly down.
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 3; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);

                Particle spike = new UrchinSpikeParticle(target.Center + angle.ToRotationVector2() * 15f, angle.ToRotationVector2() * 6f, angle + MathHelper.PiOver2, Main.rand.NextFloat(1f, 1.3f), lifetime: Main.rand.Next(10) + 25);
                GeneralParticleHandler.SpawnParticle(spike);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Owner.direction = Math.Sign(Owner.Calamity().mouseWorld.X - Owner.position.X);

            Texture2D maceTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D whirlpoolTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/RedtideWhirlpool").Value;

            float whirlpoolScale = MathHelper.Clamp(WindupProgress * 3f - 0.4f, 0f, 1f) * 2f;
            float whirlpoolOpacity = WindupProgress * 0.2f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f;
            float whirlpoolRotation = Windup * 0.34f * Owner.direction;
            SpriteEffects flip = Owner.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(whirlpoolTexture, Owner.Center - Main.screenPosition, null, Lighting.GetColor((int)Owner.Center.X / 16, (int)Owner.Center.Y / 16) * whirlpoolOpacity * 0.3f, whirlpoolRotation * 1.2f, whirlpoolTexture.Size() / 2f, whirlpoolScale, flip, 0);
            Main.spriteBatch.Draw(whirlpoolTexture, Owner.Center - Main.screenPosition, null, Lighting.GetColor((int)Owner.Center.X / 16, (int)Owner.Center.Y / 16) * whirlpoolOpacity, whirlpoolRotation, whirlpoolTexture.Size() / 2f, whirlpoolScale, flip, 0);

            Vector2 handleOrigin = new Vector2(0, maceTexture.Height);
            float maceRotation = Projectile.rotation + MathHelper.PiOver4;
            Main.spriteBatch.Draw(maceTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), maceRotation, handleOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void Kill(int timeLeft)
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
