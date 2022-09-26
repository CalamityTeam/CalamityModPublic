using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

namespace CalamityMod.Items.PermanentBoosters
{
    public class EtherealCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ethereal Core");
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
            Item.rare = ItemRarityID.Red;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item29;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.eCore)
            {
                string key = "Mods.CalamityMod.EtherealCoreText";
                Color messageColor = Color.Purple;
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
                modPlayer.eCore = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (line != null && Main.LocalPlayer.Calamity().eCore)
                line.Text = "[c/8a8a8a:You have already consumed this item]";
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MeteoriteBar, 25).
                AddIngredient<AstralBar>(25).
                AddIngredient(ItemID.FragmentNebula, 20).
                AddIngredient(ItemID.FallenStar, 50).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
