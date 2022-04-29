using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BrimlashBuster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimlash Buster");
            Tooltip.SetDefault("50% chance to do triple damage on enemy hits\n" +
                "Fires a brimstone bolt that explodes into more bolts on death");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 72;
            Item.damage = 126;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<BrimlashProj>();
            Item.shootSpeed = 18f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, (int)CalamityDusts.Brimstone);
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            float damageMult = 1f;
            if (player.Calamity().brimlashBusterBoost)
                damageMult = 2f;
            damage *= damageMult;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            player.Calamity().brimlashBusterBoost = Main.rand.NextBool(3);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            player.Calamity().brimlashBusterBoost = Main.rand.NextBool(3);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Brimlash>().
                AddIngredient<CoreofChaos>(3).
                AddIngredient(ItemID.FragmentSolar, 10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
