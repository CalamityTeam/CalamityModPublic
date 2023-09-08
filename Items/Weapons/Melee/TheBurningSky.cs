using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheBurningSky : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        private const int ProjectilesPerBarrage = 6;

        public override void SetStaticDefaults()
        {

            // Visually a sword, but with no true melee capability. The Burning Sky is held out like a staff.
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 146;
            Item.damage = 147;
            Item.knockBack = 2.5f;

            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item105;

            Item.shoot = ModContent.ProjectileType<BurningMeteor>();
            Item.shootSpeed = 14f;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Every time the item spawns more meteors, play a violent, bass heavy sound to add onto Star Wrath's use sound.
            SoundEngine.PlaySound(SoundID.Item70, player.Center);

            float speed = velocity.Length();
            for (int i = 0; i < ProjectilesPerBarrage; ++i)
            {
                float randomSpeed = speed * Main.rand.NextFloat(0.7f, 1.4f);
                CalamityUtils.ProjectileRain(source, Main.MouseWorld, 290f, 130f, 850f, 1100f, randomSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Dragonfire>(), 300);
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(ModContent.BuffType<Dragonfire>(), 300);
    }
}
