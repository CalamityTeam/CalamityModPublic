using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class StaffoftheMechworm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of the Mechworm");
            Tooltip.SetDefault("Summons an aerial mechworm to fight for you\n" +
                "Damage scales with the amount of minion slots you have\n" +
                "The damage scaling stops growing after 10 minion slots");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.mana = 15;
            item.width = 58;
            item.height = 58;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item113;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MechwormHead>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override bool CanUseItem(Player player)
        {
            float neededSlots = 1;
            float foundSlotsCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.minion && p.owner == player.whoAmI)
                {
                    foundSlotsCount += p.minionSlots;
                    if (foundSlotsCount + neededSlots > player.maxMinions)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int maxMinionScale = player.maxMinions;
            if (maxMinionScale > 10)
            {
                maxMinionScale = 10;
            }
            damage = (int)(damage * ((player.minionDamage * 5 / 3) + (player.minionDamage * 0.46f * (maxMinionScale - 1))));
            int owner = player.whoAmI;
            float num72 = item.shootSpeed;
            player.itemTime = item.useTime;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 value = Vector2.UnitX.RotatedBy((double)player.fullRotation, default);
            Vector2 vector3 = Main.MouseWorld - vector2;
            float velX = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float velY = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                velY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float dist = (float)Math.Sqrt((double)(velX * velX + velY * velY));
            if ((float.IsNaN(velX) && float.IsNaN(velY)) || (velX == 0f && velY == 0f))
            {
                velX = (float)player.direction;
                velY = 0f;
                dist = num72;
            }
            else
            {
                dist = num72 / dist;
            }
            velX *= dist;
            velY *= dist;
            int head = -1;
            int tail = -1;
            int typeHead = ModContent.ProjectileType<MechwormHead>();
            int typeTail = ModContent.ProjectileType<MechwormTail>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == owner)
                {
                    if (head == -1 && Main.projectile[i].type == typeHead)
                    {
                        head = i;
                    }
                    else if (tail == -1 && Main.projectile[i].type == typeTail)
                    {
                        tail = i;
                    }
                    if (head != -1 && tail != -1)
                    {
                        break;
                    }
                }
            }
            if (head == -1 && tail == -1)
            {
                float num77 = Vector2.Dot(value, vector3);
                if (num77 > 0f)
                {
                    player.ChangeDir(1);
                }
                else
                {
                    player.ChangeDir(-1);
                }
                velX = 0f;
                velY = 0f;
                vector2.X = (float)Main.mouseX + Main.screenPosition.X;
                vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
                int curr = Projectile.NewProjectile(vector2.X, vector2.Y, velX, velY, ModContent.ProjectileType<MechwormHead>(), damage, knockBack, owner);

                int prev = curr;
                curr = Projectile.NewProjectile(vector2.X, vector2.Y, velX, velY, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, owner, (float)prev);

                prev = curr;
                curr = Projectile.NewProjectile(vector2.X, vector2.Y, velX, velY, ModContent.ProjectileType<MechwormBody2>(), damage, knockBack, owner, (float)prev);
                Main.projectile[prev].localAI[1] = (float)curr;
                Main.projectile[prev].netUpdate = true;

                prev = curr;
                curr = Projectile.NewProjectile(vector2.X, vector2.Y, velX, velY, ModContent.ProjectileType<MechwormTail>(), damage, knockBack, owner, (float)prev);
                Main.projectile[prev].localAI[1] = (float)curr;
                Main.projectile[prev].netUpdate = true;
            }
            else if (head != -1 && tail != -1)
            {
                int body = Projectile.NewProjectile(vector2.X, vector2.Y, velX, velY, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, owner, Main.projectile[tail].ai[0]);
                int back = Projectile.NewProjectile(vector2.X, vector2.Y, velX, velY, ModContent.ProjectileType<MechwormBody2>(), damage, knockBack, owner, (float)body);

                Main.projectile[body].localAI[1] = (float)back;
                Main.projectile[body].ai[1] = 1f;
                Main.projectile[body].netUpdate = true;

                Main.projectile[back].localAI[1] = (float)tail;
                Main.projectile[back].netUpdate = true;
                Main.projectile[back].ai[1] = 1f;

                Main.projectile[tail].ai[0] = (float)back;
                Main.projectile[tail].netUpdate = true;
                Main.projectile[tail].ai[1] = 1f;
            }
            return false;
        }
    }
}
