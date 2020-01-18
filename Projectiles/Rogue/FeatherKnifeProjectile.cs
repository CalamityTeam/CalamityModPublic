using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class FeatherKnifeProjectile : ModProjectile
    {
        private int featherTimer = 35;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Knife");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.aiStyle = 2; 
            projectile.timeLeft = 600;
            aiType = 48;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            if (featherTimer == 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(projectile.position, new Vector2(projectile.velocity.X / 20, 2), ModContent.ProjectileType<StickyFeatherAero>(), (int)((double)projectile.damage * 0.4), projectile.knockBack, projectile.owner);
                }
                featherTimer = 35;
            }
            featherTimer--;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(projectile.position, new Vector2(projectile.velocity.X / 20, 2), ModContent.ProjectileType<StickyFeatherAero>(), (int)((double)projectile.damage * 0.69), projectile.knockBack, projectile.owner);
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2) && !projectile.Calamity().stealthStrike)
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<FeatherKnife>());
            }
        }
    }
}
