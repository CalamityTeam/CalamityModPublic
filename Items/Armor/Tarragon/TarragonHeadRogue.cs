using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Tarragon
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("TarragonHelmet")]
    public class TarragonHeadRogue : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.defense = 15; //98
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TarragonBreastplate>() && legs.type == ModContent.ItemType<TarragonLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraThrowing = true;
            modPlayer.rogueStealthMax += 1.15f;
            modPlayer.wearingRogueArmor = true;
            player.setBonus = "Reduces enemy spawn rates\n" +
                "Increased heart pickup range\n" +
                "Enemies have a chance to drop extra hearts on death\n" +
				"+115 maximum stealth\n" +
                "After every 25 rogue critical hits you will gain 3 seconds of damage immunity\n" +
                "This effect can only occur once every 30 seconds\n" +
                "While under the effects of a debuff you gain 10% increased rogue damage";
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += 0.1f;
            player.GetCritChance<ThrowingDamageClass>() += 10;
            player.moveSpeed += 0.05f;
            player.endurance += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomBar>(7).
                AddIngredient<DivineGeode>(6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
