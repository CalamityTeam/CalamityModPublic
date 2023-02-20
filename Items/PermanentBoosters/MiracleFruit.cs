using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

namespace CalamityMod.Items.PermanentBoosters
{
    public class MiracleFruit : ModItem
    {
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/MiracleFruitConsume");
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miracle Fruit");
            Tooltip.SetDefault("Refreshing and cool, perhaps even a bit minty\n" +
                               "Permanently increases maximum life by 25\n" +
                               "Can only be used if the max amount of life fruit has been consumed");
            SacrificeTotal = 1;
			// For some reason Life/Mana boosting items are in this set (along with Magic Mirror+)
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 18; // Life Fruit
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 36;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.Yellow;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = UseSound;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.mFruit)
            {
                string key = "Mods.CalamityMod.MiracleFruitText";
                Color messageColor = Color.GreenYellow;
                CalamityUtils.DisplayLocalizedText(key, messageColor);

                return false;
            }
            else if (player.statLifeMax < 500)
            {
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
                    player.HealEffect(25);
                }
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.mFruit = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (line != null && Main.LocalPlayer.Calamity().mFruit)
                line.Text = "[c/8a8a8a:You have already consumed this item]";
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LifeFruit, 5).
                AddIngredient<TrapperBulb>(5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<LivingShard>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
