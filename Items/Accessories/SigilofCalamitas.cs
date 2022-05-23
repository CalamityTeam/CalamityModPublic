using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SigilofCalamitas : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Sigil of Calamitas");
            Tooltip.SetDefault("15% increased magic damage and 10% decreased mana usage\n" +
                "+100 max mana\n" +
                "Increases pickup range for mana stars");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 8));
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaMagnet = true;
            player.statManaMax2 += 100;
            player.GetDamage(DamageClass.Magic) += 0.15f;
            player.manaCost *= 0.9f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CelestialEmblem).
                AddRecipeGroup("AnyEvilWater", 10).
                AddIngredient<CalamityDust>(5).
                AddIngredient<CoreofChaos>(5).
                AddIngredient<CruptixBar>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
