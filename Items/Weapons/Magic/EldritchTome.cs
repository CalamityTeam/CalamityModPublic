using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class EldritchTome : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 7;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item103;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EldritchTentacle>();
            Item.shootSpeed = 12f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 5;

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
            float tentacleYDirection = (float)Main.rand.Next(10, 80) * 0.001f;
            if (Main.rand.NextBool())
            {
                tentacleYDirection *= -1f;
            }
            float tentacleXDirection = (float)Main.rand.Next(10, 80) * 0.001f;
            if (Main.rand.NextBool())
            {
                tentacleXDirection *= -1f;
            }
            Projectile.NewProjectile(source, realPlayerPos, tentacleVelocity, ModContent.ProjectileType<EldritchTentacle>(), damage, playerKnockback, i, tentacleXDirection, tentacleYDirection);
            return false;
        }
    }
}
