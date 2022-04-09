using Terraria.DataStructures;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SeashineSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seashine Sword");
            Tooltip.SetDefault("Shoots an aqua sword beam");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.EnchantedSword);
            Item.useTime = 30;
            Item.damage = 26;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.width = 38;
            Item.height = 38;
            Item.knockBack = 2;
            Item.shootSpeed = 11;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<SeashineSwordProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaPrism>(7).
                AddIngredient<Navystone>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
