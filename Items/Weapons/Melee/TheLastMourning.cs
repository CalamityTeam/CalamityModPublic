using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheLastMourning : ModItem
    {
        public static int BaseDamage = 480;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Last Mourning");
            Tooltip.SetDefault("Summons flaming pumpkins and mourning skulls that split into fire orbs on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 94;
            item.height = 94;
			item.scale = 1.5f;
            item.melee = true;
            item.damage = BaseDamage;
            item.knockBack = 8.5f;
            item.useAnimation = 24;
            item.useTime = 24;
            item.autoReuse = true;
            item.useTurn = true;

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;

            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
            item.value = Item.buyPrice(1, 40, 0, 0);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            int logicCheckScreenHeight = Main.LogicCheckScreenHeight;
            int logicCheckScreenWidth = Main.LogicCheckScreenWidth;
            int num = Main.rand.Next(100, 300);
            int num2 = Main.rand.Next(100, 300);
            switch (Main.rand.Next(4))
            {
                case 0:
                    num -= logicCheckScreenWidth / 2 + num;
                    break;
                case 1:
                    num += logicCheckScreenWidth / 2 - num;
                    break;
                case 2:
                    num2 -= logicCheckScreenHeight / 2 + num2;
                    break;
                case 3:
                    num2 += logicCheckScreenHeight / 2 - num2;
                    break;
                default:
                    break;
            }
            num += (int)player.position.X;
            num2 += (int)player.position.Y;
            float speed = 8f;
            Vector2 vector = new Vector2((float)num, (float)num2);
            float num3 = target.position.X - vector.X;
            float num4 = target.position.Y - vector.Y;
            float num5 = (float)Math.Sqrt((double)(num3 * num3 + num4 * num4));
            num5 = speed / num5;
            num3 *= num5;
            num4 *= num5;
            Projectile.NewProjectile((float)num, (float)num2, num3, num4, ModContent.ProjectileType<MourningSkull>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 1.5f), knockback, player.whoAmI, (float)target.whoAmI, 0f);
            CalamityGlobalItem.HorsemansBladeOnHit(player, target.whoAmI, (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 1.5f), knockback, true);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            int logicCheckScreenHeight = Main.LogicCheckScreenHeight;
            int logicCheckScreenWidth = Main.LogicCheckScreenWidth;
            int num = Main.rand.Next(100, 300);
            int num2 = Main.rand.Next(100, 300);
            switch (Main.rand.Next(4))
            {
                case 0:
                    num -= logicCheckScreenWidth / 2 + num;
                    break;
                case 1:
                    num += logicCheckScreenWidth / 2 - num;
                    break;
                case 2:
                    num2 -= logicCheckScreenHeight / 2 + num2;
                    break;
                case 3:
                    num2 += logicCheckScreenHeight / 2 - num2;
                    break;
                default:
                    break;
            }
            num += (int)player.position.X;
            num2 += (int)player.position.Y;
            float speed = 8f;
            Vector2 vector = new Vector2((float)num, (float)num2);
            float num3 = target.position.X - vector.X;
            float num4 = target.position.Y - vector.Y;
            float num5 = (float)Math.Sqrt((double)(num3 * num3 + num4 * num4));
            num5 = speed / num5;
            num3 *= num5;
            num4 *= num5;
            Projectile.NewProjectile((float)num, (float)num2, num3, num4, ModContent.ProjectileType<MourningSkull>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 1.5f), item.knockBack, player.whoAmI, (float)target.whoAmI, 0f);
            CalamityGlobalItem.HorsemansBladeOnHit(player, target.whoAmI, (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 1.5f), item.knockBack, true);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dustType = 5;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        dustType = 5;
                        break;
                    case 1:
                        dustType = 6;
                        break;
                    case 2:
                        dustType = 174;
                        break;
                    default:
                        break;
                }
                int dust = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, dustType, (float)(player.direction * 2), 0f, 150, default, 1.3f);
                Main.dust[dust].velocity *= 0.2f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<BalefulHarvester>());
            r.AddIngredient(ItemID.SoulofNight, 30);
            r.AddIngredient(ModContent.ItemType<ReaperTooth>(), 5);
            r.AddIngredient(ModContent.ItemType<RuinousSoul>(), 3);
            r.AddTile(TileID.LunarCraftingStation);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
