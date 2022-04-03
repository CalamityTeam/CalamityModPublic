using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class HarvestStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvest Staff");
            Tooltip.SetDefault("Casts flaming pumpkins");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 46;
            Item.height = 44;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlamingPumpkin>();
            Item.shootSpeed = 10f;
            Item.scale = 0.9f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Pumpkin, 20).AddIngredient(ItemID.FallenStar, 5).AddTile(TileID.Anvils).Register();
        }
    }
}
