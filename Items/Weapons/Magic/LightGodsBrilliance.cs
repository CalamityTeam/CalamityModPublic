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
            item.damage = 64;
            item.magic = true;
            item.mana = 4;
            item.width = 34;
            item.height = 36;
            item.useTime = item.useAnimation = 4;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;

            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().donorItem = true;

            item.UseSound = SoundID.Item9;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LightBead>();
            item.shootSpeed = 25f;
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadecrystalTome>());
            recipe.AddIngredient(ModContent.ItemType<AbyssalTome>());
            recipe.AddIngredient(ItemID.HolyWater, 10);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 2);
            recipe.AddIngredient(ItemID.SoulofLight, 30);
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 5);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
