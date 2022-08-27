using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

namespace CalamityMod.Items.PermanentBoosters
{
    public class PhantomHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Heart");
            Tooltip.SetDefault("Permanently increases maximum mana by 50");
            SacrificeTotal = 1;
			// For some reason Life/Mana boosting items are in this set (along with Magic Mirror+)
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 19; // Mana Crystal
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item29;
            Item.consumable = true;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.pHeart)
            {
                string key = "Mods.CalamityMod.PhantomHeartText";
                Color messageColor = Color.Magenta;
                CalamityUtils.DisplayLocalizedText(key, messageColor);

                return false;
            }
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = Item.useTime;
                if (Main.myPlayer == player.whoAmI)
                {
                    player.ManaEffect(50);
                }
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.pHeart = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (line != null && Main.LocalPlayer.Calamity().pHeart)
                line.Text = "[c/8a8a8a:You have already consumed this item]";
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RuinousSoul>(10).
                AddIngredient<Phantoplasm>(100).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
