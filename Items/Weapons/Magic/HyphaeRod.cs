using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 34;
            Item.height = 34;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.TruffleSpore;
            Item.shootSpeed = 1f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo projSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float speed = Item.shootSpeed;
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
                int proj = Projectile.NewProjectile(projSource, source, spawnVec, type, damage, knockback, player.whoAmI, 0f, Main.rand.Next(3));
                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].DamageType = DamageClass.Magic;
                    Main.projectile[proj].timeLeft = CalamityUtils.SecondsToFrames(3f);
                }
            }
            return false;
        }
    }
}
