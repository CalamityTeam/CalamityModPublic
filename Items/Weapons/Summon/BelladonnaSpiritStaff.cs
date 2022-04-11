using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class BelladonnaSpiritStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Belladonna Spirit Staff");
            Tooltip.SetDefault("Summons a cute forest spirit that scatters toxic petals");
        }

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.mana = 10;
            Item.width = 40;
            Item.height = 42;
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BelladonnaSpirit>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Vine, 4).AddIngredient(ItemID.JungleSpores, 5).AddIngredient(ItemID.Stinger, 8).AddIngredient(ItemID.RichMahogany, 25).AddTile(TileID.Anvils).Register();
        }
    }
}
