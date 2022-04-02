using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class TomeofFates : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tome of Fates");
            Tooltip.SetDefault("Casts cosmic tentacles to spear your enemies\n" +
                "Can randomly fire a brimstone tentacle for immense damage");
        }

        public override void SetDefaults()
        {
            item.damage = 110;
            item.magic = true;
            item.mana = 8;
            item.width = 28;
            item.height = 30;
            item.useTime = 5;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = ItemRarityID.Cyan;
            item.UseSound = SoundID.Item103;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CosmicTentacle>();
            item.shootSpeed = 17f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 3;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpellTome);
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 9);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int i = Main.myPlayer;
            int num73 = damage;
            float num74 = knockBack;
            num74 = player.GetWeaponKnockback(item, num74);
            player.itemTime = item.useTime;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            Vector2 value2 = new Vector2(num78, num79);
            value2.Normalize();
            Vector2 value3 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
            value3.Normalize();
            value2 = value2 * 4f + value3;
            value2.Normalize();
            value2 *= item.shootSpeed;
            int projChoice = Main.rand.Next(7);
            float num91 = (float)Main.rand.Next(10, 160) * 0.001f;
            if (Main.rand.NextBool(2))
            {
                num91 *= -1f;
            }
            float num92 = (float)Main.rand.Next(10, 160) * 0.001f;
            if (Main.rand.NextBool(2))
            {
                num92 *= -1f;
            }
            if (projChoice == 0)
            {
                Projectile.NewProjectile(vector2.X, vector2.Y, value2.X, value2.Y, ModContent.ProjectileType<BrimstoneTentacle>(), (int)((double)num73 * 1.5f), num74, i, num92, num91);
            }
            else
            {
                Projectile.NewProjectile(vector2.X, vector2.Y, value2.X, value2.Y, ModContent.ProjectileType<CosmicTentacle>(), num73, num74, i, num92, num91);
            }
            return false;
        }
    }
}
