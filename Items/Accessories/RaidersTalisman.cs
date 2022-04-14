using CalamityMod.CalPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class RaidersTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Raider's Talisman");
            Tooltip.SetDefault("Whenever you crit an enemy with a rogue weapon your rogue damage increases\n" +
                "This effect can stack up to 150 times\n" +
                "Max rogue damage boost is 15%\n" +
                "This line is modified below");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            int critLevel = Main.player[Main.myPlayer].Calamity().raiderStack;
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip3");

            if (line != null)
                line.Text = "Rogue Crit Level: " + critLevel;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.raiderTalisman = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Leather, 5).
                AddIngredient(ItemID.Obsidian, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
