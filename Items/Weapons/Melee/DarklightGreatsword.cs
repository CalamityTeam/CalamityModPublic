using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DarklightGreatsword : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        internal const float ShootSpeed = 2f;

        internal const float ProjectileDamageMultiplier = 0.8f;

        internal const float TrueMeleeSlashProjectileDamageMultiplier = 0.8f;

        internal const float SlashProjectileDamageMultiplier = 0.4125f; // Makes them do 33% damage since main projectile does 80% damage

        internal const int SlashProjectileLimit = 4;

        internal const int SlashCreationRate = 18;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Shadowflame>(), BuffID.Frostburn2];
        }

        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 100;
            Item.damage = 75;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<DarkBeam>();
            Item.shootSpeed = ShootSpeed;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 offset = new Vector2(Item.width * 0.25f * -player.direction, Item.height * 0.25f);
            if (player.gravDir == 1f)
                position -= offset;
            else
                position += offset;

            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            velocity = (Main.MouseWorld - position).SafeNormalize(Vector2.UnitY) * ShootSpeed;
            type = Main.rand.NextBool() ? type : ModContent.ProjectileType<LightBeam>();
            Projectile.NewProjectile(source, position, velocity, type, (int)(damage * ProjectileDamageMultiplier), knockback * ProjectileDamageMultiplier, player.whoAmI);
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(Main.rand.NextBool() ? BuffID.Frostburn2 : BuffID.ShadowFlame, 240);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(Main.rand.NextBool() ? BuffID.Frostburn2 : BuffID.ShadowFlame, 240);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>(12).
                AddIngredient(ItemID.SoulofLight).
                AddIngredient(ItemID.SoulofNight).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
