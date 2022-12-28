using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class BrimroseStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimrose Staff");
            Tooltip.SetDefault("Fires a spread of brimstone beams");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.width = 36;
            Item.height = 34;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrimstoneBeam>();
            Item.shootSpeed = 6f;
        }
               

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UnholyCore>(6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
