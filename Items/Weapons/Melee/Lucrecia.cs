using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Melee.Shortswords;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Lucrecia : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = false;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.width = 58;
            Item.height = 58;
            Item.damage = 90;
            Item.knockBack = 8.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<LucreciaProj>();
            Item.shootSpeed = 2f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CoreofCalamity>().
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.SoulofNight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
