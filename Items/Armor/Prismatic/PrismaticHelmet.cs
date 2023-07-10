using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Prismatic
{
    [AutoloadEquip(EquipType.Head)]
    public class PrismaticHelmet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        internal static string LaserEntitySourceContext => "SetBonus_Calamity_Prismatic";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 18; //71

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
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
            player.setBonus = this.GetLocalization("SetBonus").Format(hotkey);
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
