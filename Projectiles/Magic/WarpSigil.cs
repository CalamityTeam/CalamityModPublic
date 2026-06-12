using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Magic
{
    public class WarpSigil : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private bool spawnedProjectile = false;
        private bool spawnedIntroParticle = false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 74;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile parent = Main.projectile[(int)Projectile.ai[0]];
            bool parentActive = parent != null && parent.active && parent.type == ModContent.ProjectileType<SigilSet>();

            if (!parentActive && Projectile.ai[2] == 0)
            {
                Projectile.Kill();
                return;
            }

            int i = (int)Projectile.ai[1];
            float dist = i % 3 == 0 ? 270f : 280f;
            float extraRot = i % 3 == 0 ? 0 : i % 3 == 1 ? MathHelper.ToRadians(-3.33f) : MathHelper.ToRadians(3.33f);

            Vector2 sigilPos = parent.Center + (Vector2.UnitX.RotatedBy(MathHelper.Lerp(0, MathHelper.TwoPi, i / 6f)) * dist).RotatedBy(parent.rotation + extraRot);
            Projectile.Center = sigilPos;

            if (Projectile.ai[2] > 0)
            {
                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= 25 && !spawnedProjectile)
                {
                    // ai[1] and ai[2] will inherit this sigil's position and timeLeft as its own
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<WarpSigilShotCreator>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner, 0, Projectile.whoAmI, Projectile.timeLeft);
                    spawnedProjectile = true;
                }

                if (Projectile.localAI[0] >= 70)
                {
                    Projectile.Kill();
                }
            }

            else
            {
                Projectile.rotation = 0;
                Projectile.alpha = parent.alpha;
            }

            if (parentActive)
            {
                Projectile.timeLeft = parent.timeLeft;
            }

            if (!spawnedIntroParticle)
                for (int j = 0; j < 13; j++)
                {
                    spawnedIntroParticle = true;
                    GeneralParticleHandler.SpawnParticle(new SquishyLightParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(3f, 7f), 0f).RotatedByRandom(MathHelper.TwoPi), Projectile.scale * Main.rand.NextFloat(0.2f, 0.475f), Main.rand.NextBool() ? Color.Magenta : Color.White, Main.rand.Next(12, 24), 1f, 0f, 1f));
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTexture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Texture2D blankTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/BlankSigil").Value;

            float finalScale = Projectile.scale;
            float alphaOpacity = 1f - (Projectile.alpha / 255f);
            float maskOpacity = 0f;

            // Apply animation effects if the fade-out is active
            if (Projectile.ai[2] > 0)
            {
                float animationTime = Projectile.localAI[0];

                if (animationTime <= 18)
                {
                    maskOpacity = Utils.GetLerpValue(0f, 18f, animationTime, clamped: true);
                }
                else
                {
                    maskOpacity = 1f;
                }

                if (animationTime >= 55)
                {
                    float scaleFactor = Utils.GetLerpValue(55f, 70f, animationTime, clamped: true);
                    finalScale = MathHelper.Lerp(Projectile.scale, 0f, scaleFactor);
                    alphaOpacity = MathHelper.Lerp(alphaOpacity, 0f, scaleFactor);
                }
            }

            Main.EntitySpriteDraw(mainTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * alphaOpacity, Projectile.rotation, mainTexture.Size() / 2, finalScale, SpriteEffects.None, 0);

            if (Projectile.ai[2] > 0)
            {
                Main.EntitySpriteDraw(blankTexture, Projectile.Center - Main.screenPosition, null, Color.White * maskOpacity, Projectile.rotation, blankTexture.Size() / 2, finalScale, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}
