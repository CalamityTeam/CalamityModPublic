using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class StarCannonEX : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Cannon EX");
            Tooltip.SetDefault("Fires a mix of normal, starfury, and astral stars");
        }

        public override void SetDefaults()
        {
            item.damage = 95;
            item.ranged = true;
            item.width = 74;
            item.height = 24;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Lime;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.UseSound = SoundID.Item9;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FallenStarProj>();
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.FallenStar;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int num6 = Main.rand.Next(1, 3);
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-15, 16) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-15, 16) * 0.05f;
                type = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.ProjectileType<AstralStar>(),
                    ProjectileID.Starfury,
                    ModContent.ProjectileType<FallenStarProj>()
                });
                int star = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
                if (star.WithinBounds(Main.maxProjectiles))
                    Main.projectile[star].Calamity().forceRanged = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.StarCannon);
            recipe.AddIngredient(ModContent.ItemType<AstralJelly>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 25);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
