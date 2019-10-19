using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Tools
{
    public class CrystylCrusher : ModItem
    {
        private static int PickPower = 5000;
        private static float PowderSpeed = 9f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystyl Crusher");
            Tooltip.SetDefault("Gotta dig faster, gotta go deeper\n" +
                "Right click to destroy a lot of blocks");
        }

        public override void SetDefaults()
        {
            item.damage = 500;
            item.melee = true;
            item.crit += 25;
            item.width = 70;
            item.height = 70;
            item.useTime = 2;
            item.useAnimation = 10;
            item.useTurn = true;
            item.pick = PickPower;
            item.useStyle = 1;
            item.knockBack = 9f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.Calamity().postMoonLordRarity = 16;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 1f, 0f);
            Main.projectile[proj].extraUpdates = 1;
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.pick = 0;
                item.shoot = ModContent.ProjectileType<CrystalDust>();
                item.shootSpeed = PowderSpeed;
                item.tileBoost = 0;
            }
            else
            {
                item.pick = PickPower;
                item.shoot = 0;
                item.shootSpeed = 0f;
                item.tileBoost += 50;
            }
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GallantPickaxe>());
            recipe.AddIngredient(ModContent.ItemType<BlossomPickaxe>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int num307 = Main.rand.Next(3);
                if (num307 == 0)
                {
                    num307 = 173;
                }
                else if (num307 == 1)
                {
                    num307 = 57;
                }
                else
                {
                    num307 = 58;
                }
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, num307);
            }
        }
    }
}
