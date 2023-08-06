using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class LightBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private const int Lifetime = 300;
        private const int NumAfterimages = 8;
        private const float LightBrightness = 0.7f;
        private const int DustID = 175;
        const float SwordHomingStrength = 30f;
        const float EnemyHomingStrength = 70f;

        private Color lightColor = Color.White;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = NumAfterimages;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.scale = 1.5f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // The sprite is diagonal, so rotate by pi/4
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // These effects only occur on the first frame. Alpha is set to 0 immediately on frame 1.
            if (Projectile.alpha > 0)
            {
                Projectile.alpha = 0;
                FirstFrameEffects();

                // Store original velocity for homing calculations
                Projectile.ai[0] = Projectile.velocity.Length();
            }

            // Every other frame, add less light.
            else
                Lighting.AddLight(Projectile.Center, lightColor.ToVector3() * LightBrightness);

            // Home in on the paired sword, if one is defined and it exists
            if (Projectile.ai[1] > 0f)
            {
                Projectile paired = Main.projectile[(int)(Projectile.ai[1] - 1)];
                if (paired is null || !paired.active || paired.type != ModContent.ProjectileType<LightBlade>())
                    Projectile.ai[1] = 0f;
                else
                {
                    Vector2 homingVec = Projectile.SafeDirectionTo(paired.Center) * Projectile.ai[0];
                    Projectile.velocity = (Projectile.velocity * (SwordHomingStrength - 1f) + homingVec) / SwordHomingStrength;
                }
            }

            // Also weakly home in on nearby enemies
            CalamityUtils.HomeInOnNPC(Projectile, true, 200f, Projectile.ai[0], EnemyHomingStrength);

            // The projectile never slows down, even as it tries to curve towards enemies and its paired sword.
            float currentSpeed = Projectile.velocity.Length();
            Projectile.velocity *= Projectile.ai[0] / currentSpeed;
        }

        private void FirstFrameEffects()
        {
            // Pick a random sword sprite and a random color to use.
            Projectile.frame = Main.rand.Next(14);
            lightColor = TheDanceofLight.GetRandomLightColor();

            // Spawn starting dust.
            Vector2 baseOffsetVec = new Vector2(1f, 4f);
            int numDust = 16;
            for (int i = 0; i < numDust; ++i)
            {
                Vector2 dustOffset = Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / numDust) * baseOffsetVec;
                dustOffset = dustOffset.RotatedBy(Projectile.velocity.ToRotation());
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID, 0f, 0f);
                d.position = Projectile.Center + dustOffset;
                d.velocity = dustOffset.SafeNormalize(Vector2.Zero);
                d.velocity *= 1.4f;
                d.scale = 1.5f;
                d.noGravity = true;
            }

            // Each blade appears in a huge flash of light.
            float startingBrightness = LightBrightness * 3f;
            Lighting.AddLight(Projectile.Center, lightColor.ToVector3() * startingBrightness);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer calPlayer = player.Calamity();
            calPlayer.danceOfLightCharge++;
            if (calPlayer.danceOfLightCharge >= TheDanceofLight.HitsPerFlash)
            {
                calPlayer.danceOfLightCharge = 0;
                if (Projectile.owner == Main.myPlayer)
                {
                    int flashDamage = (int)player.GetTotalDamage<MagicDamageClass>().ApplyTo(TheDanceofLight.FlashBaseDamage);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlindingLight>(), flashDamage, 0f, Projectile.owner);
                }
            }

        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
            int numDust = Main.rand.Next(4, 10);
            for (int i = 0; i < numDust; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID, 0f, 0f, 100);
                d.velocity *= 1.6f;
                d.velocity += Projectile.velocity * Main.rand.NextFloat(-0.5f, 0.5f);
                d.velocity.Y += -1f;
                d.noGravity = true;
                d.scale = 2f;
                d.fadeIn = 0.5f;
            }
        }

        public override Color? GetAlpha(Color lightColor) => lightColor;

        // Uses bizarre drawcode because it has a horizontal sprite sheet
        public override bool PreDraw(ref Color lightColor)
        {
            Point projTile = Projectile.Center.ToTileCoordinates();
            Color localLight = Lighting.GetColor(projTile.X, projTile.Y);

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rect = new Rectangle(38 * Projectile.frame, 0, 38, 38);
            Vector2 halfSpriteSize = rect.Size() / 2f;

            // Draw the projectile itself
            Color mainSwordColor = Projectile.GetAlpha(localLight);
            mainSwordColor.A = 160;
            SpriteEffects sfx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Main.EntitySpriteDraw(tex, drawPos, new Rectangle?(rect), mainSwordColor, Projectile.rotation, halfSpriteSize, Projectile.scale, sfx, 0);

            // Draw the projectile's afterimages
            for (int i = 0; i < NumAfterimages; ++i)
            {
                Color afterimageLight = Projectile.GetAlpha(localLight);
                afterimageLight.A = 40;

                float posLerp = 1f;
                Vector2 afterimagePos = (1f - posLerp) * Projectile.position + posLerp * Projectile.oldPos[i];
                float rotation = Projectile.oldRot[i];
                SpriteEffects afterimageSfx = Projectile.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                float afterimageScale = MathHelper.Lerp(1.2f * Projectile.scale, 0.4f * Projectile.scale, i / (NumAfterimages - 1f));

                Vector2 imageDrawPos = afterimagePos + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Main.EntitySpriteDraw(tex, imageDrawPos, new Rectangle?(rect), afterimageLight, rotation, halfSpriteSize, afterimageScale, afterimageSfx, 0);
            }

            return false;
        }
    }
}
