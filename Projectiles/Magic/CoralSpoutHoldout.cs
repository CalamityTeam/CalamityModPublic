using System;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace CalamityMod.Projectiles.Magic
{
    //Holdout, but invisible. It may as well be named "CoralSpoutHandler"
    public class CoralSpoutHoldout : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static float MaxCharge = 50;
        public static int ShotProjectiles = 5;

        public ref float Charge => ref Projectile.ai[0];
        public float ChargeProgress => MathHelper.Clamp(Charge, 0, MaxCharge) / MaxCharge;
        public float FullChargeProgress => MathHelper.Clamp(Charge, 0, MaxCharge * 1.5f) / (MaxCharge * 1.5f);
        public float Spread => MathHelper.PiOver2 * (1 - (float)Math.Pow(ChargeProgress, 1.5) * 0.95f);

        public Player Owner => Main.player[Projectile.owner];



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coral Spout");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            if (Owner.channel)
            {
                Projectile.timeLeft = 2;
                Owner.itemTime = 25;
                Owner.itemAnimation = 25;
                Owner.heldProj = Projectile.whoAmI;
            }

            float pointingRotation = (Owner.Calamity().mouseWorld - Owner.MountedCenter).ToRotation();
            Projectile.Center = Owner.MountedCenter + pointingRotation.ToRotationVector2() * 40f;

            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(CoralSpout.ChargeSound with { Pitch = 0.5f * ChargeProgress}, Owner.MountedCenter);
                Projectile.soundDelay = 10;
            }

            if (Charge == (int)(MaxCharge * 1.5f) && Owner.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.MaxMana, Owner.MountedCenter);
            }

            Charge++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/DeathstareBeam").Value;

            for (int i = -1; i <= 1; i += 2)
            {
                float angle = (Owner.MountedCenter - Owner.Calamity().mouseWorld).ToRotation() + (Spread / 2f) * i - MathHelper.PiOver2;
                Vector2 scale = new Vector2(0.2f, 1f) * 3f;
                Main.EntitySpriteDraw(texture, Owner.MountedCenter - Main.screenPosition, null, Color.DodgerBlue * 0.5f * (float)Math.Sqrt(ChargeProgress), angle, new Vector2(texture.Width / 2f, texture.Height), scale, 0, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            float mainAngle = (Projectile.Center - Owner.MountedCenter).ToRotation();

            if (FullChargeProgress < 1)
            {
                SoundEngine.PlaySound(SoundID.Item167 with { Volume = SoundID.Item167.Volume * 0.4f + 0.2f * ChargeProgress }, Owner.MountedCenter);


                for (int i = 0; i < ShotProjectiles; i++)
                {
                    float angleOffset = MathHelper.Lerp(Spread * -0.5f, Spread * 0.5f, i / (float)ShotProjectiles);
                    Vector2 direction = (mainAngle + angleOffset).ToRotationVector2();
                    
                    if (Owner.whoAmI == Main.myPlayer)
                    {
                        float speed = 10 + 15 * ChargeProgress;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.MountedCenter + direction * 30f, direction * speed, ModContent.ProjectileType<CoralSpike>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, ChargeProgress);
                    }

                    Color pulseColor = Main.rand.NextBool() ? Color.Coral : Color.DeepSkyBlue;
                    Particle pulse = new DirectionalPulseRing(Owner.MountedCenter + direction * 44f, Vector2.Zero, pulseColor, new Vector2(0.5f, 1f), direction.ToRotation(), 0.04f, 0.2f, 30);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }

            }

            else
            {
                SoundEngine.PlaySound(SoundID.Item42, Owner.MountedCenter);
                Vector2 direction = mainAngle.ToRotationVector2();

                if (Owner.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.MountedCenter + direction * 30f, direction * 35, ModContent.ProjectileType<ManaChargedCoral>(), (int)Projectile.damage * (ShotProjectiles + 1), Projectile.knockBack, Owner.whoAmI);
                }

                Color pulseColor = Main.rand.NextBool() ? Color.Coral : Color.DeepSkyBlue;
                Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, pulseColor, new Vector2(0.5f, 1f), direction.ToRotation(), 0.05f, 0.34f + Main.rand.NextFloat(0.3f), 30);
                GeneralParticleHandler.SpawnParticle(pulse);
            }
        }
    }
}
