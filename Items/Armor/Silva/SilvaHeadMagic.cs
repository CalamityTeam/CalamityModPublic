using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Silva
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("SilvaMaskedCap")]
    public class SilvaHeadMagic : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Silva Masked Cap");
            Tooltip.SetDefault("23% increased magic damage and 13% increased magic critical strike chance\n" +
                "+100 max mana and 19% reduced mana usage");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.defense = 21; //110
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SilvaArmor>() && legs.type == ModContent.ItemType<SilvaLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.silvaSet = true;
            modPlayer.silvaMage = true;
            player.setBonus = "All projectiles spawn healing leaf orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 5%\n" +
                "If you are reduced to 1 HP you will not die from any further damage for 8 seconds\n" +
                "This effect has a 5 minute cooldown. The cooldown does not decrement if any bosses or events are active.\n" +
                "Magic projectiles which cannot pierce will occasionally set off potent blasts of nature energy";
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost *= 0.81f;
            player.GetDamage<MagicDamageClass>() += 0.23f;
            player.GetCritChance<MagicDamageClass>() += 13;
            player.statManaMax2 += 100;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EffulgentFeather>(5).
                AddRecipeGroup("AnyGoldBar", 5).
                AddIngredient<PlantyMush>(6).
                AddIngredient<AscendantSpiritEssence>(2).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
