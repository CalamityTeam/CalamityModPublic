using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("DepthBlade")]
    public class DepthCrusher : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<RiptideDebuff>()];
        }
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 50;
            Item.damage = 48;
            Item.knockBack = 7.25f;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.shoot = ModContent.ProjectileType<DepthCrusherProjectile>();
            Item.shootSpeed = 6f;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = null;
            Item.autoReuse = true;

            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
        }
    }
}
