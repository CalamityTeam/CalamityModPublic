using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class HyphaeRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hyphae Rod");
            Tooltip.SetDefault("Creates mushroom spores near the player");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.magic = true;
            item.mana = 7;
            item.width = 34;
            item.height = 34;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item8;
            item.autoReuse = true;
            item.shoot = ProjectileID.TruffleSpore;
            item.shootSpeed = 1f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float speed = item.shootSpeed;
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float xDist = (float)Main.mouseX + Main.screenPosition.X + source.X;
            float yDist = (float)Main.mouseY + Main.screenPosition.Y + source.Y;
            Vector2 spawnVec = new Vector2(xDist, yDist);
            if (player.gravDir == -1f)
            {
                spawnVec.Y = Main.screenPosition.Y + (float)Main.screenHeight + (float)Main.mouseY + source.Y;
            }
            float distance = spawnVec.Length();
            if ((float.IsNaN(spawnVec.X) && float.IsNaN(spawnVec.Y)) || (spawnVec.X == 0f && spawnVec.Y == 0f))
            {
                spawnVec.X = (float)player.direction;
                spawnVec.Y = 0f;
                distance = speed;
            }
            else
            {
                distance = speed / distance;
            }

            int projAmt = 3;
            for (int projIndex = 0; projIndex < projAmt; projIndex++)
            {
                source = new Vector2(player.Center.X + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y);
                source.X = (source.X + player.Center.X) / 2f + (float)Main.rand.Next(-100, 101);
                source.Y -= (float)(50 * projIndex);
                spawnVec.X = (float)Main.mouseX + Main.screenPosition.X - source.X;
                spawnVec.Y = (float)Main.mouseY + Main.screenPosition.Y - source.Y;
                if (spawnVec.Y < 0f)
                {
                    spawnVec.Y *= -1f;
                }
                if (spawnVec.Y < 20f)
                {
                    spawnVec.Y = 20f;
                }
                distance = spawnVec.Length();
                distance = speed / distance;
                spawnVec.X *= distance;
                spawnVec.Y *= distance;
                spawnVec.X += (float)Main.rand.Next(-180, 181) * 0.02f;
                spawnVec.Y += (float)Main.rand.Next(-180, 181) * 0.02f;
                int proj = Projectile.NewProjectile(source, spawnVec, type, damage, knockBack, player.whoAmI, 0f, Main.rand.Next(3));
                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].Calamity().forceMagic = true;
                    Main.projectile[proj].timeLeft = CalamityUtils.SecondsToFrames(3f);
                }
            }
            return false;
        }
    }
}
