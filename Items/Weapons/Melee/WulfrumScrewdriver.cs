using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class WulfrumScrewdriver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Screwdriver");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 50;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useTurn = true;
            Item.knockBack = 3.75f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<WulfrumScrewdriverProj>();
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            hitbox = CalamityUtils.FixSwingHitbox(39, 39);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(12).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
