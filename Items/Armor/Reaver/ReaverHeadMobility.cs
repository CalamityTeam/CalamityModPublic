using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Reaver
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("ReaverVisage")]
    public class ReaverHeadMobility : ModItem
    {
        //Jump/Flight Boosts and Movement Speed Helm
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Reaver Visage");
            Tooltip.SetDefault("10% increased jump speed and 15% increased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 13; //46
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.reaverSpeed = true;
            modPlayer.wearingRogueArmor = true;
            player.setBonus = "Grants immunity to fall damage and allows constant jumping\n" +
                "10% increased flight time and horizontal wing speed\n" +
                "Hooks fly out and retract 10% faster\n" +
                "Reduces the cooldown of dashes";
            player.noFallDmg = true;
            player.autoJump = true;
            if (player.miscCounter % 3 == 2 && player.dashDelay > 0)
                player.dashDelay--;
        }

        public override void UpdateEquip(Player player)
        {
            player.jumpSpeedBoost += 0.5f;
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(7).
                AddIngredient<LivingShard>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
