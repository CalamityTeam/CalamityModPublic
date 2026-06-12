using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class EidolicWail : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HadopelagicPressure>()];
        }
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 943;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 90;
            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.reuseDelay = 40;
            Item.useLimitPerAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.UseSound = CommonCalamitySounds.WyrmScreamSound;
            Item.autoReuse = true;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<EidolicWailSoundwave>();
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);
    }
}
