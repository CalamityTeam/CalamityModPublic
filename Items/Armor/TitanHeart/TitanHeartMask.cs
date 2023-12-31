﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.TitanHeart
{
    [AutoloadEquip(EquipType.Head)]
    public class TitanHeartMask : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 12; //43
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TitanHeartMantle>() && legs.type == ModContent.ItemType<TitanHeartBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalizedValue("SetBonus");
            var modPlayer = player.Calamity();
            modPlayer.titanHeartSet = true;
            player.GetDamage<ThrowingDamageClass>() += 0.15f;
            modPlayer.rogueStealthMax += 1f;
            modPlayer.wearingRogueArmor = true;
            player.noKnockback = true;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.titanHeartMask = true;
            player.GetDamage<ThrowingDamageClass>() += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralMonolith>(10).
                AddIngredient<Materials.TitanHeart>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
