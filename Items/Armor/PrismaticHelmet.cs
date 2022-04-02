using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class PrismaticHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Helmet");
            Tooltip.SetDefault("18% increased magic damage and 12% increased magic crit\n" +
                "20% decreased non-magic damage\n" +
                "Enemies with less than 500 max health deal no contact damage\n" +
                "This does not occur while a boss is alive");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.defense = 18; //71

            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().donorItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<PrismaticRegalia>() && legs.type == ModContent.ItemType<PrismaticGreaves>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.Calamity().prismaticSet = true;
            player.statManaMax2 += 40;
            player.manaCost *= 0.85f;
            player.manaRegenBonus += 8;
            string hotkey = CalamityMod.TarraHotKey.TooltipHotkeyString();
            player.setBonus = "+40 max mana and 15% reduced mana cost\n" +
                "Increased mana regeneration rate\n" +
                "Press " + hotkey + " to unleash a barrage of death lasers at the cursor for the next 5 seconds\n" +
                "This has a 30 second cooldown";
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().prismaticHelmet = true;
            player.allDamage -= 0.2f;
            player.magicDamage += 0.2f;
            player.magicDamage += 0.18f;
            player.magicCrit += 12;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ArmoredShell>(), 3);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 4);
            recipe.AddIngredient(ItemID.Nanites, 300);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
