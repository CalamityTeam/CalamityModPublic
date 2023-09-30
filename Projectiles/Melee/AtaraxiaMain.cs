using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class AtaraxiaMain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private static int NumAnimationFrames = 5;
        private static int AnimationFrameTime = 9;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            DrawOffsetX = -40;
            DrawOriginOffsetY = -3;
            DrawOriginOffsetX = 18;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Light
            Lighting.AddLight(Projectile.Center, 0.45f, 0.1f, 0.1f);

            // Spawn dust with a 1/2 chance
            if (Main.rand.NextBool())
            {
                int idx = Dust.NewDust(Projectile.Center, 1, 1, 90);
                Main.dust[idx].position = Projectile.Center;
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 0.6f;
            }

            // Update animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.frame = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
        }

        // Explodes like Exoblade's Exobeams
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath55, Projectile.Center);

            // Transform the projectile's hitbox into a big explosion
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 140;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);

            Vector2 corner = new Vector2(Projectile.position.X, Projectile.position.Y);
            for (int i = 0; i < 50; i++)
            {
                int idx = Dust.NewDust(corner, Projectile.width, Projectile.height, 86, 0f, 0f, 0, new Color(210, 0, 255), 2.2f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 2.5f;

                idx = Dust.NewDust(corner, Projectile.width, Projectile.height, 118, 0f, 0f, 100, new Color(210, 0, 255), 1.8f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 1.8f;

                idx = Dust.NewDust(corner, Projectile.width, Projectile.height, 71, 0f, 0f, 100, new Color(210, 0, 255), 1.0f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 4.0f;
            }

            // Make the projectile ignore iframes while exploding
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
        }
    }
}
