using CalamityMod.CalPlayer;
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
        public static int BaseDamage = 360;

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
            item.useAnimation = 20;
            item.useTime = 20;
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
            CalamityPlayerOnHit.HorsemansBladeOnHit(player, target.whoAmI, (int)(item.damage * player.MeleeDamage() * 1.5f), knockback, 0, ModContent.ProjectileType<MourningSkull>());
            CalamityPlayerOnHit.HorsemansBladeOnHit(player, target.whoAmI, (int)(item.damage * player.MeleeDamage() * 1.5f), knockback, 1);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            CalamityPlayerOnHit.HorsemansBladeOnHit(player, -1, (int)(item.damage * player.MeleeDamage() * 1.5f), item.knockBack, 0, ModContent.ProjectileType<MourningSkull>());
            CalamityPlayerOnHit.HorsemansBladeOnHit(player, -1, (int)(item.damage * player.MeleeDamage() * 1.5f), item.knockBack, 1);
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
