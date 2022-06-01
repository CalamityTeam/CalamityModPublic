using CalamityMod.Items.Placeables;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace CalamityMod.Items.Potions
{
    public class SunkenStew : ModItem
    {
        public static int BuffType = BuffID.WellFed2;
        public static int BuffDuration = 216000;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hadal Stew");
            Tooltip.SetDefault("Only gives 50 seconds of Potion Sickness\n" +
               "{$CommonItemTooltip.MediumStats}\n" +
               "60 minute duration");
               SacrificeTotal = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.potion = true;
            Item.healLife = 120;
            Item.healMana = 150;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (Main.LocalPlayer.pStone)
            {
                TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");

                if (line != null)
                    line.Text = "Only gives 37 seconds of Potion Sickness";
            }
        }

        public override bool CanUseItem(Player player)
        {
            return player.potionDelay <= 0 && player.Calamity().potionTimer <= 0;
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffType, BuffDuration);
            // fixes hardcoded potion sickness duration from quick heal (see CalamityPlayerMiscEffects.cs)
            player.Calamity().potionTimer = 2;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<AbyssGravel>(10).
                AddIngredient<CoastalDemonfish>().
                AddIngredient(ItemID.Honeyfin).
                AddIngredient(ItemID.Bowl, 2).
                AddTile(TileID.CookingPots).
                Register();

            CreateRecipe(2).
                AddIngredient<Voidstone>(10).
                AddIngredient<CoastalDemonfish>().
                AddIngredient(ItemID.Honeyfin).
                AddIngredient(ItemID.Bowl, 2).
                AddTile(TileID.CookingPots).
                Register();
        }
    }
}
