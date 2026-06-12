using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Cryophobia : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<WindChilled>(), BuffID.Frozen];
        }
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 34;
            Item.damage = 96;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 22;
            Item.useAnimation = Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityLightPurpleBuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item117;
            Item.autoReuse = true;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<CryoBlast>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
    }
}
