using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.mana = 10;
            Item.width = 68;
            Item.height = 68;
            Item.useTime = Item.useAnimation = 10; // 9 because of useStyle 1
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.UseSound = SoundID.Item113;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MechwormHead>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
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

        public static void SummonBaseMechworm(IEntitySource source, int damage, float knockback, Player owner, out int tailIndex)
        {
            tailIndex = -1;
            if (Main.myPlayer != owner.whoAmI)
                return;

            int curr = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormHead>(), damage, knockback, owner.whoAmI, 0f, 0f);
            curr = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockback, owner.whoAmI, Main.projectile[curr].identity, 0f);
            if (Main.projectile.IndexInRange(curr))
                Main.projectile[curr].originalDamage = damage;
            int prev = curr;
            curr = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockback, owner.whoAmI, Main.projectile[curr].identity, 0f);
            if (Main.projectile.IndexInRange(curr))
                Main.projectile[curr].originalDamage = damage;
            Main.projectile[prev].localAI[1] = curr;
            prev = curr;
            curr = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormTail>(), damage, knockback, owner.whoAmI, Main.projectile[curr].identity, 0f);
            if (Main.projectile.IndexInRange(curr))
                Main.projectile[curr].originalDamage = damage;
            Main.projectile[prev].localAI[1] = curr;

            tailIndex = curr;
        }

        public static void AddSegmentToMechworm(IEntitySource source, int tailIndex, int damage, float knockback, Player owner)
        {
            if (Main.myPlayer != owner.whoAmI)
                return;

            Vector2 spawnPosition = Main.projectile[tailIndex].Center;
            Projectile tailAheadSegment = Main.projectile.Take(Main.maxProjectiles).FirstOrDefault(proj => MechwormBody.SameIdentity(proj, owner.whoAmI, (int)Main.projectile[tailIndex].ai[0]));
            int body = Projectile.NewProjectile(source, spawnPosition, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockback, owner.whoAmI, tailAheadSegment.identity, 0f);
            int body2 = body;
            body = Projectile.NewProjectile(source, spawnPosition, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockback, owner.whoAmI, Main.projectile[body].identity, 0f);

            var m = Main.projectileIdentity;
            Main.projectile[tailIndex].ai[0] = Main.projectile[body].identity;
            Main.projectile[tailIndex].netUpdate = true;
            if (Main.projectile.IndexInRange(body))
                Main.projectile[body].originalDamage = damage;
            if (Main.projectile.IndexInRange(body2))
                Main.projectile[body2].originalDamage = damage;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
                SummonBaseMechworm(source, damage, knockback, player, out _);
            else if (head != -1 && tail != -1)
                AddSegmentToMechworm(source, tail, damage, knockback, player);
            return false;
        }
    }
}
