using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shoes)]
    public class AngelTreads : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angel Treads");
            Tooltip.SetDefault("Extreme speed!\n" +
                               "36% increased running acceleration\n" +
                               "Increased flight time\n" +
                               "Greater mobility on ice\n" +
                               "Water and lava walking\n" +
                               "Immunity to the On Fire! debuff\n" +
                               "Temporary immunity to lava");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity6BuyPrice;
            item.rare = ItemRarityID.LightPurple;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.angelTreads = true;
            player.accRunSpeed = 7.5f;
            player.rocketBoots = 3;
            player.moveSpeed += 0.12f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 240;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FrostsparkBoots);
            recipe.AddIngredient(ItemID.LavaWaders);
            recipe.AddIngredient(ModContent.ItemType<HarpyRing>());
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 5);
            recipe.AddIngredient(ItemID.SoulofMight);
            recipe.AddIngredient(ItemID.SoulofSight);
            recipe.AddIngredient(ItemID.SoulofFright);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
