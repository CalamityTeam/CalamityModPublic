using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Prismatic
{
    [AutoloadEquip(EquipType.Head)]
    public class PrismaticHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Prismatic Helmet");
            Tooltip.SetDefault("18% increased magic damage and 12% increased magic crit\n" +
                "20% decreased non-magic damage\n" +
                "Enemies with less than 500 max health deal no contact damage\n" +
                "This does not occur while a boss is alive");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 18; //71

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;
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
            var hotkey = CalamityKeybinds.SetBonusHotKey.TooltipHotkeyString();
            player.setBonus = "+40 max mana and 15% reduced mana cost\n" +
                "Increased mana regeneration rate\n" +
                "Press " + hotkey + " to unleash a barrage of death lasers at the cursor for the next 5 seconds\n" +
                "This has a 30 second cooldown";
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().prismaticHelmet = true;
            player.GetDamage<GenericDamageClass>() -= 0.2f;
            player.GetDamage<MagicDamageClass>() += 0.2f;
            player.GetDamage<MagicDamageClass>() += 0.18f;
            player.GetCritChance<MagicDamageClass>() += 12;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArmoredShell>(3).
                AddIngredient<ExodiumCluster>(5).
                AddIngredient<DivineGeode>(4).
                AddIngredient(ItemID.Nanites, 300).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
