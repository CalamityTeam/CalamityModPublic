using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class MinionPlasmaBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Boss/AresPlasmaFireball";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 420;
            Projectile.Opacity = 0f;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            // Accelerate.
            if (Projectile.velocity.Length() < 23f)
                Projectile.velocity *= 1.0175f;

            // Fade in.
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.06667f, 0f, 1f);

            // Handle frames and rotation.
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Projectile.type];
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            // Home towards targets.
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(900f, false);
            if (potentialTarget != null)
                Projectile.velocity = Projectile.SuperhomeTowardsTarget(potentialTarget, 23f, 10f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);

            // Release a spread of plasma gas.
            if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.ai[1] != -1f)
            {
                int type = ModContent.ProjectileType<MinionPlasmaGas>();
                for (int i = 0; i < 15; i++)
                {
                    Vector2 plasmaVelocity = Main.rand.NextVector2Circular(5f, 5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, plasmaVelocity, type, Projectile.damage, 0f, Main.myPlayer);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
