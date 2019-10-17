using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class FaceMelter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Face Melter");
            Tooltip.SetDefault("Fires music notes\n" +
                               "Right click summons an amplifier that shoots towards your mouse\n" +
                               "WOOO!! FAAAAAAANTASYY WORLDDDDD!");
        }

        public override void SetDefaults()
        {
            item.damage = 250;
            item.magic = true;
            item.mana = 10;
            item.width = 56;
            item.height = 50;
            item.useTime = 5;
            item.useAnimation = 10;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 21;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.shoot = ModContent.ProjectileType<MelterNote1>();
            item.UseSound = SoundID.Item47;
            item.autoReuse = true;
            item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TheAxe);
            recipe.AddIngredient(ItemID.MagicalHarp);
            recipe.AddIngredient(null, "SirensSong");
            recipe.AddIngredient(null, "CosmiliteBar", 10);
            recipe.AddIngredient(null, "NightmareFuel", 10);
            recipe.AddIngredient(null, "EndothermicEnergy", 10);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                item.useTime = 20;
                item.useAnimation = 20;
                item.reuseDelay = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<MelterAmp>(), damage, knockBack, player.whoAmI);
                return false;
            }
            else
            {
                item.useTime = 5;
                item.useAnimation = 10;
                float SpeedX = speedX;
                float SpeedY = speedY;
                int note = Main.rand.Next(0, 2);
                if (note == 0)
                {
                    damage = (int)(damage * 1.5f);
                    type = ModContent.ProjectileType<MelterNote1>();
                }
                else
                {
                    SpeedX *= 1.5f;
                    SpeedY *= 1.5f;
                    type = ModContent.ProjectileType<MelterNote2>();
                }
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                return false;
            }
        }
    }
}
