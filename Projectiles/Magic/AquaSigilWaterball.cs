using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AquaSigilWaterball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 108;
            Projectile.height = 94;
            Projectile.friendly = true;
            Projectile.penetrate = -1; // Explodes on a delay after hitting
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Time++;
            Projectile.Opacity = Utils.GetLerpValue(0f, 10f, Time, true);

            // Hasnt hit anything
            if (Projectile.ai[1] == 0)
            {
                // Set rotation to face forward
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

                Projectile.frameCounter++;
                if (Projectile.frameCounter % 4 == 3)
                    Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
            else // After hit
            {
                Projectile.velocity = Vector2.Zero;

                // Scale up and die
                Projectile.scale += 0.05f;
                if (Projectile.scale >= 1.25f)
                {
                    Projectile.Kill();
                }
                // Failsafe
                if (Projectile.timeLeft > 5)
                {
                    Projectile.timeLeft = 5;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Normal sprite
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;

            // All-white variant
            Asset<Texture2D> ghostTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AquaSigilWaterballGhost");

            Color drawColor = Projectile.GetAlpha(lightColor);

            // NORMAL draw logic
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame), drawColor, Projectile.rotation, new Vector2(49, 160), Projectile.scale, SpriteEffects.None, 0);

            // After hitting a target, draw the white variant
            if (Projectile.ai[1] > 0)
            {
                // Fade from normal to fully white
                float fadeFactor = Utils.GetLerpValue(5f, 0f, Projectile.timeLeft, true);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

                Main.EntitySpriteDraw(ghostTexture.Value, Projectile.Center - Main.screenPosition, texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame), Color.White * fadeFactor, Projectile.rotation, new Vector2(49, 160), Projectile.scale * (1f + fadeFactor * 0.2f), SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
            Projectile.ai[1] = 1; // Has hit an enemy
            Projectile.penetrate = -1;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/UnstableCastersGauntlet/AquaSigilExplosion") { Volume = 1f, PitchVariance = 0.1f }, Projectile.Center);

            for (int i = 0; i < 40; i++)
            {
                Vector2 vel = new Vector2(10, 10).RotatedByRandom(100) * Main.rand.NextFloat(0.85f, 1.2f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + (new Vector2(Main.rand.NextFloat(-22, -30), Main.rand.NextFloat(-4, 4)).RotatedBy(Projectile.rotation - MathHelper.PiOver2)), Main.rand.NextBool(5) ? 187 : 180, vel * Main.rand.NextFloat(0.1f, 0.9f) + new Vector2(0, -2));
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(1.1f, 1.7f);
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = new Vector2(10, 10).RotatedByRandom(100) * Main.rand.NextFloat(0.85f, 1.2f);
                Projectile droplets = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + (new Vector2(Main.rand.NextFloat(-22, -30), Main.rand.NextFloat(-4, 4)).RotatedBy(Projectile.rotation - MathHelper.PiOver2)), vel * 0.8f, ModContent.ProjectileType<AquaSigilWaterdroplet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + (new Vector2(-26, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2)), Vector2.Zero, ModContent.ProjectileType<AquaSigilWaterballExplosion>(), (int)(Projectile.damage * 3.5f), Projectile.knockBack, Projectile.owner);
                explosion.ai[1] = 260f;
                explosion.localAI[1] = Main.rand.NextFloat(0.1f, 0.2f); // Interpolate
                explosion.netUpdate = true;
            }
        }
    }
}
