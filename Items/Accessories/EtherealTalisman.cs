using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class EtherealTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Ethereal Talisman");
            Tooltip.SetDefault("15% increased magic damage, 5% increased magic critical strike chance and 10% decreased mana usage\n" +
                "+150 max mana\n" +
                "Increases pickup range for mana stars\n" +
                "You automatically use mana potions when needed if visibility is on");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.eTalisman = true;

            player.manaMagnet = true;
            if (!hideVisual)
                player.manaFlower = true;

            player.statManaMax2 += 150;
            player.GetDamage(DamageClass.Magic) += 0.15f;
            player.manaCost *= 0.9f;
            player.GetCritChance(DamageClass.Magic) += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SigilofCalamitas>().
                AddIngredient(ItemID.ManaFlower).
                AddIngredient(ItemID.LunarBar, 8).
                AddIngredient<GalacticaSingularity>(4).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
