using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheGodsGambit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The God's Gambit");
            Tooltip.SetDefault("Fires a stream of slime when enemies are near\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 26;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 29;
            Item.knockBack = 3.5f;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<GodsGambitYoyo>();
            Item.shootSpeed = 10f;

            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 12);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PurifiedGel>(30).
                AddTile<StaticRefiner>().
                Register();
        }
    }
}
