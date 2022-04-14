using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shoes)]
    public class AngelTreads : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            Item.width = 36;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.accessory = true;
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
            CreateRecipe().
                AddIngredient(ItemID.TerrasparkBoots).
                AddIngredient<HarpyRing>().
                AddIngredient<EssenceofCinder>(5).
                AddIngredient(ItemID.SoulofMight).
                AddIngredient(ItemID.SoulofSight).
                AddIngredient(ItemID.SoulofFright).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
