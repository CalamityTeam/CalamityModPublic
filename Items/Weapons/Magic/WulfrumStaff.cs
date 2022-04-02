using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class WulfrumStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Staff");
            Tooltip.SetDefault("Casts a wulfrum bolt");
            //Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 13;
            item.magic = true;
            item.mana = 2;
            item.width = 44;
            item.height = 46;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item43;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<WulfrumBolt>();
            item.shootSpeed = 9f;
        }

        //public override Vector2? HoldoutOrigin() => new Vector2(8, 15);

        public override Vector2? HoldoutOffset() => new Vector2(-3, 0);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WulfrumShard>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
