using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class NeptunesBounty : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle SpinSound = new("CalamityMod/Sounds/Item/SpinningWoosh") { Volume = 0.65f };
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HadopelagicPressure>()];
        }
        public override void SetDefaults()
        {
            Item.width = 122;
            Item.height = 122;
            Item.damage = 444;
            Item.knockBack = 9f;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.shoot = ModContent.ProjectileType<NeptunesBountyProjectile>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = null;
            Item.autoReuse = true;

            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AbyssBlade>().
                AddIngredient<ReaperTooth>(6).
                AddIngredient<RuinousSoul>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
