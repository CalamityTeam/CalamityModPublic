using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PrimordialEarth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Primordial Earth");
            Tooltip.SetDefault("Casts a large blast of dust");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 19;
            Item.width = 36;
            Item.height = 42;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SupremeDustProjectile>();
            Item.shootSpeed = 4f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DeathValley>().
                AddIngredient(ItemID.AncientBattleArmorMaterial, 3).
                AddIngredient(ItemID.MeteoriteBar, 5).
                AddIngredient(ItemID.Ectoplasm, 5).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
