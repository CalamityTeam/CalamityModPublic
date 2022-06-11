using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class PhotosynthesisPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 20;
            DisplayName.SetDefault("Photosynthesis Potion");
            Tooltip.SetDefault("You regen life quickly while not moving, this effect is five times as strong during daytime\n" +
                "Dropped hearts heal more HP");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<PhotosynthesisBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BeetleJuice>().
                AddIngredient<TrapperBulb>().
                AddIngredient<EssenceofSunlight>().
                AddTile(TileID.AlchemyTable).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(40).
                AddIngredient<EssenceofSunlight>().
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}
