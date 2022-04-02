using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ParasiticSceptor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Parasitic Scepter");
            Tooltip.SetDefault("Fires a spread of water leeches that latch onto enemies, dealing a stacking damage over time");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 12;
            item.knockBack = 3f;
            item.mana = 10;
            item.useTime = item.useAnimation = 35;
            item.autoReuse = true;
            item.magic = true;
            item.shootSpeed = 10f;
            item.shoot = ModContent.ProjectileType<WaterLeechProj>();

            item.width = item.height = 52;
            item.UseSound = SoundID.Item46;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Green;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float speed = item.shootSpeed;
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
            int leechAmt = 2;
            if (Main.rand.NextBool(3))
            {
                leechAmt++;
            }
            if (Main.rand.NextBool(4))
            {
                leechAmt++;
            }
            if (Main.rand.NextBool(5))
            {
                leechAmt ++;
            }
            for (int i = 0; i < leechAmt; i++)
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
                Projectile.NewProjectile(playerPos, directionToShoot, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
