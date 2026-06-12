using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.BaseItems;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class StellarStriker : CustomUseProjItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Nightwither>()];
        }
        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 118;
            Item.damage = 123;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = Item.useTime = 28;

            Item.useTurn = true;
            Item.knockBack = 7.75f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;

            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<StellarStrikerHoldout>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CometQuasher>().
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
