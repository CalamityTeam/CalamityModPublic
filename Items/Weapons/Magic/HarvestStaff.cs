using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class HarvestStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvest Staff");
            Tooltip.SetDefault("Casts flaming pumpkins");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 26;
            item.magic = true;
            item.mana = 5;
            item.width = 46;
            item.height = 44;
            item.useTime = 23;
            item.useAnimation = 23;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FlamingPumpkin>();
            item.shootSpeed = 10f;
            item.scale = 0.9f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Pumpkin, 20);
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
