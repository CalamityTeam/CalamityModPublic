using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Shortswords;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Lucrecia : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 58;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 30;
            Item.shoot = ModContent.ProjectileType<LucreciaProj>();
            Item.shootSpeed = 4.2f;
            Item.knockBack = 8.25f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LifeAlloy>(5).
                AddIngredient<CoreofCalamity>().
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.SoulofNight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
