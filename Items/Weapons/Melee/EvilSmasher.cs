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
				"Rare Item Variant");
        }

        public override void SetDefaults()
        {
            item.width = 62;
			item.height = 62;
			item.scale = 2f;
			item.damage = 55;
            item.melee = true;
            item.useAnimation = item.useTime = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

		public override float UseTimeMultiplier	(Player player)
		{
			if (player.Calamity().brimlashBusterBoost)
				return 2f;
			return 1f;
		}

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
		{
			float damageMult = 0f;
            if (player.Calamity().brimlashBusterBoost)
				damageMult = 0.5f;
			mult += damageMult;
		}

        public override void GetWeaponKnockback(Player player, ref float knockback)
        {
            if (player.Calamity().brimlashBusterBoost)
            {
                knockback *= 1.75f;
            }
		}

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
			OnHitEffect(target.Center, player, knockback);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
			OnHitEffect(target.Center, player, item.knockBack);
        }

		private void OnHitEffect(Vector2 targetPos, Player player, float knockback)
		{
            Projectile.NewProjectile(targetPos, Vector2.Zero, ModContent.ProjectileType<FossilSpike>(), (int)(item.damage * player.MeleeDamage()), knockback, Main.myPlayer);
			player.Calamity().brimlashBusterBoost = Main.rand.NextBool(3);
		}
    }
}
