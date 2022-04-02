using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class StaffoftheMechworm : ModItem
    {
        // This value is also referenced by the God Slayer and Auric summoner helmets.
        public const int BaseDamage = 161; // originally 325
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of the Mechworm");
            Tooltip.SetDefault("Summons an aerial mechworm to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.mana = 10;
            item.width = 68;
            item.height = 68;
            item.useTime = item.useAnimation = 10; // 9 because of useStyle 1
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.UseSound = SoundID.Item113;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MechwormHead>();
            item.shootSpeed = 10f;
            item.summon = true;
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

        public static void SummonBaseMechworm(int damage, float knockBack, Player owner, out int tailIndex)
        {
            tailIndex = -1;
            if (Main.myPlayer != owner.whoAmI)
                return;

            int curr = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormHead>(), damage, knockBack, owner.whoAmI, 0f, 0f);
            curr = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, owner.whoAmI, Main.projectile[curr].identity, 0f);
            int prev = curr;
            curr = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, owner.whoAmI, Main.projectile[curr].identity, 0f);
            Main.projectile[prev].localAI[1] = curr;
            prev = curr;
            curr = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormTail>(), damage, knockBack, owner.whoAmI, Main.projectile[curr].identity, 0f);
            Main.projectile[prev].localAI[1] = curr;

            tailIndex = curr;
        }

        public static void AddSegmentToMechworm(int tailIndex, int damage, float knockBack, Player owner)
        {
            if (Main.myPlayer != owner.whoAmI)
                return;

            Vector2 spawnPosition = Main.projectile[tailIndex].Center;
            Projectile tailAheadSegment = Main.projectile.Take(Main.maxProjectiles).FirstOrDefault(proj => MechwormBody.SameIdentity(proj, owner.whoAmI, (int)Main.projectile[tailIndex].ai[0]));
            int body = Projectile.NewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, owner.whoAmI, tailAheadSegment.identity, 0f);
            int body2 = body;
            body = Projectile.NewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, owner.whoAmI, Main.projectile[body].identity, 0f);

            var m = Main.projectileIdentity;
            Main.projectile[tailIndex].ai[0] = Main.projectile[body].identity;
            Main.projectile[tailIndex].netUpdate = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int head = -1;
            int tail = -1;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer)
                {
                    if (head == -1 && Main.projectile[i].type == ModContent.ProjectileType<MechwormHead>())
                    {
                        head = i;
                    }
                    if (tail == -1 && Main.projectile[i].type == ModContent.ProjectileType<MechwormTail>())
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
                SummonBaseMechworm(damage, knockBack, player, out _);
            else if (head != -1 && tail != -1)
                AddSegmentToMechworm(tail, damage, knockBack, player);
            return false;
        }
    }
}
