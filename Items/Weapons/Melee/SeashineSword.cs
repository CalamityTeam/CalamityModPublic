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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = ModContent.ProjectileType<SeashineSwordProj>();
            return base.Shoot(player, ref position, ref velocity.X, ref velocity.Y, ref type, ref damage, ref knockBack);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SeaPrism>(), 7).AddIngredient(ModContent.ItemType<Navystone>(), 10).AddTile(TileID.Anvils).Register();
        }
    }
}
