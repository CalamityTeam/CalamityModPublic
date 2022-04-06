using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AtaxiaHeadgear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydrothermic Headgear");
            Tooltip.SetDefault("12% increased ranged damage and 10% increased ranged critical strike chance\n" +
                "Reduces ammo usage by 25%, temporary immunity to lava, and immunity to fire damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 15; //53
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AtaxiaArmor>() && legs.type == ModContent.ItemType<AtaxiaSubligar>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
            player.Calamity().hydrothermalSmoke = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased ranged damage\n" +
                "Inferno effect when below 50% life\n" +
                "You fire a homing chaos flare when using ranged weapons every 0.33 seconds\n" +
                "You emit a blazing explosion when you are hit";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.ataxiaBlaze = true;
            modPlayer.ataxiaBolt = true;
            player.GetDamage(DamageClass.Ranged) += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.ammoCost75 = true;
            player.GetDamage(DamageClass.Ranged) += 0.12f;
            player.GetCritChance(DamageClass.Ranged) += 10;
            player.lavaMax += 240;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CruptixBar>(7).
                AddIngredient(ItemID.HellstoneBar, 4).
                AddIngredient<CoreofChaos>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
