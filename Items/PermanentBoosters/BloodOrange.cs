using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

namespace CalamityMod.Items.PermanentBoosters
{
    public class BloodOrange : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Misc";

        public const int LifeBoost = 25;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeBoost);

        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/BloodOrangeConsume");
        public override void SetStaticDefaults()
        {
           	// For some reason Life/Mana boosting items are in this set (along with Magic Mirror+)
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 20; // Life Fruit
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 40;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.LightPurple;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = UseSound;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player) => player.ConsumedLifeFruit == Player.LifeFruitMax;

        public override bool? UseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = Item.useTime;
                if (modPlayer.bOrange)
                {
                    string key = "Mods.CalamityMod.Misc.BloodOrangeText";
                    Color messageColor = Color.Orange;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                    return false;
                }

                player.UseHealthMaxIncreasingItem(LifeBoost);
                modPlayer.bOrange = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip1");

            if (line != null && Main.LocalPlayer.Calamity().bOrange)
                line.Text += "\n" + CalamityUtils.GetTextValue("Misc.GenericConsumedText");
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LifeFruit, 5).
                AddIngredient<BloodOrb>(10).
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
