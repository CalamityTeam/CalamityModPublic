using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class VirulentKatana : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Virulence");
            Tooltip.SetDefault("Fires a plague cloud");
        }

        public override void SetDefaults()
        {
            item.damage = 96;
            item.knockBack = 5.5f;
            item.useAnimation = item.useTime = 15;
            item.melee = true;
            item.useTurn = true;
            item.autoReuse = true;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<PlagueDust>();

            item.width = 48;
            item.height = 62;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * 0.85), knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 300);
        }
    }
}
