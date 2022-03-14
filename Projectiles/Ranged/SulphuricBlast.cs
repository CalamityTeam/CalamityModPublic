using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SulphuricBlast : ModProjectile
    {
        public const int TotalSecondsToStick = 8;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphuric Blast");
            Main.projFrames[projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.MaxUpdates = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = projectile.MaxUpdates * 12;
        }

        public override void AI()
        {
            if (projectile.FinalExtraUpdate())
                projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 4 % Main.projFrames[projectile.type];

            projectile.StickyProjAI(8);
            projectile.Opacity = Utils.InverseLerp(CalamityUtils.SecondsToFrames(TotalSecondsToStick), CalamityUtils.SecondsToFrames(TotalSecondsToStick * 0.5f), projectile.localAI[0], true);

            // Emit sulpuric gas.
            if (Main.netMode != NetmodeID.Server && projectile.FinalExtraUpdate() && projectile.velocity.Length() > 3f)
            {
                Color color = new Color(136, 211, 113, 127);
                Color fadeColor = new Color(165, 165, 86);
                Vector2 gasSpawnPosition = projectile.Center + Main.rand.NextVector2Circular(8f, 8f);
                Vector2 gasVelocity = projectile.velocity * 1.2f + projectile.velocity.RotatedBy(0.75f) * 0.3f;
                gasVelocity *= Main.rand.NextFloat(0.24f, 0.6f);

                Particle gas = new MediumMistParticle(gasSpawnPosition, gasVelocity, color, fadeColor, Main.rand.NextFloat(0.5f, 1f), 205 - Main.rand.Next(50), 0.02f);
                GeneralParticleHandler.SpawnParticle(gas);
            }

            // Home in on enemies if not sticking to anything.
            if (projectile.ai[0] != 1f)
            {
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 720f, 16f, projectile.MaxUpdates * 20f);
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(2, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
