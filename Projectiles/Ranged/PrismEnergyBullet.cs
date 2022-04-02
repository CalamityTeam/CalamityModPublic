using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PrismEnergyBullet : ModProjectile
    {
        public ref float CurrentLaserLength => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Prism Rocket");

        public override void SetDefaults()
        {
            projectile.scale = 1.7f;
            projectile.width = projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 13;
            projectile.extraUpdates = 1;
            projectile.ranged = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft == 300)
                Main.PlaySound(SoundID.Item14, projectile.Center);

            CurrentLaserLength = (int)MathHelper.Lerp(1f, 70f, Utils.InverseLerp(0f, 15f, Time, true) * Utils.InverseLerp(0f, 15f, projectile.timeLeft, true));
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Time++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            for (int i = 0; i < 3; i++)
            {
                Vector2 shootVelocity = projectile.velocity.RotatedBy(MathHelper.Lerp(-0.3f, 0.3f, i / 2f)) * 0.4f;
                Projectile.NewProjectile(projectile.Center, shootVelocity, ModContent.ProjectileType<PrismComet>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 currentDirection = projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < CurrentLaserLength; i++)
            {
                float scale = MathHelper.Lerp(1f, 0.2f, i / CurrentLaserLength) * projectile.scale;
                Vector2 drawPosition = projectile.Center - currentDirection * i * 4f - Main.screenPosition;
                Color drawColor = Color.Lerp(Color.Lime, Color.YellowGreen, (float)Math.Cos(i / CurrentLaserLength * 2.1f - Main.GlobalTime * 2.5f) * 0.5f + 0.5f);
                drawColor = Color.Lerp(drawColor, Color.Yellow, 0.55f);
                drawColor = Color.Lerp(drawColor, Color.White, (float)Math.Pow(i / CurrentLaserLength, 3D));
                drawColor.A = 0;

                spriteBatch.Draw(texture, drawPosition, null, drawColor, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
