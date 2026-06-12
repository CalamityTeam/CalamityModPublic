using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GildedProboscis : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override int ProjectileType => ModContent.ProjectileType<GildedProboscisProj>();

        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<VermillionFlux>()];
            base.SetStaticDefaults();
            CalamityItemSets.ShowScalingCritDamageTooltip[Type] = true;
        }
        public override bool SizeModifiers => false;
        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 66;
            Item.damage = 2090;
            Item.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Item.useAnimation = Item.useTime = 65;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 15f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.shootSpeed = 13f;
            Item.channel = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            base.SetDefaults();
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
