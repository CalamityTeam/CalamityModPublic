using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Items.Weapons.DraedonsArsenal;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class ExoskeletonPlasmaCannon : ExoskeletonCannon
    {
        public override int ShootRate => 32;

        public override float ShootSpeed => 13.5f;

        public override Vector2 OwnerRestingOffset => new(300f, 96f);

        public override void ClampFirstLimbRotation(ref double limbRotation)
        {
            limbRotation = 0.23f;
        }
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Cannon");
            Main.projFrames[Type] = 6;
        }

        public override void PostAI()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Type];
        }

        public override void ShootAtTarget(NPC target, Vector2 shootDirection)
        {
            // Play the plasma caster fire sound.
            SoundEngine.PlaySound(PlasmaCaster.FireSound with { Volume = 0.4f }, Projectile.Center);

            // Create a burst of dust.
            for (int i = 0; i < 40; i++)
            {
                float dustSpeed = Main.rand.NextFloat(1.8f, 3f);                
                Vector2 dustVel = shootDirection * dustSpeed;
                dustVel = dustVel.RotatedBy(-0.35f);
                dustVel = dustVel.RotatedByRandom(2.0f * 0.35f);
                int randomDustType = Main.rand.NextBool(2) ? 107 : 110;

                Dust plasma = Dust.NewDustDirect(Projectile.TopLeft, Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 1.7f);
                plasma.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                plasma.position += shootDirection * 60f;
                plasma.noGravity = true;
                plasma.velocity *= Projectile.scale * 3f;

                plasma = Dust.NewDustDirect(Projectile.TopLeft, Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 0.8f);
                plasma.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                plasma.position += shootDirection * 60f;
                plasma.velocity *= Projectile.scale * 2f;

                plasma.noGravity = true;
                plasma.fadeIn = 1f;
                plasma.color = Color.Green * 0.5f;
            }
            for (int i = 0; i < 20; i++)
            {
                float dustSpeed = Main.rand.NextFloat(1.8f, 3f);
                Vector2 dustVel = shootDirection * dustSpeed;
                dustVel = dustVel.RotatedBy(-0.35f);
                dustVel = dustVel.RotatedByRandom(2.0f * 0.35f);
                int randomDustType = Main.rand.NextBool(2) ? 107 : 110;

                Dust plasma = Dust.NewDustDirect(Projectile.TopLeft, Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 2f);
                plasma.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(shootDirection.ToRotation()) * Projectile.width / 3f;
                plasma.position += shootDirection * 60f;
                plasma.noGravity = true;
                plasma.velocity *= Projectile.scale * 0.5f;
            }

            // Shoot the fireball. This only happens for the owner client.
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 fireballVelocity = shootDirection * ShootSpeed;
            int fireball = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, fireballVelocity, ModContent.ProjectileType<MinionPlasmaBlast>(), Projectile.damage, 0f, Projectile.owner);
            if (Main.projectile.IndexInRange(fireball))
                Main.projectile[fireball].originalDamage = Projectile.originalDamage;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonPlasmaCannonGlowmask").Value;
            Rectangle frame = texture.Frame(2, Main.projFrames[Type], TargetingSomething.ToInt(), Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotation = Projectile.rotation;
            if (Projectile.spriteDirection == -1)
                rotation += MathHelper.Pi;

            DrawLimbs();
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), rotation, origin, Projectile.scale, direction, 0);

            return false;
        }
    }
}
