using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AuguroftheElements : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.damage = 147;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 11;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 2;
            Item.useAnimation = 10;
            Item.reuseDelay = 5;
            Item.useLimitPerAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item103;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ElementTentacle>();
            Item.shootSpeed = 30f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            Vector2 value2 = new Vector2(num78, num79);
            value2.Normalize();
            Vector2 value3 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
            value3.Normalize();
            value2 = value2 * 6f + value3;
            value2.Normalize();
            value2 *= Item.shootSpeed;
            float num91 = (float)Main.rand.Next(10, 50) * 0.001f;
            if (Main.rand.NextBool(2))
            {
                num91 *= -1f;
            }
            float num92 = (float)Main.rand.Next(10, 50) * 0.001f;
            if (Main.rand.NextBool(2))
            {
                num92 *= -1f;
            }
            Projectile.NewProjectile(source, vector2, value2, type, damage, knockback, player.whoAmI, num92, num91);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TomeofFates>().
                AddIngredient(ItemID.ShadowFlameHexDoll).
                AddIngredient<EldritchTome>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
