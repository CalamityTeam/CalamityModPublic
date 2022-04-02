using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;

namespace CalamityMod.Items.Weapons.Magic
{
    public class StratusSphere : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stratus Sphere");
            Tooltip.SetDefault("Fires an energy orb containing the essence of our stratosphere\n" +
                "Up to six of these can be active at a time");
        }
        public override void SetDefaults()
        {
            item.damage = 251;
            item.noMelee = true;
            item.magic = true;
            item.width = 22;
            item.height = 24;
            item.useTime = 45;
            item.useAnimation = 45;
            item.shoot = ModContent.ProjectileType<StratusSphereProj>();
            item.shootSpeed = 7f;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.mana = 30;
            item.knockBack = 2;
            item.UseSound = SoundID.Item20;
            item.rare = ItemRarityID.LightRed;
            item.autoReuse = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.holdStyle = 3;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 6;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.NebulaArcanum);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 5);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 4);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

