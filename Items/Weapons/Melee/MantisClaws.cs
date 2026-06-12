using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.BaseItems;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MantisClaws : CustomUseProjItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HeavyBleeding>()];
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
            Item.damage = 75;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 25;
            Item.knockBack = 0.25f;
            Item.shoot = ModContent.ProjectileType<MantisClawHoldout>();

            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool MeleePrefix() => true;
    }
}
