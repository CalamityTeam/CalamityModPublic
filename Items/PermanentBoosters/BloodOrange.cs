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
    public class BloodOrange : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Orange");
            Tooltip.SetDefault("It has a distinctly sweet flavor and a strong aroma\n" +
                               "Permanently increases maximum life by 25\n" +
                               "Can only be used if the max amount of life fruit has been consumed\n" +
                               "");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.LightPurple;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.bOrange)
            {
                string key = "Mods.CalamityMod.BloodOrangeText";
                Color messageColor = Color.Orange;
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
                modPlayer.bOrange = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (line != null && Main.LocalPlayer.Calamity().bOrange)
                line.Text = "[c/8a8a8a:You have already consumed this item]";
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LifeFruit, 5).
                AddIngredient(ItemID.OrangeBloodroot).
                AddIngredient<BloodOrb>(10).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddIngredient(ItemID.SoulofFright, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
