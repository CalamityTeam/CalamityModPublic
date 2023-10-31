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
            Item.damage = 131;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
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
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            Vector2 tentacleVelocity = new Vector2(mouseXDist, mouseYDist);
            tentacleVelocity.Normalize();
            Vector2 tentacleRandVelocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
            tentacleRandVelocity.Normalize();
            tentacleVelocity = tentacleVelocity * 6f + tentacleRandVelocity;
            tentacleVelocity.Normalize();
            tentacleVelocity *= Item.shootSpeed;
            float tentacleYDirection = (float)Main.rand.Next(10, 50) * 0.001f;
            if (Main.rand.NextBool())
            {
                tentacleYDirection *= -1f;
            }
            float tentacleXDirection = (float)Main.rand.Next(10, 50) * 0.001f;
            if (Main.rand.NextBool())
            {
                tentacleXDirection *= -1f;
            }
            Projectile.NewProjectile(source, realPlayerPos, tentacleVelocity, type, damage, knockback, player.whoAmI, tentacleXDirection, tentacleYDirection);
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
