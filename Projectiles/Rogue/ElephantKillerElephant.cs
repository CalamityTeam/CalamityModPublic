using System;
using System.Collections.Generic;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ElephantKillerElephant : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public SlotId soundSlot;
        public float rumble = 0;
        public float displace = 0;
        public ref float holeSize => ref Projectile.localAI[1];
        public ref float time => ref Projectile.ai[0];
        public Color clr = Color.CornflowerBlue;
        public static Asset<Texture2D> ElephantTexture { get; private set; }
        public override void Load()
        {   
            if (Main.dedServ)
                return;

            ElephantTexture = Terraria.GameContent.TextureAssets.Projectile[Type];
        }
        public override void SetDefaults()
        {
            Projectile.scale = 2f;
            Projectile.width = 135;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 4600;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void AI()
        {
            float rate = Main.GlobalTimeWrappedHourly * 5;
            List<Color> eColors = new List<Color>()
            {
                Color.CornflowerBlue,
                Color.SkyBlue
            };

            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            clr = Color.Lerp(Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f), Color.White, 0.7f);

            if (time == 0)
            {
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            float sine = MathF.Sin((time * 0.05f) / MathHelper.Pi);
            if (time < 70)
            {
                Projectile.velocity *= 0.93f;
            }
            else
            {
                Projectile.extraUpdates = 1;
                Projectile.velocity = Vector2.Zero;
                Projectile.Center += Vector2.UnitY * sine * 0.1f;
            }
            Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.spriteDirection == -1 ? MathHelper.Pi : 0, 0.04f);

            if (time == 90)
            {
                soundSlot =  SoundEngine.PlaySound(ElephantKiller.ElephantSound with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f), MaxInstances = -1 }, Projectile.Center);
                rumble = 30;
            }

            if (SoundEngine.TryGetActiveSound(soundSlot, out var sound) && sound.IsPlaying)
                sound.Position = Projectile.Center;

            Projectile.Opacity = 1 - MathF.Pow(Utils.GetLerpValue(230, 0,  Projectile.timeLeft, true), 6);
            rumble = MathHelper.Lerp(rumble, 0, 0.03f);
            time += Utils.Remap(Projectile.Opacity, 1, 0, 1, 10);
            holeSize = MathHelper.Lerp(holeSize, 0, 0.04f);

            Lighting.AddLight(Projectile.Center, clr.ToVector3() * 1.2f * Projectile.Opacity * MathF.Pow(Utils.GetLerpValue(0, 35, time, true), 3));
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 generalDrawPos = Projectile.Center + Main.rand.NextVector2Circular(rumble, rumble) + Vector2.UnitY * 25;

            SpriteEffects spriteFx = SpriteEffects.FlipHorizontally;
            if (Projectile.spriteDirection == -1)
                spriteFx |= SpriteEffects.FlipVertically;

            float deathFade = 1 - Projectile.Opacity;
            float lerp = MathF.Pow(Utils.GetLerpValue(0, 35, time, true), 3);
            float scale = (0.5f + 0.5f * lerp);

            float lerpFade = (1 - MathF.Pow(Utils.GetLerpValue(1f, 0, deathFade, true), 5));
            int frameDivis = 2;
            int framesY = tex.Height / frameDivis;
            int framesX = tex.Width / frameDivis;
            float directionRot = Projectile.rotation;
            Color usedColor = clr;
            for (int i = 1; i <= framesY; i++)
            {

                float sine = MathF.Sin((time * 0.15f + i * frameDivis) / MathHelper.Pi);
                float frameLerp = Utils.GetLerpValue(framesY, 1, i, true);
                float frameFade = Math.Max(MathF.Pow(frameLerp, 4), deathFade);
                float colorFade = MathF.Pow(1 - frameLerp, 1.5f);

                float rate = Main.GlobalTimeWrappedHourly * 4 + i * 0.03f;
                List<Color> eColors = new List<Color>()
                {
                    Color.CornflowerBlue,
                    Color.SkyBlue
                };

                int colorIndex = (int)(rate / 2 % eColors.Count);
                Color currentColor = eColors[colorIndex];
                Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                usedColor = Color.Lerp(currentColor, nextColor, rate % 2f >= 1f ? 1f : rate % 1f);

                for (int x = 1; x <= framesX; x++)
                {
                    Rectangle frame = tex.Frame(framesX, framesY, framesX - x, framesY - i);

                    Vector2 xPos = Vector2.UnitX.RotatedBy(Projectile.spriteDirection == -1 ? directionRot + MathHelper.Pi : directionRot) * frame.Width * x * 2 * Projectile.spriteDirection;

                    Vector2 frameMovement = generalDrawPos + (xPos + -Vector2.UnitY.RotatedBy(directionRot) * framesY * (Projectile.spriteDirection == 1 ? -2 * frameDivis : 0f) + -Vector2.UnitY.RotatedBy(directionRot) * frame.Height * 2 * i * Projectile.spriteDirection) * scale
                    + Vector2.UnitX.RotatedBy(directionRot) * (25 + (295 * Utils.GetLerpValue(tex.Height / 2, 0, Math.Abs(tex.Height / 2 - i), true) * lerpFade)) * sine * frameFade;

                    Vector2 pixelPlace = frameMovement;

                    Vector2 holePlace = new Vector2(Projectile.ai[1], Projectile.ai[2]) + (Projectile.spriteDirection * tex.Size());
                    float distFromHole = pixelPlace.Distance(holePlace);
                    Vector2 holeAdjust = pixelPlace.DirectionFrom(holePlace) * MathHelper.Lerp(0, 25, MathF.Pow(Utils.GetLerpValue(80, 0, distFromHole, true), 2f)) * holeSize;
                    
                    Vector2 linePlace = holePlace + Projectile.localAI[0].ToRotationVector2() * distFromHole;
                    Vector2 lineAdjust = pixelPlace.DirectionFrom(linePlace) * MathHelper.Lerp(0, 25, MathF.Pow(Utils.GetLerpValue(80, 0, pixelPlace.Distance(linePlace), true), 2f)) * (holeSize / (8 - 7 * MathF.Pow(Utils.GetLerpValue(420, 0, distFromHole, true), 1.5f)));
                    
                    Main.EntitySpriteDraw(tex, frameMovement + holeAdjust + lineAdjust - Main.screenPosition, frame, usedColor with { A = 0 } * (colorFade) * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, Projectile.scale * scale, spriteFx, 0);
                }
            }
            
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(soundSlot, out var sound) && sound.IsPlaying)
                sound?.Stop();
        }
    }
}
