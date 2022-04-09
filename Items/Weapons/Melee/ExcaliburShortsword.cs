using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ExcaliburShortsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Excalibur Shortsword");
            Tooltip.SetDefault("Don't underestimate the power of shortswords");
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.width = 40;
            Item.height = 40;
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5.25f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 57);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
