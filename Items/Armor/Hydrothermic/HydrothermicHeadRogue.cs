using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Hydrothermic
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("AtaxiaHood")]
    public class HydrothermicHeadRogue : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Hydrothermic Hood");
            Tooltip.SetDefault("12% increased rogue damage and 10% increased rogue critical strike chance\n" +
                "50% chance to not consume rogue items and 5% increased movement speed\n" +
                "Temporary immunity to lava and immunity to fire damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 12; //49
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HydrothermicArmor>() && legs.type == ModContent.ItemType<HydrothermicSubligar>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
            player.Calamity().hydrothermalSmoke = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased rogue damage\n" +
                "Inferno effect when below 50% life\n" +
                "Rogue weapons unleash a volley of homing chaos flames around the player every 2.5 seconds\n" +
                "You emit a blazing explosion when you are hit\n" +
                "Rogue stealth builds while not attacking and slower while moving, up to a max of 110\n" +
                "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            var modPlayer = player.Calamity();
            modPlayer.ataxiaBlaze = true;
            modPlayer.ataxiaVolley = true;
            modPlayer.rogueStealthMax += 1.1f;
            player.GetDamage<ThrowingDamageClass>() += 0.05f;
            modPlayer.wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().rogueAmmoCost *= 0.5f;
            player.GetDamage<ThrowingDamageClass>() += 0.12f;
            player.GetCritChance<ThrowingDamageClass>() += 10;
            player.moveSpeed += 0.05f;
            player.lavaMax += 240;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScoriaBar>(7).
                AddIngredient(ItemID.HellstoneBar, 4).
                AddIngredient<CoreofChaos>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
