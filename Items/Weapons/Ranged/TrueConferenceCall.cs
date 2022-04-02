using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TrueConferenceCall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Conference Call");
            Tooltip.SetDefault("@everyone\n" +
                "50% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 32;
            item.ranged = true;
            item.width = 66;
            item.height = 26;
            item.useTime = 32;
            item.useAnimation = 32;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.rare = ItemRarityID.Cyan;
            item.UseSound = SoundID.Item38;
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ProjectileID.PurificationPowder;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            int bulletAmt = 4;
            for (int index = 0; index < bulletAmt; ++index)
            {
                velocity.X += Main.rand.Next(-15, 16) * 0.05f;
                velocity.Y += Main.rand.Next(-15, 16) * 0.05f;
                int proj = Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI);
                Main.projectile[proj].extraUpdates += 2;
            }

            int maxTargets = 12;
            int[] targets = new int[maxTargets];
            int targetArrayIndex = 0;
            Rectangle rectangle = new Rectangle((int)player.Center.X - 960, (int)player.Center.Y - 540, 1920, 1080);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.chaseable && npc.lifeMax > 5 && !npc.dontTakeDamage && !npc.friendly && !npc.immortal)
                {
                    if (npc.Hitbox.Intersects(rectangle))
                    {
                        if (targetArrayIndex < maxTargets)
                        {
                            targets[targetArrayIndex] = i;
                            targetArrayIndex++;
                        }
                        else
                            break;
                    }
                }
            }

            if (targetArrayIndex == 0)
                return false;

            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            int extraBulletDamage = (int)(damage * 0.7);

            for (int j = 0; j < targetArrayIndex; j++)
            {
                vector2 = new Vector2(player.position.X + player.width * 0.5f + (Main.rand.Next(201) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                vector2.X = (vector2.X + player.Center.X) / 2f + Main.rand.Next(-200, 201);
                vector2.Y -= 100 * j;

                Vector2 velocity2 = Vector2.Normalize(Main.npc[targets[j]].Center - vector2) * item.shootSpeed;

                int proj = Projectile.NewProjectile(vector2, velocity2, type, extraBulletDamage, knockBack, player.whoAmI);
                Main.projectile[proj].extraUpdates += 2;
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].timeLeft /= 2;
            }

            if (targetArrayIndex == 12)
                return false;

            // Fire bullets at the same targets if 12 unique targets aren't found
            for (int k = 0; k < maxTargets - targetArrayIndex; k++)
            {
                int randomTarget = Main.rand.Next(targetArrayIndex);

                vector2 = new Vector2(player.position.X + player.width * 0.5f + (Main.rand.Next(201) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                vector2.X = (vector2.X + player.Center.X) / 2f + Main.rand.Next(-200, 201);
                vector2.Y -= 100 * randomTarget;

                Vector2 velocity2 = Vector2.Normalize(Main.npc[targets[randomTarget]].Center - vector2) * item.shootSpeed;

                int proj = Projectile.NewProjectile(vector2, velocity2, type, extraBulletDamage, knockBack, player.whoAmI);
                Main.projectile[proj].extraUpdates += 2;
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].timeLeft /= 2;
            }

            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TacticalShotgun);
            recipe.AddIngredient(ItemID.FragmentVortex, 7);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
