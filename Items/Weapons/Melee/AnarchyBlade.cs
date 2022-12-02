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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 114;
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 19;
            Item.useTime = 19;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 122;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, (int)CalamityDusts.Brimstone);
        }

        // Gains 10% of missing health as base damage.
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            int lifeAmount = player.statLifeMax2 - player.statLife;
            damage.Base += lifeAmount * 0.1f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneBoom>(), damage, knockback, Main.myPlayer);
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
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneBoom>(), damage, Item.knockBack, Main.myPlayer);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BrokenHeroSword).
                AddIngredient<UnholyCore>(5).
                AddIngredient<CoreofChaos>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
