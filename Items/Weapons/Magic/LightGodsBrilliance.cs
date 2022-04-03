using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class LightGodsBrilliance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light God's Brilliance");
            Tooltip.SetDefault("Casts small, homing light beads along with explosive light balls");
        }

        public override void SetDefaults()
        {
            Item.damage = 64;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.width = 34;
            Item.height = 36;
            Item.useTime = Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(1, 80, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().donorItem = true;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LightBead>();
            Item.shootSpeed = 25f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 beadVelocity = new Vector2(speedX, speedY) + Main.rand.NextVector2Square(-2.5f, 2.5f);
            Projectile.NewProjectile(position, beadVelocity, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);

            if (Main.rand.NextBool(3))
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<LightBall>(), damage * 2, knockBack, player.whoAmI, 0f, 0f);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ShadecrystalTome>()).AddIngredient(ModContent.ItemType<AbyssalTome>()).AddIngredient(ItemID.HolyWater, 10).AddIngredient(ItemID.SoulofLight, 30).AddIngredient(ModContent.ItemType<EffulgentFeather>(), 5).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8).AddIngredient(ModContent.ItemType<NightmareFuel>(), 20).AddTile(TileID.Bookcases).Register();
        }
    }
}
