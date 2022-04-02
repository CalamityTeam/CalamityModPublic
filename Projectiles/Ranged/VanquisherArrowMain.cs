using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class VanquisherArrowMain : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Ammo/VanquisherArrow";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.arrow = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 90;
            projectile.extraUpdates = 1;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (projectile.timeLeft % 45 == 0)
            {
                if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<VanquisherArrowSplit>()] < 6)
                    Projectile.NewProjectile(projectile.Center, projectile.velocity * 0.25f, ModContent.ProjectileType<VanquisherArrowSplit>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color = Color.White;
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * (b2 / 255f));
                color = new Color(b2, b2, b2, a2);
            }
            Rectangle frame = new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Ammo/VanquisherArrowGlow"), projectile.Center - Main.screenPosition, frame, color, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
        }
    }
}
