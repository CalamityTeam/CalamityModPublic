using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TyphonsGreed : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Typhon's Greed");
            Tooltip.SetDefault("Summons water spirits while in use");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Melee;
            Item.width = 16;
            Item.height = 16;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<TyphonsGreedStaff>();
            Item.shootSpeed = 24f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DepthCells>(30).
                AddIngredient<Lumenyl>(10).
                AddIngredient<Tenebris>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
