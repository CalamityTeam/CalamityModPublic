using System;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class ShieldoftheHighRuler : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int ShieldSlamIFrames = 12;
        public const float EoCDashVelocity = 14.5f;
        public const float TabiDashVelocity = 18.9f;

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 30;
            Item.knockBack = 9f;
            Item.width = 36;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
            Item.defense = 12;
            Item.accessory = true;
        }

        public override bool MeleePrefix() => false;
        public override bool WeaponPrefix() => false;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.dashType = 2;
            modPlayer.DashID = string.Empty;
            modPlayer.copyrightInfringementShield = true;
            player.noKnockback = true;
            player.fireWalk = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Weak] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Confused] = true;
            player.buffImmune[BuffID.Silenced] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[BuffID.WindPushed] = true;
            player.buffImmune[BuffID.Stoned] = true;
            player.statLifeMax2 += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EoCShield).
                AddIngredient(ItemID.CobaltShield).
                AddIngredient<LifeAlloy>(4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
