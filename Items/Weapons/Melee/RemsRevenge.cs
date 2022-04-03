using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class RemsRevenge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rem's Revenge");
            Tooltip.SetDefault("Wielded by the most powerful fighter.\n" +
            "Summons blood explosions and lowers enemy defense on hit");
        }

        public override void SetDefaults()
        {
            Item.damage = 375;
            Item.DamageType = DamageClass.Melee;
            Item.width = 44;
            Item.height = 34;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 10f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<RemsRevengeProj>();
            Item.shootSpeed = 12f;
            Item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.BlueMoon).AddIngredient(ItemID.LunarBar, 5).AddIngredient(ModContent.ItemType<Lumenite>(), 10).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
