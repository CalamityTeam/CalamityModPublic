using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GrandGuardian : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Guardian");
            Tooltip.SetDefault("Lowers enemy defense by 1 with every strike\n" +
                       "If enemy defense is 0 or below your attacks will heal you\n" +
                       "Striking enemies causes a large explosion\n" +
                       "Striking enemies that are under half life will cause them to release rainbow bolts\n" +
                       "Enemies spawn healing orbs on death");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 130;
            Item.height = 130;
            Item.damage = 150;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 22;
            Item.useTurn = true;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.shootSpeed = 12f;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation += new Vector2(-32f * player.direction, 12f * player.gravDir).RotatedBy(player.itemRotation);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.Calamity().miscDefenseLoss < target.defense)
                target.Calamity().miscDefenseLoss += 1;

            if (target.Calamity().miscDefenseLoss >= target.defense && target.canGhostHeal && !player.moonLeech)
            {
                player.statLife += 4;
                player.HealEffect(4);
            }

            OnHitEffects(player, target.Center, target.life, target.lifeMax, knockback, damage, crit);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            OnHitEffects(player, target.Center, target.statLife, target.statLifeMax2, Item.knockBack, damage, crit);
        }

        private void OnHitEffects(Player player, Vector2 targetPos, int targetLife, int targetMaxLife, float knockback, int damage, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            if (crit)
                damage /= 2;

            Projectile.NewProjectile(source, targetPos, Vector2.Zero, ModContent.ProjectileType<RainbowBoom>(), (int)(damage * 0.5f), 0f, player.whoAmI);
            if (targetLife <= (targetMaxLife * 0.5f) && player.ownedProjectileCounts[ModContent.ProjectileType<RainBolt>()] < 3)
            {
                float randomSpeedX = Main.rand.Next(6, 13);
                float randomSpeedY = Main.rand.Next(6, 13);
                Projectile.NewProjectile(source, targetPos.X, targetPos.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<RainBolt>(), (int)(damage * 0.75f), knockback, player.whoAmI);
                Projectile.NewProjectile(source, targetPos.X, targetPos.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<RainBolt>(), (int)(damage * 0.75f), knockback, player.whoAmI);
                Projectile.NewProjectile(source, targetPos.X, targetPos.Y, 0f, -randomSpeedY, ModContent.ProjectileType<RainBolt>(), (int)(damage * 0.75f), knockback, player.whoAmI);
            }
            if (targetLife <= 0 && !player.moonLeech && player.ownedProjectileCounts[ModContent.ProjectileType<RainHeal>()] < 3)
            {
                float randomSpeedX = Main.rand.Next(3, 7);
                float randomSpeedY = Main.rand.Next(3, 7);
                Projectile.NewProjectile(source, targetPos.X, targetPos.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<RainHeal>(), 0, 0f, player.whoAmI);
                Projectile.NewProjectile(source, targetPos.X, targetPos.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<RainHeal>(), 0, 0f, player.whoAmI);
                Projectile.NewProjectile(source, targetPos.X, targetPos.Y, 0f, -randomSpeedY, ModContent.ProjectileType<RainHeal>(), 0, 0f, player.whoAmI);
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MajesticGuard>().
                AddIngredient<BarofLife>(10).
                AddIngredient(ItemID.FragmentSolar, 10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
