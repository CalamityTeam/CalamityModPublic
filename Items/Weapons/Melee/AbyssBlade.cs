using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AbyssBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle SpinSound = new("CalamityMod/Sounds/Item/SpinningWoosh") { Volume = 0.65f };
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<CrushDepth>()];
        }
        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 74;
            Item.damage = 123;
            Item.knockBack = 7.5f;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.shoot = ModContent.ProjectileType<AbyssBladeProjectile>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = null;
            Item.autoReuse = true;

            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DepthCrusher>().
                AddIngredient<Voidstone>(20).
                AddIngredient<DepthCells>(20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
