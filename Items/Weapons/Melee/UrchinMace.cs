using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class UrchinMace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Mace");
            Tooltip.SetDefault("Throws short-range whirlpools");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.damage = 35;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 19;
            Item.knockBack = 4;
            Item.UseSound = SoundID.Item1;
            Item.height = 48;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<UrchinMaceProjectile>();
            Item.shootSpeed = 9f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
