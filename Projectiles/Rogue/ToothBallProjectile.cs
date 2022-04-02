using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class ToothBallProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ToothBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ball");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.aiStyle = 2;
            projectile.timeLeft = 600;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<ToothBall>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer && projectile.Calamity().stealthStrike)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, -10f, ModContent.ProjectileType<StealthNimbusCloud>(), projectile.damage / 2, 0f, projectile.owner, 1f, 0f);
            }
        }
    }
}
