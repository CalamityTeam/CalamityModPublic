using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DaedalusHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Daedalus Helm");
            Tooltip.SetDefault("10% increased melee damage and critical strike chance\n" +
                "15% increased melee speed");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 21; //51
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
            player.setBonus = "5% increased melee damage\n" +
                "Enemies are more likely to target you\n" +
                "You reflect projectiles back at enemies\n" +
                "Reflected projectiles deal 50% less damage to you\n" +
                "This reflect has a 90 second cooldown which is shared with all other dodges and reflects";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.daedalusReflect = true;
            player.GetDamage<MeleeDamageClass>() += 0.05f;
            player.aggro += 500;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed<MeleeDamageClass>() += 0.15f;
            player.GetDamage<MeleeDamageClass>() += 0.1f;
            player.GetCritChance<MeleeDamageClass>() += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VerstaltiteBar>(8).
                AddIngredient(ItemID.CrystalShard, 3).
                AddIngredient<EssenceofEleum>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
