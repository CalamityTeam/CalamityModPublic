using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Nanotech : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nanotech");
            Tooltip.SetDefault("Rogue projectiles create nanoblades as they travel");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 20;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            int critLevel = Main.player[Main.myPlayer].Calamity().raiderStack;
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                {
                    line2.text = "Rogue projectiles create nanoblades as they travel\n" +
                "Stealth strikes summon lunar flares and sparks on enemy hits\n" +
				"Stealth strikes have +30 armor penetration, deal 10% more damage, and heal for 1 HP\n" +
                "Rogue weapons have a chance to instantly kill normal enemies\n" +
                "10% increased rogue damage, 5% increased rogue crit chance, and 15% increased rogue velocity\n" +
                "Whenever you crit an enemy with a rogue weapon your rogue damage increases\n" +
                "This effect can stack up to 250 times\n" +
                "Max rogue damage boost is 25%\n" +
                "Rogue Crit Level: " + critLevel;
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.nanotech = true;
            modPlayer.moonCrown = true;
            modPlayer.raiderTalisman = true;
            modPlayer.electricianGlove = true;
            player.Calamity().throwingDamage += 0.1f;
            player.Calamity().throwingCrit += 5;
            player.Calamity().throwingVelocity += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<RogueEmblem>());
            recipe.AddIngredient(ModContent.ItemType<RaidersTalisman>());
            recipe.AddIngredient(ModContent.ItemType<MoonstoneCrown>());
            recipe.AddIngredient(ModContent.ItemType<ElectriciansGlove>());

            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 20);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 20);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
