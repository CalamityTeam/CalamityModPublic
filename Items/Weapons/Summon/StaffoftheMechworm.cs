using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
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
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item113;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MechwormHead>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
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
            damage = (int)(damage * ((player.MinionDamage() * 5 / 3) + (player.MinionDamage() * 0.46f * (maxMinionScale - 1))));
            int head = -1;
            int tail = -1;
            for (int num187 = 0; num187 < Main.projectile.Length; num187++)
            {
                if (Main.projectile[num187].active && Main.projectile[num187].owner == Main.myPlayer)
                {
                    if (head == -1 && Main.projectile[num187].type == ModContent.ProjectileType<MechwormHead>())
                    {
                        head = num187;
                    }
                    if (tail == -1 && Main.projectile[num187].type == ModContent.ProjectileType<MechwormTail>())
                    {
                        tail = num187;
                    }
                    if (head != -1 && tail != -1)
                    {
                        break;
                    }
                }
            }
            if (head == -1 && tail == -1)
            {
                speedX = 0f;
                speedY = 0f;
                position.X = (float)Main.mouseX + Main.screenPosition.X;
                position.Y = (float)Main.mouseY + Main.screenPosition.Y;
                int curr = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                curr = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, player.whoAmI, curr, 0f);
                int head2 = curr;
                curr = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<MechwormBody2>(), damage, knockBack, player.whoAmI, curr, 0f);
                Main.projectile[head2].localAI[1] = curr;
                head2 = curr;
                curr = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<MechwormTail>(), damage, knockBack, player.whoAmI, curr, 0f);
                Main.projectile[head2].localAI[1] = curr;
            }
            else if (head != -1 && tail != -1)
            {
                int body = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, player.whoAmI, Projectile.GetByUUID(Main.myPlayer, Main.projectile[tail].ai[0]), 0f);
                int body2 = body;
                body = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<MechwormBody2>(), damage, knockBack, player.whoAmI, body, 0f);
                Main.projectile[body2].localAI[1] = body;
                Main.projectile[body2].netUpdate = true;
                Main.projectile[body2].ai[1] = 1f;
                Main.projectile[body].localAI[1] = tail;
                Main.projectile[body].netUpdate = true;
                Main.projectile[body].ai[1] = 1f;
                Main.projectile[tail].ai[0] = Main.projectile[body].projUUID;
                Main.projectile[tail].netUpdate = true;
                Main.projectile[tail].ai[1] = 1f;
            }
            return false;
        }
    }
}
