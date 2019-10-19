using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Viscera : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Viscera");
            Tooltip.SetDefault("Fires a blood beam that heals you on enemy hits\n" +
                "The more tiles and enemies the beam bounces off of or travels through the more healing the beam does");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 153;
            item.magic = true;
            item.mana = 15;
            item.width = 50;
            item.height = 52;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Viscera>();
            item.shootSpeed = 6f;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
