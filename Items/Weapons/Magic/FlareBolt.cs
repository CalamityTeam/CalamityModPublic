using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class FlareBolt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Bolt");
            Tooltip.SetDefault("Casts a slow-moving ball of flame");
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlareBoltProjectile>();
            Item.shootSpeed = 7.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 6).
                AddIngredient(ItemID.Obsidian, 9).
                AddIngredient(ItemID.Fireblossom, 2).
                AddIngredient(ItemID.LavaBucket).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
