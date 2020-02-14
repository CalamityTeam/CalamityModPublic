using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class TheDanceofLight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Dance of Light");
            Tooltip.SetDefault("'Show them the wrath of a Lightbearer'");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 42;
            item.damage = 500;
            item.knockBack = 4f;
            item.magic = true;
            item.mana = 4;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 4;
            item.useAnimation = 4;
            item.UseSound = SoundID.Item105;
            item.autoReuse = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<ElementBallShiv>();
            item.shootSpeed = 14f;

            item.value = Item.buyPrice(platinum: 5);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
        }

        public override Vector2? HoldoutOffset()
        {
            return Vector2.Zero;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float intendedSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            Vector2 target = Main.MouseWorld;
            float minOffsetX = -40f;
            float maxOffsetX = 40f;
            if (target.X > player.Center.X)
                minOffsetX = -120f;
            else
                maxOffsetX = 120f;

            int numProjectiles = 3;
            for (int i = 0; i < numProjectiles; ++i)
            {
                float speed = Main.rand.NextFloat(0.94f, 1.02f) * intendedSpeed;
                Vector2 deviation = new Vector2(Main.rand.NextFloat(minOffsetX, maxOffsetX), Main.rand.NextFloat(-1400f, -1200f));
                Vector2 startPos = target + deviation;
                Vector2 vel = Vector2.Normalize(-deviation) * speed;
                Projectile.NewProjectileDirect(startPos, vel, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarFlareBook);
            recipe.AddIngredient(ModContent.ItemType<WrathoftheAncients>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
