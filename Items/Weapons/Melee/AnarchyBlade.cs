using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AnarchyBlade : ModItem
    {
        private static int BaseDamage = 150;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anarchy Blade");
            Tooltip.SetDefault("The lower your life the more damage this blade does\n" +
                "Your hits will generate a large explosion\n" +
                "If you're below 50% life your hits have a chance to instantly kill regular enemies");
        }

        public override void SetDefaults()
        {
            item.width = 94;
            item.damage = BaseDamage;
            item.melee = true;
            item.useAnimation = 19;
            item.useTime = 19;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 100;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(ModContent.ItemType<UnholyCore>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, (int)CalamityDusts.Brimstone);
        }

        // Gains 10% of missing health as base damage.
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            int lifeAmount = player.statLifeMax2 - player.statLife;
            flat += lifeAmount * 0.1f * player.MeleeDamage();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
                damage /= 2;

            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneBoom>(), damage, knockback, Main.myPlayer);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);

            if (player.statLife < (player.statLifeMax2 * 0.5f) && Main.rand.NextBool(5))
            {
                if (!CalamityPlayer.areThereAnyDamnBosses && CalamityGlobalNPC.ShouldAffectNPC(target))
                {
                    target.life = 0;
                    target.HitEffect(0, 10.0);
                    target.active = false;
                    target.NPCLoot();
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (crit)
                damage /= 2;

            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneBoom>(), damage, item.knockBack, Main.myPlayer);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
        }
    }
}
