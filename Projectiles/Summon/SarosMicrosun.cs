using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SarosMicrosun : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saros Microsun");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 600;

            Projectile.width = Projectile.height = 62;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Tries to detect a target on a certain distance.
            NPC target = Projectile.Center.MinionHoming(5000f, Owner);

            DoAnimation(); // Does the animation of the minion.

            // Rotates to the left if the projectile's heading to the left and viceversa.
            Projectile.rotation += MathHelper.ToRadians(1f + (Owner.velocity.X * 0.04f)) * Owner.direction;

            // If there's a target, go towards the target.
            if (target != null)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 35f, 0.1f);
                Projectile.netUpdate = true;
            }
        }

        public void DoAnimation()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 6 % Main.projFrames[Projectile.type];
        }

        public override bool PreDraw(ref Color lightColor) // Makes the afterimages of the disk.
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Color afterimageDrawColor = Color.DarkOrange with { A = 25 } * Projectile.Opacity * (1f - i / (float)Projectile.oldPos.Length);
                Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Main.EntitySpriteDraw(texture, afterimageDrawPosition, frame, afterimageDrawColor, Projectile.rotation, origin, Projectile.scale, direction, 0);
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            int dustAmount = 180;
            for (int d = 0; d < dustAmount; d++)
            {
                float angle = MathHelper.TwoPi / dustAmount * d;
                Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(20f, 30f);

                Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.ProfanedFire, velocity);
                spawnDust.noGravity = true;
                spawnDust.color = Color.Lerp(Color.White, Color.Yellow, 0.25f);
                spawnDust.scale = velocity.Length() * 0.25f;
                spawnDust.velocity *= 0.5f;
            }
        }
    }
}
