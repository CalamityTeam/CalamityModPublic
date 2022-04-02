using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class FeatherKnifeProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/FeatherKnife";

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
            aiType = ProjectileID.ThrowingKnife;
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
            if (projectile.timeLeft % 35 == 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(projectile.position, new Vector2(projectile.velocity.X / 20, 2), ModContent.ProjectileType<StickyFeatherAero>(), (int)(projectile.damage * 0.4), projectile.knockBack, projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().forceRogue = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int proj = Projectile.NewProjectile(projectile.position, new Vector2(projectile.velocity.X / 20, 2), ModContent.ProjectileType<StickyFeatherAero>(), (int)(projectile.damage * 0.69), projectile.knockBack, projectile.owner);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().forceRogue = true;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2) && !projectile.noDropItem)
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<FeatherKnife>());
        }
    }
}
