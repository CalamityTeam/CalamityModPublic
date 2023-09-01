using Terraria.DataStructures;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Particles;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class UtensilPoker : RogueWeapon
    {
        private int counter = 0;
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 66;
            Item.damage = 333;
            Item.DamageType = RogueDamageClass.Instance;
            Item.knockBack = 8f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useAnimation = 45;
            Item.reuseDelay = 15;            
            Item.useLimitPerAnimation = 3;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<Fork>();
            Item.shootSpeed = 12f;
        }

		public override float StealthDamageMultiplier => 2f;
        public override float StealthVelocityMultiplier => 1.4f;

        public override void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            CalamityPlayer mp = player.Calamity();
            if (mp.StealthStrikeAvailable())
            {
				type = ModContent.ProjectileType<ButcherKnife>();
			}
			else
			{
                type = ModContent.ProjectileType<Fork>();
                double dmgMult = 1D;
                float kbMult = 1f;
                switch (counter)
                {
                    case 0:
                        type = ModContent.ProjectileType<Fork>();
                        dmgMult = 1.1;
                        kbMult = 2f;
                        break;
                    case 1:
                        type = ModContent.ProjectileType<Knife>();
                        dmgMult = 1.2;
                        kbMult = 1f;
                        break;
                    case 2:
                        type = ModContent.ProjectileType<CarvingFork>();
                        dmgMult = 1D;
                        kbMult = 1f;
                        break;
                    default:
                        break;
                }
				damage = (int)(damage * dmgMult);
				knockback = knockback * kbMult;
                
            }
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityPlayer mp = player.Calamity();

			int idx = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			if (idx.WithinBounds(Main.maxProjectiles))
				Main.projectile[idx].Calamity().stealthStrike = mp.StealthStrikeAvailable();
            counter++;
            if (counter >= 3)
                counter = 0;
            return false;
        }
    }
}
