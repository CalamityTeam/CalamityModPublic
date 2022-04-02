using CalamityMod.NPCs.Yharon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SkyFlareRevenge : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/SkyFlare";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Flare");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 10;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 4)
            {
                projectile.frame = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 53, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item20, projectile.position);
            int dustAmt = 36;
            for (int d = 0; d < dustAmt; d++)
            {
                Vector2 dustSource = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                dustSource = dustSource.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                Vector2 dustVel = dustSource - projectile.Center;
                int idx = Dust.NewDust(dustSource + dustVel, 0, 0, 244, dustVel.X * 2f, dustVel.Y * 2f, 100, default, 1.4f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].noLight = true;
                Main.dust[idx].velocity = dustVel;
            }
            if (projectile.owner == Main.myPlayer)
            {
                int type = ModContent.ProjectileType<InfernadoRevenge>();
                int damage = projectile.GetProjectileDamage(ModContent.NPCType<Yharon>());
                int nado = Projectile.NewProjectile(projectile.Center, Vector2.Zero, type, damage, 4f, Main.myPlayer, 16f, 50f);
                Main.projectile[nado].Bottom = projectile.Center;
                Main.projectile[nado].netUpdate = true;
            }
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
