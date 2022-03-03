using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
	public class HauntedScroll : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Haunted Scroll");
            Tooltip.SetDefault("Summons a stack of haunted dishes to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 25;
            item.mana = 10;
            item.width = 52;
            item.height = 52;
            item.useTime = item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<HauntedDishes>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 30f);
				//projectile.ai[1] is attack cooldown.  Setting it here prevents immediate attacks
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.anyWood = true;
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddIngredient(ItemID.Bowl);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
