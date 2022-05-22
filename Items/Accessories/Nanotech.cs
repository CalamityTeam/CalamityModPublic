using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class Nanotech : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Nanotech");
            Tooltip.SetDefault("Rogue projectiles create nanoblades as they travel\n" +
                "Stealth strikes summon nanobeams and sparks on enemy hits\n" +
                "Stealth strikes have +20 armor penetration, deal 5% more damage, and heal for 1 HP\n" +
                "15% increased rogue damage and 15% increased rogue velocity\n" +
                "Whenever you crit an enemy with a rogue weapon your rogue damage increases\n" +
                "This effect can stack up to 150 times\n" +
                "Max rogue damage boost is 15%\n" +
                "This line is modified below");
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            int critLevel = Main.player[Main.myPlayer].Calamity().raiderStack;
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip7");

            if (line != null)
                line.Text = "Rogue Crit Level: " + critLevel;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.nanotech = true;
            modPlayer.raiderTalisman = true;
            modPlayer.electricianGlove = true;
            modPlayer.filthyGlove = true;
            modPlayer.bloodyGlove = true;
            player.GetDamage<ThrowingDamageClass>() += 0.15f;
            player.Calamity().rogueVelocity += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RogueEmblem>().
                AddIngredient<RaidersTalisman>().
                AddIngredient<MoonstoneCrown>().
                AddIngredient<ElectriciansGlove>().
                AddIngredient(ItemID.LunarBar, 8).
                AddIngredient<GalacticaSingularity>(4).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
