using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Tarragon
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("TarragonMask")]
    public class TarragonHeadMagic : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Tarragon Mask");
            Tooltip.SetDefault("Temporary immunity to lava\n" +
                "Can move freely through liquids\n" +
                "20% increased magic damage and 10% increased magic critical strike chance\n" +
                "5% increased damage reduction, +100 max mana, and 15% reduced mana usage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 50, 0, 0);
            Item.defense = 10; //98
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
            modPlayer.tarraMage = true;
            player.setBonus = "Reduces enemy spawn rates\n" +
                "Increased heart pickup range\n" +
                "Enemies have a chance to drop extra hearts on death\n" +
                "On every 5th critical strike you will fire a leaf storm\n" +
                "Magic projectiles heal you on enemy hits\n" +
                "Amount healed is based on projectile damage";
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost *= 0.85f;
            player.GetDamage<MagicDamageClass>() += 0.2f;
            player.GetCritChance<MagicDamageClass>() += 10;
            player.endurance += 0.05f;
            player.lavaMax += 240;
            player.statManaMax2 += 100;
            player.ignoreWater = true;
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
