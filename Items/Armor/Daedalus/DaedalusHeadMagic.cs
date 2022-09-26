using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Daedalus
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("DaedalusHat")]
    public class DaedalusHeadMagic : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Daedalus Hood");
            Tooltip.SetDefault("13% increased magic damage and 7% increased magic critical strike chance\n" +
                "10% decreased mana usage and +60 max mana");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 5; //35
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DaedalusBreastplate>() && legs.type == ModContent.ItemType<DaedalusLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased magic damage\n" +
                "You have a 10% chance to absorb physical attacks and projectiles when hit\n" +
                "If you absorb an attack you are healed for 1/2 of that attack's damage";
            var modPlayer = player.Calamity();
            modPlayer.daedalusAbsorb = true;
            player.GetDamage<MagicDamageClass>() += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost *= 0.9f;
            player.GetDamage<MagicDamageClass>() += 0.13f;
            player.GetCritChance<MagicDamageClass>() += 7;
            player.statManaMax2 += 60;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>(8).
                AddIngredient(ItemID.CrystalShard, 3).
                AddIngredient<EssenceofEleum>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
