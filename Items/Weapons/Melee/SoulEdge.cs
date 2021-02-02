using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SoulEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Edge");
            Tooltip.SetDefault("Fires the ghastly souls of long-deceased abyss dwellers");
        }

        public override void SetDefaults()
        {
            item.width = 88;
            item.damage = 219;
            item.melee = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.useTurn = true;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 88;
            item.shoot = ModContent.ProjectileType<GhastlySoulLarge>();
            item.shootSpeed = 12f;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numShots = 3;
            int projDamage = (int)(damage * 0.5f);
            for (int i = 0; i < numShots; ++i)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-40, 41) * 0.05f;
                float ai1 = Main.rand.NextFloat() + 0.5f;
                // TODO -- unchecked type addition math assumes we can guarantee load order
                // this is extremely unsafe and load order may become non deterministic in 1.4
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, Main.rand.Next(type, type + 3), projDamage, knockBack, player.whoAmI, 0.0f, ai1);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);
        }
    }
}
