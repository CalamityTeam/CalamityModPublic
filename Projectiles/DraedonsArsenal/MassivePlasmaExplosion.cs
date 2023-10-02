using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;
using Terraria.ID;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class MassivePlasmaExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float lightAmt = 1f;

        public int frameX = 0;
        public int frameY = 0;
        private const int horizontalFrames = 4;
        private const int verticalFrames = 5;
        private const int frameLength = 5;
        private const float radius = 191.5f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 383;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = frameLength * horizontalFrames * verticalFrames / 2;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % frameLength == frameLength - 1)
            {
                frameY++;
                if (frameY >= verticalFrames)
                {
                    frameX++;
                    frameY = 0;
                }
                if (frameX >= horizontalFrames)
                {
                    Projectile.Kill();
                }
            }

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 4f * lightAmt);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.FlareSound, Projectile.Center);
                Projectile.localAI[0] = 1f;
            }
            lightAmt = (float)Math.Sin(Time / 37 * MathHelper.Pi) * 2f;
            if (lightAmt > 1f)
                lightAmt = 1f;
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.CursedInferno, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int length = texture.Width / horizontalFrames;
            int height = texture.Height / verticalFrames;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle frame = new Rectangle(frameX * length, frameY * height, length, height);
            Vector2 origin = new Vector2(length / 2f, height / 2f);
            Main.EntitySpriteDraw(texture, drawPos, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
