using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class DeathstareBeam : ModProjectile
    {
        public ref float OwnerUUID => ref projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 10;
        }
        public override void AI()
        {
            if (!Main.projectile.IndexInRange((int)OwnerUUID))
            {
                projectile.Kill();
                return;
            }

            projectile.Opacity = Utils.InverseLerp(1f, 0f, 1f - projectile.timeLeft / 10f, true);
            projectile.Center = Main.projectile[(int)OwnerUUID].Center - projectile.velocity;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D beamTexture = Main.projectileTexture[projectile.type];
            Vector2 drawPosition = projectile.Center + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Vector2 drawScale = new Vector2(0.55f, projectile.velocity.Length() / beamTexture.Height * 20f);
            Color color = Color.White * 2.1f * projectile.Opacity;

            if (Math.Abs(projectile.rotation) > 0.008f)
                spriteBatch.Draw(beamTexture, drawPosition, null, color, projectile.rotation, beamTexture.Frame().Bottom(), drawScale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
