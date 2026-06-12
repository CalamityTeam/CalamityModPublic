using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PrismaticMagicCircle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public ref float Timer => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];
        public int Lifetime = 360;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 512;
            Projectile.friendly = true;
            Projectile.DamageType = MeleeRangedHybridDamageClass.Instance;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Lifetime;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Timer++;
            if (Owner.CantUseHoldout())
            {
                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;
            }
            if (Owner.active && !Owner.dead)
                Projectile.Center = Owner.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 60f;

            // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            Vector2 aimVector = (Main.MouseWorld - Owner.RotatedRelativePoint(Owner.MountedCenter, true)).SafeNormalize(Vector2.UnitY);
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), PrismaticBreakerHoldout.LaserAimLag));

            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer < 30f)
                Projectile.scale = MathHelper.Lerp(0f, 1f, Timer / 30f);
            else
                Projectile.scale = Utils.GetLerpValue(0f, 30f, Projectile.timeLeft, true);

            if (Timer == 1f && Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.Item67, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item68, Projectile.Center);
                SoundEngine.PlaySound(Items.Weapons.DraedonsArsenal.TeslaCannon.FireSound with { Pitch = 1f });
                Vector2 spawnPos = Vector2.Lerp(Projectile.Center, Owner.Center, 0.5f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, Projectile.velocity, ModContent.ProjectileType<PrismaticRay>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw the awesome and cool magic circle to hide the fact that the laser gets cut off horribly
            Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
            Texture2D howNoisy = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/MeltyNoise").Value;
            Vector2 squishScale = new Vector2(Projectile.width / howNoisy.Width * 0.55f, Projectile.height / howNoisy.Height * 2f) * Projectile.scale * 0.36f;

            GameShaders.Misc["CalamityMod:ExoVortex"].Apply();
            for (int i = 0; i < 6; i++)
                Main.spriteBatch.Draw(howNoisy, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, howNoisy.Size() / 2f, squishScale, SpriteEffects.None, 0f);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
