using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Mourningstar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mourningstar");
            Tooltip.SetDefault("Launches two solar whip swords that explode on hit");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.damage = 127;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.5f;
            Item.UseSound = SoundID.Item116;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.shootSpeed = 24f;
            Item.shoot = ModContent.ProjectileType<MourningstarFlail>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.SolarEruption).AddIngredient(ModContent.ItemType<CoreofChaos>(), 6).AddIngredient(ModContent.ItemType<CoreofCinder>(), 6).AddIngredient(ModContent.ItemType<DivineGeode>(), 6).AddTile(TileID.LunarCraftingStation).Register();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float ai3 = (Main.rand.NextFloat() - 0.75f) * 0.7853982f; //0.5
            float ai3X = (Main.rand.NextFloat() - 0.25f) * 0.7853982f; //0.5
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, ai3);
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, ai3X);
            return false;
        }
    }
}
