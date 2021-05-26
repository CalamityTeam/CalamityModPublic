using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EvilSmasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evil Smasher");
            Tooltip.SetDefault("EViL! sMaSH eVIl! SmAsh...ER!\n" +
				"For every enemy you kill this hammer gains stat bonuses\n" +
				"These bonuses stack until a cap is reached\n" +
				"The bonuses will reset if you get hit or select a different item\n" +
				"Summons fossil spikes on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 64;
			item.height = 66;
			item.scale = 2f;
			item.damage = 50;
            item.melee = true;
            item.useAnimation = item.useTime = 38;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
			item.Calamity().challengeDrop = true;
		}

		public override float UseTimeMultiplier	(Player player) => 1f + (player.Calamity().evilSmasherBoost * 0.1f);

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat) => mult += player.Calamity().evilSmasherBoost * 0.1f;

        public override void GetWeaponKnockback(Player player, ref float knockback) => knockback *= 1f + (player.Calamity().evilSmasherBoost * 0.1f);

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
			OnHitEffect(target.Center, player, knockback);

			if (target.life <= 0 && player.Calamity().evilSmasherBoost < 10)
				player.Calamity().evilSmasherBoost += 1;
		}

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
			OnHitEffect(target.Center, player, item.knockBack);
        }

		private void OnHitEffect(Vector2 targetPos, Player player, float knockback)
		{
            Projectile.NewProjectile(targetPos, Vector2.Zero, ModContent.ProjectileType<FossilSpike>(), (int)(item.damage * player.MeleeDamage()), knockback, Main.myPlayer);
		}
    }
}
