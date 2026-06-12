using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class VenusianTrident : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Daybroken>()];
        }

        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 68;
            Item.damage = 1150;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 90;
            Item.useAnimation = Item.useTime = 65;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 9f;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.UseSound = SoundID.Item45;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VenusianBolt>();
            Item.shootSpeed = 13f;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.InfernoFork).
                AddIngredient<RuinousSoul>(2).
                AddIngredient<TwistingNether>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
