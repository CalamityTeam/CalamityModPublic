using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Umbraphile
{
    [AutoloadEquip(EquipType.Head)]
    public class UmbraphileHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Umbraphile Hood");
            Tooltip.SetDefault("8% increased rogue damage and 10% increased rogue velocity");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 8; //36
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<UmbraphileRegalia>() && legs.type == ModContent.ItemType<UmbraphileBoots>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.umbraphileSet = true;
            modPlayer.rogueStealthMax += 1.1f;
            player.setBonus = "+110 maximum stealth\n" +
				"Rogue weapons have a chance to create explosions on hit\n" +
                "Stealth strikes always create an explosion\n" +
                "Penumbra potions always build stealth at max effectiveness";
            player.Calamity().wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += 0.08f;
            player.Calamity().rogueVelocity += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SolarVeil>(12).
                AddIngredient(ItemID.HallowedBar, 8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
