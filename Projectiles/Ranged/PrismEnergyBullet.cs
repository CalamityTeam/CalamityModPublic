using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class PrismEnergyBullet : ModProjectile
    {
        public ref float CurrentLaserLength => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Prism Rocket");

        public override void SetDefaults()
        {
            Projectile.scale = 1.7f;
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 300)
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            CurrentLaserLength = (int)MathHelper.Lerp(1f, 70f, Utils.GetLerpValue(0f, 15f, Time, true) * Utils.GetLerpValue(0f, 15f, Projectile.timeLeft, true));
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Time++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            for (int i = 0; i < 3; i++)
            {
                Vector2 shootVelocity = Projectile.velocity.RotatedBy(MathHelper.Lerp(-0.3f, 0.3f, i / 2f)) * 0.4f;
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, shootVelocity, ModContent.ProjectileType<PrismComet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 currentDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < CurrentLaserLength; i++)
            {
                float scale = MathHelper.Lerp(1f, 0.2f, i / CurrentLaserLength) * Projectile.scale;
                Vector2 drawPosition = Projectile.Center - currentDirection * i * 4f - Main.screenPosition;
                Color drawColor = Color.Lerp(Color.Lime, Color.YellowGreen, (float)Math.Cos(i / CurrentLaserLength * 2.1f - Main.GlobalTimeWrappedHourly * 2.5f) * 0.5f + 0.5f);
                drawColor = Color.Lerp(drawColor, Color.Yellow, 0.55f);
                drawColor = Color.Lerp(drawColor, Color.White, (float)Math.Pow(i / CurrentLaserLength, 3D));
                drawColor.A = 0;

                Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
