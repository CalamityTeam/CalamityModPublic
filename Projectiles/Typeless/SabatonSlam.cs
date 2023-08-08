using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using System;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Typeless
{
    public class SabatonSlam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public float scaleFromFall;
        public float damageScaleFromFall;
        public int timeLeft = 60;
        public bool ableToHit = true;

        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {

            if (Projectile.timeLeft <= 40)
            {
                ableToHit = false;
            }
            if (Projectile.localAI[0] == 0)
            {
                scaleFromFall = (Projectile.ai[0] / 22) + 0.5f;
                damageScaleFromFall = Projectile.ai[0] / 40;
                Projectile.damage = (int)(300f * damageScaleFromFall + 300f);

                SoundEngine.PlaySound(new("CalamityMod/Sounds/Custom/GravistarSlam") { Volume = 0.75f }, Projectile.Center);

                //Spawn particles, but also increase the count to fill more of the circle the bigger it is
                int particleCount = (int)(10 * scaleFromFall);
                for (int i = 0; i < particleCount; i++)
                {
                    SquareParticle square = new SquareParticle(Projectile.Center + Main.rand.NextVector2Circular(scaleFromFall * 74f, scaleFromFall * 74f), Main.rand.NextVector2Circular(2.5f, 2.5f), false, 120, 3.5f + Main.rand.NextFloat(0.6f), Color.Lerp(Color.Cyan, Color.LightCyan, 0.75f));
                    GeneralParticleHandler.SpawnParticle(square);
                }

                Projectile.localAI[0]++;
            }
                
            
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion();
            Texture2D telegraphBase = ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value;

            GameShaders.Misc["CalamityMod:CircularGradientWithEdge"].UseOpacity(0.75f * Projectile.timeLeft/(float)timeLeft);
            GameShaders.Misc["CalamityMod:CircularGradientWithEdge"].UseColor(Color.Lerp(Color.Cyan, Color.LightCyan, 0.5f));
            GameShaders.Misc["CalamityMod:CircularGradientWithEdge"].UseSecondaryColor(Color.White);
            GameShaders.Misc["CalamityMod:CircularGradientWithEdge"].UseSaturation(scaleFromFall);

            GameShaders.Misc["CalamityMod:CircularGradientWithEdge"].Apply();

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, lightColor, 0, telegraphBase.Size() / 2f, scaleFromFall*156f, 0, 0);
            Main.spriteBatch.ExitShaderRegion();

            return false;
        }
        public override bool? CanDamage() => ableToHit ? (bool?)null : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, (scaleFromFall * 78f), targetHitbox);
    }
}
