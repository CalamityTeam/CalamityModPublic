using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MonstrousKnives : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.damage = 9;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 21;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.height = 20;

            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<MonstrousKnife>();
            Item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float speed = Item.shootSpeed;
            Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float xDist = Main.mouseX + Main.screenPosition.X - playerPos.X;
            float yDist = Main.mouseY + Main.screenPosition.Y - playerPos.Y;
            if (player.gravDir == -1f)
            {
                yDist = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - playerPos.Y;
            }
            Vector2 vector = new Vector2(xDist, yDist);
            float speedMult = vector.Length();
            if ((float.IsNaN(xDist) && float.IsNaN(yDist)) || (xDist == 0f && yDist == 0f))
            {
                xDist = player.direction;
                yDist = 0f;
                speedMult = speed;
            }
            else
            {
                speedMult = speed / speedMult;
            }
            xDist *= speedMult;
            yDist *= speedMult;
            int knifeAmt = 3;
            if (Main.rand.NextBool())
            {
                knifeAmt++;
            }
            if (Main.rand.NextBool(4))
            {
                knifeAmt++;
            }
            if (Main.rand.NextBool(8))
            {
                knifeAmt++;
            }
            if (Main.rand.NextBool(16))
            {
                knifeAmt++;
            }
            for (int i = 0; i < knifeAmt; i++)
            {
                float xVec = xDist;
                float yVec = yDist;
                float spreadMult = 0.05f * i;
                xVec += Main.rand.NextFloat(-25f, 25f) * spreadMult;
                yVec += Main.rand.NextFloat(-25f, 25f) * spreadMult;
                Vector2 directionToShoot = new Vector2(xVec, yVec);
                speedMult = directionToShoot.Length();
                speedMult = speed / speedMult;
                xVec *= speedMult;
                yVec *= speedMult;
                directionToShoot = new Vector2(xVec, yVec);
                Projectile.NewProjectile(source, playerPos, directionToShoot, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ThrowingKnife, 200).
                AddIngredient(ItemID.LifeCrystal).
                AddIngredient(ItemID.LesserHealingPotion, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
