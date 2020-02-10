using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class GhastlyVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghastly Visage");
            Tooltip.SetDefault("Fires homing ghast energy that explodes");
        }

        public override void SetDefaults()
        {
            item.damage = 92;
            item.magic = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.mana = 20;
            item.width = 32;
            item.height = 36;
            item.useTime = 27;
            item.useAnimation = 27;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<GhastlyVisageProj>();
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == ModContent.ProjectileType<GhastlyVisageProj>() && p.owner == player.whoAmI)
                {
                    return false;
                }
            }
            return true;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 origin = new Vector2(16f, 16f);
			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/GhastlyVisageGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<GhastlyVisageProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
