using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Lacerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacerator");
            Tooltip.SetDefault("Enemies that are hit by the yoyo will have their life drained\n" +
            "A very agile yoyo\n" +
            "Someone thought this was a viable weapon against DoG at one point lol");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 107;
            Item.knockBack = 7f;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<LaceratorYoyo>();
            Item.shootSpeed = 16f;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(4).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
