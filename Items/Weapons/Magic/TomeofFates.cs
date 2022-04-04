using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
            Item.damage = 110;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 5;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = Item.buyPrice(0, 95, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item103;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CosmicTentacle>();
            Item.shootSpeed = 17f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref int crit) => crit += 3;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.SpellTome).AddIngredient(ModContent.ItemType<MeldiateBar>(), 9).AddTile(TileID.Bookcases).Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int i = Main.myPlayer;
            int num73 = damage;
            float num74 = knockback;
            num74 = player.GetWeaponKnockback(Item, num74);
            player.itemTime = Item.useTime;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            Vector2 value2 = new Vector2(num78, num79);
            value2.Normalize();
            Vector2 value3 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
            value3.Normalize();
            value2 = value2 * 4f + value3;
            value2.Normalize();
            value2 *= Item.shootSpeed;
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
                Projectile.NewProjectile(source, vector2.X, vector2.Y, value2.X, value2.Y, ModContent.ProjectileType<BrimstoneTentacle>(), (int)((double)num73 * 1.5f), num74, i, num92, num91);
            }
            else
            {
                Projectile.NewProjectile(source, vector2.X, vector2.Y, value2.X, value2.Y, ModContent.ProjectileType<CosmicTentacle>(), num73, num74, i, num92, num91);
            }
            return false;
        }
    }
}
