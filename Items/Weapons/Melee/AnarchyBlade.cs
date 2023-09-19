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
    public class AnarchyBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        private static int BaseDamage = 150;

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
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
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

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage *= 0.5f;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_ItemUse(Item);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneBoom>(), Item.damage, Item.knockBack, Main.myPlayer);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);

            if (player.statLife <= (player.statLifeMax2 * 0.5f) && Main.rand.NextBool(5))
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

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            var source = player.GetSource_ItemUse(Item);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneBoom>(), Item.damage, Item.knockBack, Main.myPlayer);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BreakerBlade).
                AddIngredient<UnholyCore>(5).
                AddIngredient<CoreofHavoc>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
