using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AethersWhisper : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Shadowflame>()];
        }
        public override void SetDefaults()
        {
            Item.width = 134;
            Item.height = 44;
            Item.damage = 600;
            Item.knockBack = 5.5f;
            Item.useAnimation = Item.useTime = 24;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<AetherBeam>();
            Item.mana = 30;
            Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;

            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = CommonCalamitySounds.LaserCannonSound;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlasmaRod>().
                AddIngredient<TwistingNether>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
