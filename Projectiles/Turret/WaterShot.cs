using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Turret
{
    public class WaterShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";


        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water Shot");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 180;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 7;
        }

        public override bool PreAI()
        {
            // If projectile knockback is set to 0 in the tile entity file, projectile hits players instead
            // This is used to check if the projectile came from the hostile version of the tile entity
            if (Projectile.knockBack == 0f)
                Projectile.hostile = true;
            else Projectile.friendly = true;
            return true;
        }

        public override void AI()
        {
            float fallSpeedCap = 10f;
            float downwardsAccel = 0.08f;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.velocity.Y -= 1.5f; // Add vertical velocity at the start
                // play a sound frame 1.
                var sound = SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PlantyMushMine", 3) with { Volume = 0.65f }, Projectile.Center);

                Projectile.localAI[0] = 1f;
            }
            if (Projectile.velocity.Y < fallSpeedCap)
                Projectile.velocity.Y += downwardsAccel;
            if (Projectile.velocity.Y > fallSpeedCap)
                Projectile.velocity.Y = fallSpeedCap;
            Projectile.velocity.X *= 0.995f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Wet, 240);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Wet, 240);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Color color = new Color(59, 175, 252);
                color.A = 0;
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(-32f, -32f); //Last vector is to offset the circle so that it is displayed where the hitbox actually is, instead of a bit down and to the right.
                Color outerColor = color;
                Color innerColor = color * 0.2f;
                float intensity = 0.6f;
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                if (Projectile.timeLeft <= 45) //Shrinks to nothing when projectile is nearing death
                {
                    intensity *= Projectile.timeLeft / 45f;
                }
                // Become smaller the futher along the old positions we are.
                Vector2 outerScale = new Vector2(1f) * intensity;
                Vector2 innerScale = new Vector2(1f) * intensity * 0.7f;
                outerColor *= intensity;
                innerColor *= intensity;
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.40f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.40f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
