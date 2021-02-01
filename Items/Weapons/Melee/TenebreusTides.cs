using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TenebreusTides : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tenebreus Tides");
            Tooltip.SetDefault("Inundatio ex Laminis\n" +
                "Shoots a water spear that pierces enemies and terrain\n" +
                "Striking enemies summon liquid blades and spears to assault the struck foe");
        }

        public override void SetDefaults()
        {
            item.damage = 80;
            item.knockBack = 4.5f;
            item.melee = true;
            item.useAnimation = item.useTime = 14;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<TenebreusTidesProjectile>();
            item.shootSpeed = 12f;

            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.rare = ItemRarityID.Cyan;
            item.Calamity().donorItem = true;

            item.width = item.height = 72;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AmidiasTrident>());
            recipe.AddIngredient(ModContent.ItemType<Atlantis>());
            recipe.AddIngredient(ItemID.InfluxWaver);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 25);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
