using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class HellfireFlamberge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellfire Flamberge");
            Tooltip.SetDefault("Fires a spread of volcanic fireballs");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.damage = 90;
            item.melee = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.useTurn = true;
            item.knockBack = 7.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 60;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<ChaosFlameSmall>();
            item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Main.PlaySound(SoundID.Item20, player.position);
            for (int index = 0; index < 3; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-40, 41) * 0.05f;
                float damageMult = 0.5f;
                switch (index)
                {
                    case 0:
                    case 1:
                        type = ModContent.ProjectileType<ChaosFlameSmall>();
                        break;
                    case 2:
                        type = ModContent.ProjectileType<ChaosFlameLarge>();
                        damageMult = 0.75f;
                        break;
                    default:
                        break;
                }
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * damageMult), knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool(3) ? 16 : 174);
            }
            if (Main.rand.NextBool(5))
            {
                int smoke = Gore.NewGore(new Vector2(hitbox.X, hitbox.Y), default, Main.rand.Next(375, 378), 0.75f);
                Main.gore[smoke].behindTiles = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}
