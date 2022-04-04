using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            Item.damage = 251;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.width = 22;
            Item.height = 24;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.shoot = ModContent.ProjectileType<StratusSphereProj>();
            Item.shootSpeed = 7f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.mana = 30;
            Item.knockBack = 2;
            Item.UseSound = SoundID.Item20;
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.holdStyle = 3;
            Item.value = Item.buyPrice(1, 40, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 6;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.NebulaArcanum).AddIngredient(ModContent.ItemType<Lumenite>(), 5).AddIngredient(ModContent.ItemType<RuinousSoul>(), 4).AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 12).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}

