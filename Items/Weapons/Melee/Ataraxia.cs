using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Ataraxia : ModItem
    {
        public static int BaseDamage = 5600;

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
            item.damage = BaseDamage;
            item.knockBack = 2.5f;
            item.useAnimation = 18;
            item.useTime = 18;
            item.autoReuse = true;
            item.useTurn = true;

            item.useStyle = 1;
            item.UseSound = SoundID.Item1;

            item.rare = 10;
            item.Calamity().postMoonLordRarity = 21;
            item.value = Item.buyPrice(2, 50, 0, 0);

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
            Vector2 centerVec = new Vector2(speedX, speedY);
            int center = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, centerID, centerDamage, knockBack, player.whoAmI, 0.0f, 0.0f);

            // Side projectiles (these deal 75% damage)
            int sideID = ModContent.ProjectileType<AtaraxiaSide>();
            int sideDamage = (int)(0.75f * centerDamage);
            Vector2 speed = new Vector2(speedX, speedY);
            speed.Normalize();
            speed *= 22f;
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 leftOffset = speed.RotatedBy((double)MathHelper.PiOver4, default);
            Vector2 rightOffset = speed.RotatedBy((double)-MathHelper.PiOver4, default);
            leftOffset -= 1.4f * speed;
            rightOffset -= 1.4f * speed;
            Projectile.NewProjectile(rrp.X + leftOffset.X, rrp.Y + leftOffset.Y, speedX, speedY, sideID, sideDamage, knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(rrp.X + rightOffset.X, rrp.Y + rightOffset.Y, speedX, speedY, sideID, sideDamage, knockBack, player.whoAmI, 0.0f, 0.0f);
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

            // Individual true melee homing missiles deal 10% of the weapon's base damage.
            int numSplits = 5;
            int trueMeleeID = ModContent.ProjectileType<AtaraxiaHoming>();
            int trueMeleeDamage = (int)(0.1f * damage);
            float angleVariance = MathHelper.TwoPi / (float)numSplits;
            float spinOffsetAngle = MathHelper.Pi / (2f * numSplits);
            Vector2 posVec = new Vector2(8f, 0f).RotatedByRandom(MathHelper.TwoPi);

            for (int i = 0; i < numSplits; ++i)
            {
                posVec = posVec.RotatedBy(angleVariance);
                Vector2 velocity = new Vector2(posVec.X, posVec.Y).RotatedBy(spinOffsetAngle);
                velocity.Normalize();
                velocity *= 8f;
                Projectile.NewProjectile(target.Center + posVec, velocity, trueMeleeID, trueMeleeDamage, knockback, player.whoAmI, 0.0f, 0.0f);
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
            r.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 25);
            r.AddIngredient(ModContent.ItemType<Phantoplasm>(), 35);
            r.AddIngredient(ModContent.ItemType<NightmareFuel>(), 90);
            r.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 90);
            r.AddIngredient(ModContent.ItemType<DarksunFragment>(), 65);
            r.AddIngredient(ModContent.ItemType<BarofLife>(), 15);
            r.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 5);
            r.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 10);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
