using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class OnyxChainBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Chain Blaster");
            Tooltip.SetDefault("50% chance to not consume ammo\n" +
                "Fires a spread of bullets and an onyx shard");
        }

        public override void SetDefaults()
        {
            item.damage = 58;
            item.ranged = true;
            item.width = 64;
            item.height = 32;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item36;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 24f;
            item.useAmmo = 97;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-25, 26) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-25, 26) * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX * 0.9f, SpeedY * 0.9f, 661, damage, knockBack, player.whoAmI, 0f, 0f);
            for (int i = 0; i <= 3; i++)
            {
                float SpeedNewX = speedX + (float)Main.rand.Next(-45, 46) * 0.05f;
                float SpeedNewY = speedY + (float)Main.rand.Next(-45, 46) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedNewX, SpeedNewY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.OnyxBlaster);
            recipe.AddIngredient(ItemID.ChainGun);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
