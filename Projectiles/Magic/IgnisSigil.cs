using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.ID;

namespace CalamityMod.Projectiles.Magic
{
    public class IgnisSigil : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private bool spawnedProjectile = false;
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

                if (Projectile.localAI[0] == 35 && !spawnedProjectile)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);

                    Player owner = Main.player[Projectile.owner];
                    Vector2 targetDirection = Projectile.Center.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX);

                    spawnedProjectile = true;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, targetDirection * 50f, ModContent.ProjectileType<IgnisSigilFireball>(), Projectile.damage * 5, Projectile.knockBack, Projectile.owner);
                }

                if (Projectile.localAI[0] >= 50)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.scale = parent.scale;
                Projectile.rotation = 0;
                Projectile.alpha = parent.alpha;
            }

            if (parentActive)
            {
                Projectile.timeLeft = parent.timeLeft;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Get textures
            Texture2D mainTexture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Texture2D blankTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/BlankSigil").Value;

            float finalScale = Projectile.scale;
            float alphaOpacity = 1f - (Projectile.alpha / 255f);
            float maskOpacity = 0f;

            // Apply animation effects if fading
            if (Projectile.ai[2] > 0)
            {
                float animationTime = Projectile.localAI[0];

                if (animationTime <= 24)
                {
                    maskOpacity = Utils.GetLerpValue(0f, 24f, animationTime, clamped: true);
                }
                else
                {
                    maskOpacity = 1f;
                }

                if (animationTime >= 35)
                {
                    float scaleFactor = Utils.GetLerpValue(35f, 50f, animationTime, clamped: true);
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
