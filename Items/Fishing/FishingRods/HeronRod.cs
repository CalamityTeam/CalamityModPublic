using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Fishing.FishingRods
{
    public class HeronRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Heron Rod");
            Tooltip.SetDefault("Increased fishing power in space.\n" + //John Steinbeck quote but fish instead of snake
                "A silent head and beak lanced down and plucked it out by the head,\n" +
                "and the beak swallowed the little fish while its tail waved frantically.");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 25;
            Item.shootSpeed = 14.5f;
            Item.shoot = ModContent.ProjectileType<HeronBobber>();
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<AerialiteBar>(), 7).
                AddIngredient(ItemID.SunplateBlock, 5).
                AddTile(TileID.SkyMill).
                Register();
        }
    }
}
