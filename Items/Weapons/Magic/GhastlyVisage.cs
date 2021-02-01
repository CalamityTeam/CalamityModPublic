using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
            item.damage = 55;
            item.magic = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.mana = 20;
            item.width = 32;
            item.height = 36;
            item.useTime = 27;
            item.useAnimation = 27;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = ItemRarityID.Red;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<GhastlyVisageProj>();
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/GhastlyVisageGlow"));
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<GhastlyVisageProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
