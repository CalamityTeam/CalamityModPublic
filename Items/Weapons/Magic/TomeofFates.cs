using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class TomeofFates : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
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
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item103;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CosmicTentacle>();
            Item.shootSpeed = 17f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 3;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int i = Main.myPlayer;
            float playerKnockback = knockback;
            playerKnockback = player.GetWeaponKnockback(Item, playerKnockback);
            player.itemTime = Item.useTime;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            Vector2 tentacleVelocity = new Vector2(mouseXDist, mouseYDist);
            tentacleVelocity.Normalize();
            Vector2 tentacleRandVelocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
            tentacleRandVelocity.Normalize();
            tentacleVelocity = tentacleVelocity * 4f + tentacleRandVelocity;
            tentacleVelocity.Normalize();
            tentacleVelocity *= Item.shootSpeed;
            int projChoice = Main.rand.Next(7);
            float tentacleYDirection = (float)Main.rand.Next(10, 160) * 0.001f;
            if (Main.rand.NextBool())
            {
                tentacleYDirection *= -1f;
            }
            float tentacleXDirection = (float)Main.rand.Next(10, 160) * 0.001f;
            if (Main.rand.NextBool())
            {
                tentacleXDirection *= -1f;
            }
            if (projChoice == 0)
            {
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, tentacleVelocity.X, tentacleVelocity.Y, ModContent.ProjectileType<BrimstoneTentacle>(), (int)((double)damage * 1.5f), playerKnockback, i, tentacleXDirection, tentacleYDirection);
            }
            else
            {
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, tentacleVelocity.X, tentacleVelocity.Y, ModContent.ProjectileType<CosmicTentacle>(), damage, playerKnockback, i, tentacleXDirection, tentacleYDirection);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpellTome).
                AddIngredient<MeldConstruct>(9).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
