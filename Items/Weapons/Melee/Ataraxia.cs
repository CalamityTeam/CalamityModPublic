using CalamityMod.Items.Materials;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Ataraxia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ataraxia");
            Tooltip.SetDefault("Equanimity");
        }

        public override void SetDefaults()
        {
            item.width = 94;
            item.height = 92;
            item.melee = true;
            item.damage = 3651;
            item.knockBack = 2.5f;
            item.useAnimation = 19;
            item.useTime = 19;
            item.autoReuse = true;
            item.useTurn = true;

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;

            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
            item.value = Item.buyPrice(platinum: 2, gold: 50);

            item.shoot = ModContent.ProjectileType<AtaraxiaMain>();
            item.shootSpeed = 9f;
        }

        // Fires one large and two small projectiles which stay together in formation.
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Play the Terra Blade sound upon firing
            Main.PlaySound(SoundID.Item60, position);

            // Center projectile
            int centerID = ModContent.ProjectileType<AtaraxiaMain>();
            int centerDamage = damage;
            Vector2 centerVel = new Vector2(speedX, speedY);
            Projectile.NewProjectile(position, centerVel, centerID, centerDamage, knockBack, player.whoAmI, 0f, 0f);

            // Side projectiles (these deal 75% damage)
            int sideID = ModContent.ProjectileType<AtaraxiaSide>();
            int sideDamage = (int)(0.75f * centerDamage);
            Vector2 speed = new Vector2(speedX, speedY);
            speed.Normalize();
            speed *= 22f;
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 leftOffset = speed.RotatedBy(MathHelper.PiOver4, default);
            Vector2 rightOffset = speed.RotatedBy(-MathHelper.PiOver4, default);
            leftOffset -= 1.4f * speed;
            rightOffset -= 1.4f * speed;
            Projectile.NewProjectile(rrp.X + leftOffset.X, rrp.Y + leftOffset.Y, speedX, speedY, sideID, sideDamage, knockBack, player.whoAmI, 0f, 0f);
            Projectile.NewProjectile(rrp.X + rightOffset.X, rrp.Y + rightOffset.Y, speedX, speedY, sideID, sideDamage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        // On-hit, tosses out five homing projectiles. This is not like Holy Collider.
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 480);
            target.AddBuff(BuffID.Ichor, 480);

            // Does not summon extra projectiles versus dummies.
            if (target.type == NPCID.TargetDummy)
                return;

			OnHitEffects(player, target.Center);
        }

        // On-hit, tosses out five homing projectiles. This is not like Holy Collider.
        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 480);
            target.AddBuff(BuffID.Ichor, 480);
			OnHitEffects(player, target.Center);
        }

		private void OnHitEffects(Player player, Vector2 targetPos)
		{

            // Individual true melee homing missiles deal 10% of the weapon's base damage.
            int numSplits = 5;
            int trueMeleeID = ModContent.ProjectileType<AtaraxiaHoming>();
            int trueMeleeDamage = (int)(0.1f * item.damage * player.MeleeDamage());
            float angleVariance = MathHelper.TwoPi / (float)numSplits;
            float spinOffsetAngle = MathHelper.Pi / (2f * numSplits);
            Vector2 posVec = new Vector2(8f, 0f).RotatedByRandom(MathHelper.TwoPi);

            for (int i = 0; i < numSplits; ++i)
            {
                posVec = posVec.RotatedBy(angleVariance);
                Vector2 velocity = new Vector2(posVec.X, posVec.Y).RotatedBy(spinOffsetAngle);
                velocity.Normalize();
                velocity *= 8f;
                Projectile.NewProjectile(targetPos + posVec, velocity, trueMeleeID, trueMeleeDamage, item.knockBack, player.whoAmI, 0.0f, 0.0f);
            }
		}

        // Spawn some fancy dust while swinging
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int dustCount = Main.rand.Next(3, 6);
            Vector2 corner = new Vector2(hitbox.X + hitbox.Width / 4, hitbox.Y + hitbox.Height / 4);
            for (int i = 0; i < dustCount; ++i)
            {
                // Pick a random dust to spawn
                int dustID;
                switch (Main.rand.Next(5))
                {
                    case 0:
                    case 1:
                        dustID = 70;
                        break;
                    case 2:
                        dustID = 71;
                        break;
                    default:
                        dustID = 86;
                        break;
                }
                int idx = Dust.NewDust(corner, hitbox.Width / 2, hitbox.Height / 2, dustID);
                Main.dust[idx].noGravity = true;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.BrokenHeroSword);
			r.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
            r.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 12);
            r.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 4);
            r.AddIngredient(ModContent.ItemType<DarksunFragment>(), 15);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
