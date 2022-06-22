﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Hydrothermic
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("AtaxiaHelm")]

    public class HydrothermicHeadMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Hydrothermic Helm");
            Tooltip.SetDefault("12% increased melee damage and 10% increased melee critical strike chance\n" +
                "18% increased melee speed\n" +
                "Melee attacks and melee projectiles inflict on fire\n" +
                "Temporary immunity to lava and immunity to fire damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 33; //67
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
            player.setBonus = "5% increased melee damage\n" +
                "Enemies are more likely to target you\n" +
                "Inferno effect when below 50% life\n" +
                "Melee attacks and projectiles cause chaos flames to erupt on enemy hits\n" +
                "You emit a blazing explosion when you are hit";
            var modPlayer = player.Calamity();
            modPlayer.ataxiaBlaze = true;
            modPlayer.ataxiaGeyser = true;
            player.GetDamage<MeleeDamageClass>() += 0.05f;
            player.aggro += 700;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.ataxiaFire = true;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.18f;
            player.GetDamage<MeleeDamageClass>() += 0.12f;
            player.GetCritChance<MeleeDamageClass>() += 10;
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